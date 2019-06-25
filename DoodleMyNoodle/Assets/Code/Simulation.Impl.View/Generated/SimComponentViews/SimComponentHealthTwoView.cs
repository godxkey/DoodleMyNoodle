// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using UnityEngine;

public class SimComponentHealthTwoView : SimComponentHealthView
{
    [SerializeField] System.Single specialStuff;

    public override Type simComponentType => typeof(SimComponentHealthTwo);
    
    protected override SimComponent CreateComponentFromSerializedData()
    {
        SimComponentHealthTwo comp = new SimComponentHealthTwo();

        ApplySerializedDataToSim(comp);

        return comp;
    }

#if UNITY_EDITOR
    public override void ApplySimToSerializedData(SimComponent baseComp)
    {
        base.ApplySimToSerializedData(baseComp);
        SimComponentHealthTwo comp = (SimComponentHealthTwo)baseComp;
        specialStuff = comp.specialStuff;

    }
#endif
    public override void ApplySerializedDataToSim(SimComponent baseComp)
    {
        base.ApplySerializedDataToSim(baseComp);
        SimComponentHealthTwo comp = (SimComponentHealthTwo)baseComp;
        comp.specialStuff = specialStuff;

    }
}
