using System;
using System.Collections.Generic;

public class SimEventComponent : SimComponent, ISimEventListener
{
    public List<SimEventInternal> EventsWeHaveRegisteredTo { get; set; }
    List<SimEventInternal> _localEvents;

    protected SimEvent<T> CreateLocalEvent<T>()
    {
        if (_localEvents == null)
            _localEvents = new List<SimEventInternal>();

        SimEvent<T> evt = new SimEvent<T>();
        _localEvents.Add(evt);
        return evt;
    }

    public override void OnSimDestroy()
    {
        base.OnSimDestroy();

        if(EventsWeHaveRegisteredTo != null)
        {
            for (int i = 0; i < EventsWeHaveRegisteredTo.Count; i++)
            {
                EventsWeHaveRegisteredTo[i].UnregisterListener_Internal(this, SimEventInternal.SerializedBehavior.Serialized);
            }
            EventsWeHaveRegisteredTo.Clear();
        }


        // dispose of our local events
        if(_localEvents != null)
        {
            for (int i = 0; i < _localEvents.Count; i++)
            {
                _localEvents[i].Dispose();
            }
        }
    }
}