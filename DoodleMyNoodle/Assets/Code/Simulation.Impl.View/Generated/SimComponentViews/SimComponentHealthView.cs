// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using UnityEngine;

public class SimComponentHealthView : SimComponentView
{
    [SerializeField] System.Single maxHealth;
    [SerializeField] System.Single minHealth;
    [SerializeField] System.Single currentHealth;

    public override Type simComponentType => typeof(SimComponentHealth);
    
    protected override SimComponent CreateComponentFromSerializedData()
    {
        SimComponentHealth comp = new SimComponentHealth();

        ApplySerializedDataToSim(comp);

        return comp;
    }

#if UNITY_EDITOR
    public override void ApplySimToSerializedData(SimComponent baseComp)
    {
        base.ApplySimToSerializedData(baseComp);
        SimComponentHealth comp = (SimComponentHealth)baseComp;
        maxHealth = comp.maxHealth;
        minHealth = comp.minHealth;
        currentHealth = comp.currentHealth;

    }
#endif
    public override void ApplySerializedDataToSim(SimComponent baseComp)
    {
        base.ApplySerializedDataToSim(baseComp);
        SimComponentHealth comp = (SimComponentHealth)baseComp;
        comp.maxHealth = maxHealth;
        comp.minHealth = minHealth;
        comp.currentHealth = currentHealth;

    }
}
