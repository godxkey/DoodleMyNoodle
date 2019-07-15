using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimWorld : IDisposable
{
    public ReadOnlyCollection<SimEntity> entities => _entitiesReadOnly ?? (_entitiesReadOnly = _entities.AsReadOnly());

    [NonSerialized]
    ReadOnlyCollection<SimEntity> _entitiesReadOnly;

    readonly List<SimEntity> _entities = new List<SimEntity>();
    SimEntityId _nextEntityId = SimEntityId.firstValid;

    [NonSerialized]
    readonly List<SimComponent> _simComponentBuffer = new List<SimComponent>();

    [NonSerialized]
    readonly List<ISimTickable> _tickables = new List<ISimTickable>();



    public void LoadScene(string sceneName)
    {
        throw new NotImplementedException();
    }

    public SimEntity Instantiate(SimBlueprint original)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject);
        return OnInstantiated_Internal(original, newGameObject); ;
    }
    public SimEntity Instantiate(SimBlueprint original, Transform parent)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, parent);
        return OnInstantiated_Internal(original, newGameObject);
    }
    public SimEntity Instantiate(SimBlueprint original, FixVector3 position, FixQuaternion rotation)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat());
        return OnInstantiated_Internal(original, newGameObject, ref position, ref rotation);
    }
    public SimEntity Instantiate(SimBlueprint original, FixVector3 position, FixQuaternion rotation, SimTransform parent)
    {
        GameObject newGameObject = GameObject.Instantiate(original.prefab.gameObject, position.ToUnityVec(), rotation.ToUnityQuat(), parent.unityTransform);
        return OnInstantiated_Internal(original, newGameObject, ref position, ref rotation);
    }

    SimEntity OnInstantiated_Internal(SimBlueprint original, GameObject newGameObject, ref FixVector3 position, ref FixQuaternion rotation)
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
        newEntity.blueprintId = original.id;
        newEntity.entityId = _nextEntityId;
        _nextEntityId.Increment();

        AddToEntityList(newEntity);
        OnAllSimComponents(newEntity, (x) => x.OnSimAwake());

        return newEntity;
    }

    int _expectingDestructions = 0;
    public void Destroy(SimEntity entity)
    {
        OnAllSimComponents(entity, (x) => x.OnSimDestroy());

        _expectingDestructions++;
        GameObject.Destroy(entity.gameObject);
        _expectingDestructions--;
    }
    internal void OnDestroyEntity(SimEntity entity)
    {
        if (_expectingDestructions == 0)
        {
            DebugService.LogWarning("A SimEntity is getting incorrectly destroyed. Please call SimWorld.Destroy() to destroy simualation entities.");
        }
        RemoveFromEntityList(entity);
    }


    void AddToEntityList(SimEntity simEntity)
    {
        _entities.Add(simEntity);
        OnAllSimComponents(simEntity, (x) =>
        {
            if (x is ISimTickable)
            {
                _tickables.Add((ISimTickable)x);
            }
            x.OnAddedToEntityList();
        });
    }
    void RemoveFromEntityList(SimEntity simEntity)
    {
        OnAllSimComponents(simEntity, (x) =>
        {
            if (x is ISimTickable)
            {
                _tickables.Remove((ISimTickable)x);
            }
            x.OnRemovingFromEntityList();
        });
        _entities.Remove(simEntity);
    }

    void OnAllSimComponents(SimEntity entity, Action<SimComponent> action)
    {
        entity.GetComponents(_simComponentBuffer);
        for (int i = 0; i < _simComponentBuffer.Count; i++)
        {
            action(_simComponentBuffer[i]);
        }
        _simComponentBuffer.Clear();
    }

    internal void Tick_PostInput()
    {
        for (int i = 0; i < _tickables.Count; i++)
        {
            if (_tickables[i].isActiveAndEnabled)
            {
                _tickables[i].OnSimTick();
            }
        }
    }
    public void Dispose() { }
}
