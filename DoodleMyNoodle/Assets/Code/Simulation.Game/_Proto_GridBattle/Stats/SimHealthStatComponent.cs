using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimHealthStatComponent : SimClampedStatComponent, ISimTickable
{
    public void OnSimTick()
    {
        //Debug.Log($"Health: {gameObject.name} : {Value}");
    }

    public override int SetValue(int value)
    {
        if(value <= 0)
        {
            Simulation.Destroy(SimEntity);
        }
        return base.SetValue(value);
    }
}
