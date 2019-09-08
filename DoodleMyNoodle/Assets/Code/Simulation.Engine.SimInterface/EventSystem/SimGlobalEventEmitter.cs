using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimGlobalEventEmitter
{
    /// <summary>
    /// Safe event registration. The emitter and listener can safely be destroyed without causing nullptr exceptions.
    /// </summary>
    public static bool RegisterListener<T>(ISimEventListener<T> listener)
        => SimGlobalEventEmitterInternal.RegisterListener(listener);
    /// <summary>
    /// Identical to RegisterEventListener, but accepts listeners that are 'unsafe'. This means the listener has to 
    /// manually call UnregisterEventListener upon its destruction. Use the safe method if possible.
    /// </summary>
    public static bool RegisterListenerUnsafe<T>(ISimEventListenerUnsafe<T> listener)
        => SimGlobalEventEmitterInternal.RegisterListenerUnsafe(listener);


    public static bool UnregisterListener<T>(ISimEventListener<T> listener)
        => SimGlobalEventEmitterInternal.UnregisterListener(listener);
    public static bool UnregisterListenerUnsafe<T>(ISimEventListenerUnsafe<T> listener)
        => SimGlobalEventEmitterInternal.UnregisterListenerUnsafe(listener);


    public static void RaiseEvent<T>(in T value)
        => SimGlobalEventEmitterInternal.RaiseEvent(value);
}
