using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimComponentsLinker : SimComponent
{
    private List<SimComponent> _playerComponents = new List<SimComponent>();

    public class PlayerComponentEvent : UnityEvent<SimComponent> { }
    public PlayerComponentEvent OnComponentAdded = new PlayerComponentEvent();
    public PlayerComponentEvent OnComponentRemoved = new PlayerComponentEvent();

    public T AddComponent<T>() where T : SimComponent
    {
        T newComponent = Simulation.AddComponent<T>(SimEntity);
        _playerComponents.Add(newComponent);
        OnComponentAdded?.Invoke(newComponent);
        return newComponent;
    }

    public void RemoveComponent<T>() where T : SimComponent
    {
        SimComponent oldComponent = GetComponent<T>();
        _playerComponents.Remove(oldComponent);
        OnComponentRemoved?.Invoke(oldComponent);
        Simulation.RemoveComponent<T>(SimEntity);
    }
}
