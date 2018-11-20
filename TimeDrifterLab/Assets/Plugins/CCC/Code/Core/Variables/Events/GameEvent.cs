using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CCC/GameEvent")]
public class GameEvent : ScriptableObject
{
    private List<Action> eventListeners;

    void OnEnable()
    {
        eventListeners = new List<Action>();
    }

    public void Raise()
    {
        for (int i = eventListeners.Count - 1; i >= 0; i--)
            eventListeners[i].Invoke();
    }

    public void Subscribe(Action listener)
    {
        if (!eventListeners.Contains(listener))
            eventListeners.Add(listener);
    }

    public void Unsubscribe(Action listener)
    {
        eventListeners.Remove(listener);
    }

    public void UnsubscribeAll()
    {
        eventListeners.Clear();
    }
}