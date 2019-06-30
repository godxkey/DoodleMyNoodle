using System;
using UnityEngine;

[RequireComponent(typeof(SimEntityView))]
public abstract class SimComponentView : SimObjectView
{
    public SimComponent simComponent => attachedToSim ? (SimComponent)simObject : null;
    public abstract Type simComponentType { get; }

    protected abstract SimComponent CreateComponentFromSerializedData();

    internal SimComponent GetComponentFromSerializedData() => CreateComponentFromSerializedData();


#if UNITY_EDITOR
    public void UpdateSerializedDataFromSim()
    {
        ApplySimToSerializedData(simComponent);
    }
    public void UpdateSimFromSerializedData()
    {
        ApplySerializedDataToSim(simComponent);
    }
    protected virtual void ApplySimToSerializedData(SimComponent comp) { }
#endif
    protected virtual void ApplySerializedDataToSim(SimComponent comp) { }
}