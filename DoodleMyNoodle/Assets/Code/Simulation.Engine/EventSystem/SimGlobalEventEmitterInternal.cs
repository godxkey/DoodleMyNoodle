using System;
using System.Collections.Generic;


public class SimGlobalEventEmitterInternal : SimSingleton<SimGlobalEventEmitterInternal>
{
    Dictionary<Type, SimEventInternal> _eventsMap = new Dictionary<Type, SimEventInternal>();


    /// <summary>
    /// Safe event registration. The emitter and listener can safely be destroyed without causing nullptr exceptions.
    /// </summary>
    internal static bool RegisterListener<T>(ISimEventListener<T> listener)
    {
        return RegisterListenerBase(listener, SimEventInternal.SerializedBehavior.Serialized);
    }
    /// <summary>
    /// Identical to RegisterEventListener, but accepts listeners that are 'unsafe'. This means the listener has to 
    /// manually call UnregisterEventListener upon its destruction. Use the safe method if possible.
    /// </summary>
    internal static bool RegisterListenerUnsafe<T>(ISimEventListenerUnsafe<T> listener)
    {
        return RegisterListenerBaseUnsafe(listener, SimEventInternal.SerializedBehavior.Serialized);
    }

    /// <summary>
    /// Safe event registration. The emitter and listener can safely be destroyed without causing nullptr exceptions.
    /// </summary>
    internal static bool RegisterListenerFromView<T>(ISimEventListener<T> listener)
    {
        return RegisterListenerBase(listener, SimEventInternal.SerializedBehavior.NotSerialized);
    }
    /// <summary>
    /// Identical to RegisterEventListener, but accepts listeners that are 'unsafe'. This means the listener has to 
    /// manually call UnregisterEventListener upon its destruction. Use the safe method if possible.
    /// </summary>
    internal static bool RegisterListenerFromViewUnsafe<T>(ISimEventListenerUnsafe<T> listener)
    {
        return RegisterListenerBaseUnsafe(listener, SimEventInternal.SerializedBehavior.NotSerialized);
    }

    internal static bool UnregisterListener<T>(ISimEventListener<T> listener)
    {
        return UnregisterListenerBase(listener, SimEventInternal.SerializedBehavior.Serialized);
    }
    internal static bool UnregisterListenerUnsafe<T>(ISimEventListenerUnsafe<T> listener)
    {
        return UnregisterListenerBaseUnsafe(listener, SimEventInternal.SerializedBehavior.Serialized);
    }

    internal static bool UnregisterListenerFromView<T>(ISimEventListener<T> listener)
    {
        return UnregisterListenerBase(listener, SimEventInternal.SerializedBehavior.NotSerialized);
    }
    internal static bool UnregisterListenerFromViewUnsafe<T>(ISimEventListenerUnsafe<T> listener)
    {
        return UnregisterListenerBaseUnsafe(listener, SimEventInternal.SerializedBehavior.NotSerialized);
    }

    internal static void RaiseEvent<T>(in T value)
    {
        if (Instance)
        {
            if (Instance._eventsMap.TryGetValue(typeof(T), out SimEventInternal evnt))
            {
                evnt.Raise_Internal(value);
            }
        }
        else
            DebugService.LogError("Cannot raise global event, SimGlobalEventEmitter has no valid instance");
    }

    static bool RegisterListenerBase<T>(ISimEventListener<T> listener, SimEventInternal.SerializedBehavior serializedBehavior)
    {
        if (GetInstanceWithError(out SimGlobalEventEmitterInternal instance))
        {
            return instance._eventsMap.GetOrAdd(typeof(T)).RegisterListener_Internal(listener, serializedBehavior);
        }

        return false;
    }
    static bool RegisterListenerBaseUnsafe<T>(ISimEventListenerUnsafe<T> listener, SimEventInternal.SerializedBehavior serializedBehavior)
    {
        if (GetInstanceWithError(out SimGlobalEventEmitterInternal instance))
        {
            return instance._eventsMap.GetOrAdd(typeof(T)).RegisterListenerUnsafe_Internal(listener, serializedBehavior);
        }

        return false;
    }

    static bool UnregisterListenerBase<T>(ISimEventListener<T> listener, SimEventInternal.SerializedBehavior serializedBehavior)
    {
        if (GetInstanceWithError(out SimGlobalEventEmitterInternal instance))
        {
            if (instance._eventsMap.TryGetValue(typeof(T), out SimEventInternal evnt))
            {
                return evnt.UnregisterListener_Internal(listener, serializedBehavior);
            }
            else
            {
                DebugService.LogError($"{nameof(SimGlobalEventEmitterInternal)}.UnregisterListener: Trying to unregister a listener that was never registered.");
                return false;
            }
        }

        return false;
    }
    static bool UnregisterListenerBaseUnsafe<T>(ISimEventListenerUnsafe<T> listener, SimEventInternal.SerializedBehavior serializedBehavior)
    {
        if (GetInstanceWithError(out SimGlobalEventEmitterInternal instance))
        {
            if (instance._eventsMap.TryGetValue(typeof(T), out SimEventInternal evnt))
            {
                return evnt.UnregisterListenerUnsafe_Internal(listener, serializedBehavior);
            }
            else
            {
                DebugService.LogError($"{nameof(SimGlobalEventEmitterInternal)}.UnregisterListener: Trying to unregister a listener that was never registered.");
                return false;
            }
        }

        return false;
    }

    static bool GetInstanceWithError(out SimGlobalEventEmitterInternal instance)
    {
        instance = Instance;
        if (instance)
        {
            return true;
        }
        else
        {
            DebugService.LogError("SimGlobalEventEmitter has no valid instance");
            return false;
        }
    }
}