using System;
using System.Collections.Generic;
using UnityEngine;

public class SimEventComponent : SimComponent, ISimEventListener
{
    [System.Serializable]
    struct SerializedData
    {
        public List<SimEventInternal> EventsWeHaveRegisteredTo;
        public List<SimEventInternal> LocalEvents;
    }

    public List<SimEventInternal> EventsWeHaveRegisteredTo { get => _evtComponentData.EventsWeHaveRegisteredTo; set => _evtComponentData.EventsWeHaveRegisteredTo = value; }

    protected SimEvent<T> CreateLocalEvent<T>()
    {
        if (_evtComponentData.LocalEvents == null)
            _evtComponentData.LocalEvents = new List<SimEventInternal>();

        SimEvent<T> evt = new SimEvent<T>();
        _evtComponentData.LocalEvents.Add(evt);
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
        if(_evtComponentData.LocalEvents != null)
        {
            for (int i = 0; i < _evtComponentData.LocalEvents.Count; i++)
            {
                _evtComponentData.LocalEvents[i].Dispose();
            }
            _evtComponentData.LocalEvents = null;
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [CCC.InspectorDisplay.AlwaysExpand]
    [HideInInspector]
    SerializedData _evtComponentData = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_evtComponentData);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _evtComponentData = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}