using System;
using System.Collections.Generic;
using UnityEngine;

internal class SimModuleEntityManager : SimModuleBase
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
    internal SimEntity Instantiate(SimEntity entity, SimTransformComponent parent)
    {
        GameObject newGameObject = GameObject.Instantiate(entity.gameObject, parent.UnityTransform);
        SetNewGameObjectTransform(newGameObject, parent);
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

        SetNewGameObjectTransform(newGameObject, position, rotation);

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
        SetNewGameObjectTransform(newGameObject, position, rotation, parent);
        return OnInstantiated_Internal(entity.BlueprintId, newGameObject); ;
    }

    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //internal SimEntity Instantiate(in SimBlueprintId blueprintId)
    //    => Instantiate(SimModules._BlueprintManager.GetBlueprint(blueprintId));
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //internal SimEntity Instantiate(in SimBlueprintId blueprintId, Transform parent)
    //    => Instantiate(SimModules._BlueprintManager.GetBlueprint(blueprintId), parent);
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //internal SimEntity Instantiate(in SimBlueprintId blueprintId, in FixVector3 position, in FixQuaternion rotation)
    //    => Instantiate(SimModules._BlueprintManager.GetBlueprint(blueprintId), position, rotation);
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //internal SimEntity Instantiate(in SimBlueprintId blueprintId, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
    //    => Instantiate(SimModules._BlueprintManager.GetBlueprint(blueprintId), position, rotation, parent);

    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprint blueprint)
    {
        if (!ValidateBlueprint(blueprint))
            return null;
        GameObject newGameObject = GameObject.Instantiate(blueprint.Prefab.gameObject);
        return OnInstantiated_Internal(blueprint.Id, newGameObject); ;
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprint blueprint, SimTransformComponent parent)
    {
        if (!ValidateBlueprint(blueprint))
            return null;
        GameObject newGameObject = GameObject.Instantiate(blueprint.Prefab.gameObject, parent.UnityTransform);
        SetNewGameObjectTransform(newGameObject, parent);
        return OnInstantiated_Internal(blueprint.Id, newGameObject);
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprint blueprint, in FixVector3 position, in FixQuaternion rotation)
    {
        if (!ValidateBlueprint(blueprint))
            return null;
        GameObject newGameObject = GameObject.Instantiate(blueprint.Prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat());
        SetNewGameObjectTransform(newGameObject, position, rotation);
        return OnInstantiated_Internal(blueprint.Id, newGameObject, position, rotation);
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(in SimBlueprint blueprint, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
    {
        if (!ValidateBlueprint(blueprint))
            return null;
        GameObject newGameObject = GameObject.Instantiate(blueprint.Prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat(), parent.UnityTransform);
        SetNewGameObjectTransform(newGameObject, position, rotation, parent);
        return OnInstantiated_Internal(blueprint.Id, newGameObject, position, rotation);
    }

    void SetNewGameObjectTransform(GameObject newGameObject, in FixVector3 position, in FixQuaternion rotation, SimTransformComponent parent)
    {
        SimTransformComponent simTransformComponent = newGameObject.GetComponent<SimTransformComponent>();
        if (simTransformComponent)
        {
            simTransformComponent.SetParent(parent);
            simTransformComponent.LocalPosition = position;
            simTransformComponent.LocalRotation = rotation;
        }
    }
    void SetNewGameObjectTransform(GameObject newGameObject, SimTransformComponent parent)
    {
        SimTransformComponent simTransformComponent = newGameObject.GetComponent<SimTransformComponent>();
        if (simTransformComponent)
        {
            simTransformComponent.SetParent(parent);
        }
    }
    void SetNewGameObjectTransform(GameObject newGameObject, in FixVector3 position, in FixQuaternion rotation)
    {
        SimTransformComponent simTransformComponent = newGameObject.GetComponent<SimTransformComponent>();
        if (simTransformComponent)
        {
            simTransformComponent.LocalPosition = position;
            simTransformComponent.LocalRotation = rotation;
        }
    }


    bool ValidateBlueprint(in SimBlueprint bp)
    {
        if (bp.IsValid)
            return true;
        else
        {
            Debug.LogWarning($"Invalid blueprint with Id: {bp.Id}. Try updating the blueprint bank with Tools > UpdateSimBlueprintBank.");
            return false;
        }
    }

    SimEntity OnInstantiated_Internal(in SimBlueprintId blueprintId, GameObject newGameObject, in FixVector3 position, in FixQuaternion rotation)
    {
        SimTransformComponent simTransform = newGameObject.GetComponent<SimTransformComponent>();
        if (simTransform)
        {
            simTransform.LocalPosition = position;
            simTransform.LocalRotation = rotation;
        }

        return OnInstantiated_Internal(blueprintId, newGameObject);
    }
    SimEntity OnInstantiated_Internal(in SimBlueprintId blueprintId, GameObject newGameObject)
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
    internal void InjectNewEntityIntoSim(SimEntity newEntity, in SimBlueprintId blueprintId)
    {
        newEntity.BlueprintId = blueprintId;

        SimModules._World.Entities.Add(newEntity);
        AddEntityToRuntime(newEntity);

        // fbessette: This used to be a cached List (to prevent allocation).
        //            Since entities can instantiate other entities in their OnSimAwake, the iterator could break by being modified
        //            while still being iterated on. Now we create a new list every, which allocates a lot of garbage. We should
        //            find a non-allocating solution for this (investigate pooling of component iterators with a disposable pattern ?) TODO
        foreach (SimObject obj in newEntity.GetComponents<SimObject>())
        {
            // assign id
            obj.SimObjectId = SimModules._World.NextObjectId;
            SimModules._World.NextObjectId++;

            obj.OnSimAwake();

            // This should eventually cause the OnSimStart() method to get called
            SimModules._World.ObjectsThatHaventStartedYet.Add(obj);
        }
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

            SimModules._World.ObjectsThatHaventStartedYet.RemoveWithLastSwap(obj);
        }

        PendingPermanentEntityDestructions++;
        GameObject.Destroy(entity.gameObject);
    }
    /// <summary>
    /// This should be called by the entity itself upon destruction. 
    /// </summary>
    internal void OnDestroyEntity(SimEntity entity)
    {
        if (PendingPermanentEntityDestructions == 0 && !SimModules._IsDisposed)
        {
            DebugService.LogWarning("A SimEntity is getting incorrectly destroyed. Please call SimWorld.Destroy() to destroy simulation entities.");
        }

        if (PendingPermanentEntityDestructions > 0)
        {
            SimModules._World.Entities.Remove(entity);
            PendingPermanentEntityDestructions--;
        }

        RemoveEntityFromRuntime(entity);
    }

    /// <summary>
    /// Add the entity to the entity-list
    /// <para/>
    /// NB: also called when reloading/reconstructing a saved game
    /// </summary>
    internal void AddEntityToRuntime(SimEntity simEntity)
    {
        foreach (SimObject obj in simEntity.GetComponents<SimObject>())
        {
            SimModules._Ticker.OnAddSimObjectToSim(obj);
            SimModules._InputProcessorManager.OnAddSimObjectToSim(obj);

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
    internal void RemoveEntityFromRuntime(SimEntity simEntity)
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

            SimModules._InputProcessorManager.OnRemovingSimObjectFromSim(obj);
            SimModules._Ticker.OnRemovingSimObjectFromSim(obj);
        }
    }
}