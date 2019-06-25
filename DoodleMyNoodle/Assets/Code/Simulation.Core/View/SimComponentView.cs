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
    private void OnValidate()
    {
        if (Application.isPlaying && attachedToSim)
        {
            UpdateSimFromSerializedData();
        }
    }
    public void UpdateSerializedDataFromSim()
    {
        ApplySimToSerializedData(simComponent);
    }
    public void UpdateSimFromSerializedData()
    {
        ApplySerializedDataToSim(simComponent);
    }
    public virtual void ApplySimToSerializedData(SimComponent comp) { }
#endif
    public virtual void ApplySerializedDataToSim(SimComponent comp) { }
}