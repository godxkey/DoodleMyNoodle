using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimWorld
{
    internal uint tickId = 0;
    internal List<SimEntity> entities = new List<SimEntity>();
    internal SimEntityId nextEntityId = SimEntityId.firstValid;

    //[NonSerialized]
    //ReadOnlyCollection<SimEntity> _entitiesReadOnly;

    //[NonSerialized]
    //readonly List<ISimTickable> _tickables = new List<ISimTickable>();

    //[NonSerialized]
    //int pendingSceneLoads = 0;


    ////public ReadOnlyCollection<SimEntity> entities => _entitiesReadOnly ?? (_entitiesReadOnly = _entities.AsReadOnly());
    //public bool canBeTicked => pendingSceneLoads == 0;
    //public bool canBeSaved => pendingSceneLoads == 0;

    ///// <summary>
    ///// Instantiate all gameobjects in the given scene and inject them all into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public void LoadScene(string sceneName)
    //{
    //    // fbessette: eventually, we'll want to have a preloading mechanism outside of the simulation so we don't have a CPU spike here.
    //    pendingSceneLoads++;
    //    SceneService.Load(sceneName, LoadSceneMode.Additive, (scene) =>
    //    {
    //        pendingSceneLoads--;
    //        foreach (GameObject gameObject in scene.GetRootGameObjects())
    //        {
    //            SimEntity newEntity = gameObject.GetComponent<SimEntity>();
    //            if (newEntity)
    //            {
    //                InjectNewEntityIntoSim(newEntity, new SimBlueprintId(SimBlueprintId.Type.SceneGameObject, "TODO"));
    //            }
    //        }
    //    });
    //}

    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public SimEntity Instantiate(SimBlueprint original)
    //{
    //    GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject);
    //    return OnInstantiated_Internal(original, newGameObject); ;
    //}
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public SimEntity Instantiate(SimBlueprint original, Transform parent)
    //{
    //    GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, parent);
    //    return OnInstantiated_Internal(original, newGameObject);
    //}
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public SimEntity Instantiate(SimBlueprint original, FixVector3 position, FixQuaternion rotation)
    //{
    //    GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat());
    //    return OnInstantiated_Internal(original, newGameObject, ref position, ref rotation);
    //}
    ///// <summary>
    ///// Instantiate entity from the blueprint and inject it into the simulation
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //public SimEntity Instantiate(SimBlueprint original, FixVector3 position, FixQuaternion rotation, SimTransform parent)
    //{
    //    GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat(), parent.unityTransform);
    //    return OnInstantiated_Internal(original, newGameObject, ref position, ref rotation);
    //}

    //SimEntity OnInstantiated_Internal(SimBlueprint original, GameObject newGameObject, ref FixVector3 position, ref FixQuaternion rotation)
    //{
    //    SimTransform simTransform = newGameObject.GetComponent<SimTransform>();
    //    if (simTransform)
    //    {
    //        simTransform.localPosition = position;
    //        simTransform.localRotation = rotation;
    //    }

    //    return OnInstantiated_Internal(original, newGameObject);
    //}
    //SimEntity OnInstantiated_Internal(SimBlueprint original, GameObject newGameObject)
    //{
    //    SimEntity newEntity = newGameObject.GetComponent<SimEntity>();
    //    InjectNewEntityIntoSim(newEntity, original.id);
    //    return newEntity;
    //}


    ///// <summary>
    ///// All newly created entities go through here.
    ///// <para/>
    ///// NB: not called if reloading/reconstructing a saved game
    ///// </summary>
    //void InjectNewEntityIntoSim(SimEntity newEntity, SimBlueprintId blueprintId)
    //{
    //    newEntity.blueprintId = blueprintId;
    //    newEntity.entityId = nextEntityId;
    //    nextEntityId.Increment();

    //    AddToEntityList(newEntity);
    //    foreach (SimComponent comp in newEntity.GetComponents<SimComponent>())
    //    {
    //        comp.OnSimAwake();
    //    }
    //}

    //int _expectingDestructions = 0;
    ///// <summary>
    ///// Permanently destroys the entity from the simulation
    ///// </summary>
    //public void Destroy(SimEntity entity)
    //{
    //    foreach (SimComponent comp in entity.GetComponents<SimComponent>())
    //    {
    //        comp.OnSimDestroy();
    //    }

    //    _expectingDestructions++;
    //    GameObject.Destroy(entity.gameObject);
    //    _expectingDestructions--;
    //}
    ///// <summary>
    ///// This should be called by the entity itself upon destruction. 
    ///// </summary>
    //internal void OnDestroyEntity(SimEntity entity)
    //{
    //    if (_expectingDestructions == 0)
    //    {
    //        DebugService.LogWarning("A SimEntity is getting incorrectly destroyed. Please call SimWorld.Destroy() to destroy simualation entities.");
    //    }
    //    RemoveFromEntityList(entity);
    //}


    ///// <summary>
    ///// Add the entity to the entity-list
    ///// <para/>
    ///// NB: also called when reloading/reconstructing a saved game
    ///// </summary>
    //void AddToEntityList(SimEntity simEntity)
    //{
    //    _entities.Add(simEntity);

    //    foreach (SimComponent comp in simEntity.GetComponents<SimComponent>())
    //    {
    //        if (comp is ISimTickable)
    //        {
    //            _tickables.Add((ISimTickable)comp);
    //        }
    //        comp.OnAddedToEntityList();
    //    }
    //}
    //void RemoveFromEntityList(SimEntity simEntity)
    //{
    //    foreach (SimComponent comp in simEntity.GetComponents<SimComponent>())
    //    {
    //        if (comp is ISimTickable)
    //        {
    //            _tickables.Remove((ISimTickable)comp);
    //        }
    //        comp.OnRemovingFromEntityList();
    //    }
    //    _entities.Remove(simEntity);
    //}

    //internal void Tick_PostInput()
    //{
    //    for (int i = 0; i < _tickables.Count; i++)
    //    {
    //        if (_tickables[i].isActiveAndEnabled)
    //        {
    //            _tickables[i].OnSimTick();
    //        }
    //    }
    //}
}
