using System;
using System.Collections.Generic;
using UnityEngine;

public class SimModuleEntityManager : IDisposable
{
    List<ISimEntityListChangeObserver> _changeObservers = new List<ISimEntityListChangeObserver>();

    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimEntity entity)
    {
        GameObject newGameObject = GameObject.Instantiate(entity.gameObject);
        return OnInstantiated_Internal(entity.BlueprintId, newGameObject); ;
    }
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimEntity entity, Transform parent)
    {
        GameObject newGameObject = GameObject.Instantiate(entity.gameObject, parent);
        return OnInstantiated_Internal(entity.BlueprintId, newGameObject); ;
    }
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimEntity entity, in FixVector3 position, in FixQuaternion rotation)
    {
        GameObject newGameObject = GameObject.Instantiate(entity.gameObject, position.ToUnityVec(), rotation.ToUnityQuat());
        return OnInstantiated_Internal(entity.BlueprintId, newGameObject); ;
    }
    /// <summary>
    /// Duplicate the entity using Unity's traditional Instantiate replication model and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimEntity entity, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
    {
        GameObject newGameObject = GameObject.Instantiate(entity.gameObject, position.ToUnityVec(), rotation.ToUnityQuat(), parent.UnityTransform);
        return OnInstantiated_Internal(entity.BlueprintId, newGameObject); ;
    }

    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprintId blueprintId)
        => Instantiate(SimModules.BlueprintBank.GetBlueprint(blueprintId));
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprintId blueprintId, Transform parent)
        => Instantiate(SimModules.BlueprintBank.GetBlueprint(blueprintId), parent);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprintId blueprintId, in FixVector3 position, in FixQuaternion rotation)
        => Instantiate(SimModules.BlueprintBank.GetBlueprint(blueprintId), position, rotation);
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprintId blueprintId, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
        => Instantiate(SimModules.BlueprintBank.GetBlueprint(blueprintId), position, rotation, parent);

    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint blueprint)
    {
        GameObject newGameObject = GameObject.Instantiate(blueprint.prefab.gameObject);
        return OnInstantiated_Internal(blueprint.id, newGameObject); ;
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint blueprint, Transform parent)
    {
        GameObject newGameObject = GameObject.Instantiate(blueprint.prefab.gameObject, parent);
        return OnInstantiated_Internal(blueprint.id, newGameObject);
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint blueprint, in FixVector3 position, in FixQuaternion rotation)
    {
        GameObject newGameObject = GameObject.Instantiate(blueprint.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat());
        return OnInstantiated_Internal(blueprint.id, newGameObject, position, rotation);
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint blueprint, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
    {
        GameObject newGameObject = GameObject.Instantiate(blueprint.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat(), parent.UnityTransform);
        return OnInstantiated_Internal(blueprint.id, newGameObject, position, rotation);
    }

    SimEntity OnInstantiated_Internal(SimBlueprintId blueprintId, GameObject newGameObject, in FixVector3 position, in FixQuaternion rotation)
    {
        SimTransformComponent simTransform = newGameObject.GetComponent<SimTransformComponent>();
        if (simTransform)
        {
            simTransform.localPosition = position;
            simTransform.localRotation = rotation;
        }

        return OnInstantiated_Internal(blueprintId, newGameObject);
    }
    SimEntity OnInstantiated_Internal(SimBlueprintId blueprintId, GameObject newGameObject)
    {
        SimEntity newEntity = newGameObject.GetComponent<SimEntity>();
        InjectNewEntityIntoSim(newEntity, blueprintId);
        return newEntity;
    }


    /// <summary>
    /// All newly created entities go through here.
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal void InjectNewEntityIntoSim(SimEntity newEntity, SimBlueprintId blueprintId)
    {
        newEntity.BlueprintId = blueprintId;
        newEntity.EntityId = SimModules.World.NextEntityId;
        SimModules.World.NextEntityId++;

        SimModules.World.Entities.Add(newEntity);

        foreach (SimObject obj in newEntity.GetComponents<SimObject>())
        {
            obj.OnSimAwake();

            // This should eventually cause the OnSimStart() method to get called
            SimModules.World.ObjectsThatHaventStartedYet.Add(obj);
        }

        AddEntityToRuntime(newEntity);
    }

    internal int PendingPermanentEntityDestructions = 0;

    /// <summary>
    /// Permanently destroys the entity from the simulation
    /// </summary>
    internal void Destroy(SimEntity entity)
    {
        foreach (SimObject obj in entity.GetComponents<SimObject>())
        {
            obj.OnSimDestroy();

            SimModules.World.ObjectsThatHaventStartedYet.RemoveWithLastSwap(obj);
        }

        PendingPermanentEntityDestructions++;
        GameObject.Destroy(entity.gameObject);
    }
    /// <summary>
    /// This should be called by the entity itself upon destruction. 
    /// </summary>
    internal void OnDestroyEntity(SimEntity entity)
    {
        if (PendingPermanentEntityDestructions == 0 && !SimModules.IsDisposed)
        {
            DebugService.LogWarning("A SimEntity is getting incorrectly destroyed. Please call SimWorld.Destroy() to destroy simulation entities.");
        }

        if (PendingPermanentEntityDestructions > 0)
        {
            SimModules.World.Entities.Remove(entity);
            PendingPermanentEntityDestructions--;
        }

        RemoveEntityFromRuntime(entity);
    }


    /// <summary>
    /// Add the entity to the entity-list
    /// <para/>
    /// NB: also called when reloading/reconstructing a saved game
    /// </summary>
    void AddEntityToRuntime(SimEntity simEntity)
    {
        foreach (SimObject obj in simEntity.GetComponents<SimObject>())
        {
            SimModules.Ticker.OnAddSimObjectToSim(obj);
            SimModules.InputProcessorManager.OnAddSimObjectToSim(obj);

            obj.OnAddedToRuntime();

            foreach (ISimEntityListChangeObserver existingObserver in _changeObservers)
            {
                existingObserver.OnAddSimObjectToRuntime(obj);
            }

            if (obj is ISimEntityListChangeObserver newObserver)
            {
                _changeObservers.Add(newObserver);
            }
        }
    }
    void RemoveEntityFromRuntime(SimEntity simEntity)
    {
        foreach (SimObject obj in simEntity.GetComponents<SimObject>())
        {
            if (obj is ISimEntityListChangeObserver oldObserver)
            {
                _changeObservers.Remove(oldObserver);
            }

            foreach (ISimEntityListChangeObserver existingObserver in _changeObservers)
            {
                existingObserver.OnRemoveSimObjectFromRuntime(obj);
            }

            obj.OnRemovingFromRuntime();

            SimModules.InputProcessorManager.OnRemovingSimObjectFromSim(obj);
            SimModules.Ticker.OnRemovingSimObjectFromSim(obj);
        }
    }

    public void Dispose() { }
}