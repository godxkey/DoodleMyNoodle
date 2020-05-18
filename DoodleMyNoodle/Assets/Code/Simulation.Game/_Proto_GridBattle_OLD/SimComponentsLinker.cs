using CCC.InspectorDisplay;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngineX.InspectorDisplay;

// TODO : Changer en global event
public class SimComponentsLinker : SimComponent
{
    [System.Serializable]
    struct SerializedData
    {
        public List<SimComponent> PlayerComponents;
    }

    public List<SimComponent> PlayerComponents { get => _data.PlayerComponents; }

    // TODO : Changer en des SimEvents
    public class PlayerComponentEvent : UnityEvent<SimComponent> { }
    public PlayerComponentEvent OnComponentAdded = new PlayerComponentEvent();
    public PlayerComponentEvent OnComponentRemoved = new PlayerComponentEvent();

    public T AddComponent<T>() where T : SimComponent
    {
        T newComponent = Simulation.AddComponent<T>(SimEntity);
        PlayerComponents.Add(newComponent);
        OnComponentAdded?.Invoke(newComponent);
        return newComponent;
    }

    public void RemoveComponent<T>() where T : SimComponent
    {
        SimComponent oldComponent = GetComponent<T>();
        PlayerComponents.Remove(oldComponent);
        OnComponentRemoved?.Invoke(oldComponent);
        Simulation.RemoveComponent<T>(SimEntity);
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
