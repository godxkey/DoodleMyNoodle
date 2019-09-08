using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FOR THE VIEW

public static class SimEventExtensions
{
    public static bool RegisterListenerFromView<T>(this SimEvent<T> simEvent, ISimEventListener<T> listener) 
        => simEvent.RegisterListener_Internal(listener, SimEventInternal.SerializedBehavior.NotSerialized);
    public static bool RegisterListenerFromViewUnsafe<T>(this SimEvent<T> simEvent, ISimEventListenerUnsafe<T> listener) 
        => simEvent.RegisterListenerUnsafe_Internal(listener, SimEventInternal.SerializedBehavior.NotSerialized);


    public static void UnregisterListenerFromView<T>(this SimEvent<T> simEvent, ISimEventListener<T> listener)
        => simEvent.UnregisterListener_Internal(listener, SimEventInternal.SerializedBehavior.NotSerialized);
    public static void UnregisterListenerFromViewUnsafe<T>(this SimEvent<T> simEvent, ISimEventListenerUnsafe<T> listener) 
        => simEvent.UnregisterListenerUnsafe_Internal(listener, SimEventInternal.SerializedBehavior.NotSerialized);


    public static bool IsListenerRegisteredFromView(this SimEventInternal simEvent, ISimEventListenerBase listener) 
        => simEvent.IsRegistered_Internal(listener, SimEventInternal.SerializedBehavior.NotSerialized);
}
