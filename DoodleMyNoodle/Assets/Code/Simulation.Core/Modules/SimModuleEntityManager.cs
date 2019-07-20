using System.Collections.Generic;
using UnityEngine;

public class SimModuleEntityManager
{
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint original)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject);
        return OnInstantiated_Internal(original, newGameObject); ;
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint original, Transform parent)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, parent);
        return OnInstantiated_Internal(original, newGameObject);
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat());
        return OnInstantiated_Internal(original, newGameObject, position, rotation);
    }
    /// <summary>
    /// Instantiate entity from the blueprint and inject it into the simulation
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal SimEntity Instantiate(SimBlueprint original, in FixVector3 position, in FixQuaternion rotation, SimTransform parent)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat(), parent.unityTransform);
        return OnInstantiated_Internal(original, newGameObject, position, rotation);
    }

    SimEntity OnInstantiated_Internal(SimBlueprint original, GameObject newGameObject, in FixVector3 position, in FixQuaternion rotation)
    {
        SimTransform simTransform = newGameObject.GetComponent<SimTransform>();
        if (simTransform)
        {
            simTransform.localPosition = position;
            simTransform.localRotation = rotation;
        }

        return OnInstantiated_Internal(original, newGameObject);
    }
    SimEntity OnInstantiated_Internal(SimBlueprint original, GameObject newGameObject)
    {
        SimEntity newEntity = newGameObject.GetComponent<SimEntity>();
        InjectNewEntityIntoSim(newEntity, original.id);
        return newEntity;
    }


    /// <summary>
    /// All newly created entities go through here.
    /// <para/>
    /// NB: not called if reloading/reconstructing a saved game
    /// </summary>
    internal void InjectNewEntityIntoSim(SimEntity newEntity, SimBlueprintId blueprintId)
    {
        newEntity.blueprintId = blueprintId;
        newEntity.entityId = SimModules.world.nextEntityId;
        SimModules.world.nextEntityId.Increment();

        AddToEntityList(newEntity);
        foreach (SimComponent comp in newEntity.GetComponents<SimComponent>())
        {
            comp.OnSimAwake();
        }
    }

    int _expectingDestructions = 0;
    /// <summary>
    /// Permanently destroys the entity from the simulation
    /// </summary>
    internal void Destroy(SimEntity entity)
    {
        foreach (SimComponent comp in entity.GetComponents<SimComponent>())
        {
            comp.OnSimDestroy();
        }

        _expectingDestructions++;
        GameObject.Destroy(entity.gameObject);
        _expectingDestructions--;
    }
    /// <summary>
    /// This should be called by the entity itself upon destruction. 
    /// </summary>
    internal void OnDestroyEntity(SimEntity entity)
    {
        if (_expectingDestructions == 0)
        {
            DebugService.LogWarning("A SimEntity is getting incorrectly destroyed. Please call SimWorld.Destroy() to destroy simualation entities.");
        }
        RemoveFromEntityList(entity);
    }


    /// <summary>
    /// Add the entity to the entity-list
    /// <para/>
    /// NB: also called when reloading/reconstructing a saved game
    /// </summary>
    void AddToEntityList(SimEntity simEntity)
    {
        SimModules.world.entities.Add(simEntity);

        foreach (SimComponent comp in simEntity.GetComponents<SimComponent>())
        {
            SimModules.ticker.OnAddSimComponentToSim(comp);
            comp.OnAddedToEntityList();
        }
    }
    void RemoveFromEntityList(SimEntity simEntity)
    {
        foreach (SimComponent comp in simEntity.GetComponents<SimComponent>())
        {
            comp.OnRemovingFromEntityList();
            SimModules.ticker.OnRemovingSimComponentToSim(comp);
        }
        SimModules.world.entities.Remove(simEntity);
    }
}