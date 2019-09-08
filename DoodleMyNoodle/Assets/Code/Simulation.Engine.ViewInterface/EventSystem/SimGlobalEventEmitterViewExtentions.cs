using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimGlobalEventEmitterView
{
    /// <summary>
    /// Safe event registration. The emitter and listener can safely be destroyed without causing nullptr exceptions.
    /// </summary>
    public static bool RegisterListenerFromView<T>(ISimEventListener<T> listener)
        => SimGlobalEventEmitterInternal.RegisterListenerFromView(listener);
    /// <summary>
    /// Identical to RegisterEventListener, but accepts listeners that are 'unsafe'. This means the listener has to 
    /// manually call UnregisterEventListener upon its destruction. Use the safe method if possible.
    /// </summary>
    public static bool RegisterListenerFromViewUnsafe<T>(ISimEventListenerUnsafe<T> listener)
        => SimGlobalEventEmitterInternal.RegisterListenerFromViewUnsafe(listener);

    public static bool UnregisterListenerFromView<T>(ISimEventListener<T> listener)
        => SimGlobalEventEmitterInternal.UnregisterListenerFromView(listener);
    public static bool UnregisterListenerFromViewUnsafe<T>(ISimEventListenerUnsafe<T> listener)
        => SimGlobalEventEmitterInternal.UnregisterListenerFromViewUnsafe(listener);
}
