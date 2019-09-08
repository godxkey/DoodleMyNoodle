using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// FOR THE SIM

public static class SimEventExtensions
{
    public static bool RegisterListener<T>(this SimEvent<T> simEvent, ISimEventListener<T> listener)
        => simEvent.RegisterListener_Internal(listener, SimEventInternal.SerializedBehavior.Serialized);
    public static bool RegisterListenerUnsafe<T>(this SimEvent<T> simEvent, ISimEventListenerUnsafe<T> listener)
        => simEvent.RegisterListenerUnsafe_Internal(listener, SimEventInternal.SerializedBehavior.Serialized);

    public static void UnregisterListener<T>(this SimEvent<T> simEvent, ISimEventListener<T> listener)
        => simEvent.UnregisterListener_Internal(listener, SimEventInternal.SerializedBehavior.Serialized);
    public static void UnregisterListenerUnsafe<T>(this SimEvent<T> simEvent, ISimEventListenerUnsafe<T> listener)
        => simEvent.UnregisterListenerUnsafe_Internal(listener, SimEventInternal.SerializedBehavior.Serialized);

    public static bool IsListenerRegistered(this SimEventInternal simEvent, ISimEventListenerBase listener)
        => simEvent.IsRegistered_Internal(listener, SimEventInternal.SerializedBehavior.Serialized);

    public static void Raise<T>(this SimEvent<T> simEvent, in T value)
        => simEvent.Raise_Internal(in value);
}
