using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOT SERIALIZABLE
public class SimViewEventBase
{
    event Action listeners;

    public void Invoke()
    {

    }
}

struct OnEntityDiedEvent
{
    public GameObject victim;
}

public class User
{
    void Start()
    {
        EventManager.ListenTo<OnEntityDiedEvent>(Test);
    }

    void OnDestroy()
    {
        EventManager.StopListeningTo<OnEntityDiedEvent>(Test);
    }

    void Test(in OnEntityDiedEvent val)
    {
        if(val.victim.name == "Johnny")
        {
            // ...
        }
    }
}

class EntitySystem
{
    void KillEntity(GameObject entity)
    {
        OnEntityDiedEvent data;
        data.victim = entity;

        EventManager.Emit(data);
    }
}


public class EventManager
{
    static Dictionary<Type, List<object>> _callbacksMap = new Dictionary<Type, List<object>>(512);

    public delegate void EventCallbackReadOnly<T>(in T y);

    /// <summary>
    /// NB: Don't forget to call StopListeningToEvent
    /// </summary>
    public static void ListenTo<T>(EventCallbackReadOnly<T> callback)
    {
        if(callback == null)
        {
            DebugService.LogError("EventManager.ListenTo<T>: Trying to register a null callback.");
            return;
        }

        List<object> callbacks = _callbacksMap.GetOrCreate(typeof(T));

#if DEBUG_BUILD
        if (callbacks.Contains(callback))
        {
            DebugService.LogError("EventManager.ListenTo<T>: Trying to register the same callback twice.");
            return;
        }
#endif

        callbacks.Add(callback);
    }

    public static void StopListeningTo<T>(EventCallbackReadOnly<T> callback)
    {
        if (_callbacksMap.TryGetValue(typeof(T), out List<object> callbacks))
        {
            bool hasRemovedItem = callbacks.RemoveWithLastSwap(callback);
            if (!hasRemovedItem)
            {
                DebugService.LogError("EventManager.StopListeningTo<T>: Trying to unregister a callback that is not there.");
            }
        }
    }

    public static void Emit<T>(in T value)
    {
        if (_callbacksMap.TryGetValue(typeof(T), out List<object> callbacks))
        {
            for (int i = 0; i < callbacks.Count; i++)
            {
                if (callbacks[i] is EventCallbackReadOnly<T> callback)
                {
#if DEBUG_BUILD
                    try
                    {
#endif
                        callback.Invoke(value);
#if DEBUG_BUILD
                    }
                    catch (Exception e)
                    {
                        DebugService.LogError("Exception in event listener: " + e.Message);
                    }
#endif
                }
            }
        }
    }
}


public class EventListener
{
    public delegate void EventCallbackReadOnly<T>(in T y);
    List<object> callbacks;

    void OnChannelEmit<T>(in T value)
    {
        for (int i = 0; i < callbacks.Count; i++)
        {
            if (callbacks[i] is EventCallbackReadOnly<T> callback)
            {
#if DEBUG_BUILD
                try
                {
#endif
                    callback.Invoke(value);
#if DEBUG_BUILD
                }
                catch (Exception e)
                {
                    DebugService.LogError("Exception in event listener: " + e.Message);
                }
#endif
            }
        }
    }


    protected void Register<T>(EventCallbackReadOnly<T> callback)
    {
        callbacks.Add(callback);
    }
}

public class EventChannel
{
    List<object> listeners;
}