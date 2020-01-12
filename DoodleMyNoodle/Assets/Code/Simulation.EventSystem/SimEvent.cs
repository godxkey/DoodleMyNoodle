using System;
using System.Collections.Generic;

[Serializable]
public class SimEventInternal : IDisposable
{
    internal enum SerializedBehavior : byte
    {
        /// <summary>
        /// The listener is saved in the simulation. Restarting/Reloading the game will rebuild the link
        /// </summary>
        Serialized,

        /// <summary>
        /// The listener is not saved in the simulation. Restarting/Reloading the game will NOT rebuild the link
        /// </summary>
        NotSerialized
    }
    internal enum SafeBehavior : byte
    {
        /// <summary>
        /// The listener has a list of EventsWeHaveRegisteredTo which is used to keep track of event he HAS to unregister before dying
        /// </summary>
        Safe,

        /// <summary>
        /// The listener has to remember to manually unregister
        /// </summary>
        Unsafe
    }

    public List<ISimEventListenerBase> _listenersSerialized; // serialized

    [NonSerialized]
    List<ISimEventListenerBase> _listenersNotSerialized; // NOT serialized

    internal bool RegisterListener_Internal(ISimEventListener listener, SerializedBehavior serializedBehavior)
    {
        return RegisterListener_Internal(ref GetList(serializedBehavior), listener, SafeBehavior.Safe);
    }
    internal bool RegisterListenerUnsafe_Internal(ISimEventListenerUnsafe listener, SerializedBehavior serializedBehavior)
    {
        return RegisterListener_Internal(ref GetList(serializedBehavior), listener, SafeBehavior.Unsafe);
    }

    internal bool IsRegistered_Internal(ISimEventListenerBase listener, SerializedBehavior serializedBehavior)
    {
        return IsRegistered_Internal(GetList(serializedBehavior), listener);
    }

    internal bool UnregisterListener_Internal(ISimEventListener listener, SerializedBehavior serializedBehavior)
    {
        return UnregisterListener_Internal(GetList(serializedBehavior), listener, SafeBehavior.Safe);
    }
    internal bool UnregisterListenerUnsafe_Internal(ISimEventListenerUnsafe listener, SerializedBehavior serializedBehavior)
    {
        return UnregisterListener_Internal(GetList(serializedBehavior), listener, SafeBehavior.Unsafe);
    }

    internal void Raise_Internal<T>(in T value)
    {
        Raise_Internal(_listenersSerialized, value);
        Raise_Internal(_listenersNotSerialized, value);
    }

    public void Dispose()
    {
        UnregisterAllFromList(_listenersSerialized);
        UnregisterAllFromList(_listenersNotSerialized);
    }

    ref List<ISimEventListenerBase> GetList(SerializedBehavior serializedBehavior)
    {
        if (serializedBehavior == SerializedBehavior.Serialized)
        {
            return ref _listenersSerialized;
        }
        else
        {
            return ref _listenersNotSerialized;
        }
    }

    bool RegisterListener_Internal(ref List<ISimEventListenerBase> listeners, ISimEventListenerBase listener, SafeBehavior safeBehavior)
    {
        if (listener == null)
        {
            DebugService.LogError($"{nameof(SimEventInternal)}.RegisterListener: Trying to register a null listener." +
                $" Are you inheriting from {nameof(ISimEventListener)}?");
            return false;
        }

        if (IsRegistered_Internal(listeners, listener))
        {
            DebugService.LogError($"{nameof(SimEventInternal)}.RegisterListener: Trying to register the same listener twice.");
            return false;
        }

        // store a reference to listener
        if (listeners == null)
            listeners = new List<ISimEventListenerBase>();
        listeners.Add(listener);


        if (safeBehavior == SafeBehavior.Safe)
        {
            // give 'this' reference to listener
            ISimEventListener safeListener = (ISimEventListener)listener;
            if (safeListener.EventsWeHaveRegisteredTo == null)
                safeListener.EventsWeHaveRegisteredTo = new List<SimEventInternal>();
            safeListener.EventsWeHaveRegisteredTo.Add(this);
        }

        return true;
    }

    bool IsRegistered_Internal(List<ISimEventListenerBase> listeners, ISimEventListenerBase listener)
    {
        return listeners != null && listeners.Contains(listener);
    }

    bool UnregisterListener_Internal(List<ISimEventListenerBase> listeners, ISimEventListenerBase listener, SafeBehavior safeBehavior)
    {
        if (listeners != null)
        {
            int index = listeners.IndexOf(listener);

            if (index != -1)
            {
                UnregisterListenerAt(listeners, index, safeBehavior);
                return true;
            }
        }

        DebugService.LogError($"{nameof(SimEventInternal)}.UnregisterListener: Trying to unregister a listener that was never registered.");
        return false;
    }

    void UnregisterListenerAt(List<ISimEventListenerBase> listeners, int i, SafeBehavior safeBehavior)
    {
        if (safeBehavior == SafeBehavior.Safe)
        {
            ISimEventListener safeListener = (ISimEventListener)listeners[i];
            safeListener.EventsWeHaveRegisteredTo?.RemoveWithLastSwap(this);
        }
        listeners.RemoveWithLastSwapAt(i);
    }

    void UnregisterAllFromList(List<ISimEventListenerBase> listeners)
    {
        if (listeners != null)
        {
            while (listeners.Count > 0)
            {
                UnregisterListenerAt(
                    listeners,              // list
                    listeners.Count - 1,    // index
                    (listeners[listeners.Count - 1] is ISimEventListener) ? SafeBehavior.Safe : SafeBehavior.Unsafe); // safe ?
            }
        }
    }

    void Raise_Internal<T>(List<ISimEventListenerBase> listeners, in T value)
    {
        if (listeners == null)
            return;

        for (int i = 0; i < listeners.Count; i++)
        {
#if DEBUG_BUILD
            try
            {
#endif
                ((ISimEventListenerBase<T>)listeners[i]).OnEventRaised(value);
#if DEBUG_BUILD
            }
            catch (Exception e)
            {
                DebugService.LogError(e.Message + " - stack:\n " + e.StackTrace);
            }
#endif
        }
    }

}

[Serializable]
public class SimEvent<T> : SimEventInternal
{
    // Using this SimEvent<T> instead of SimEvent adds an extra validation at compile time
    // e.g.:    SimEvent<OnDeathEvent> event;
    //          event.RegisterListener(x);  !! ONLY COMPILES IF X INHERITS FROM ISimEventListener<OnDeathEvent> !!
}