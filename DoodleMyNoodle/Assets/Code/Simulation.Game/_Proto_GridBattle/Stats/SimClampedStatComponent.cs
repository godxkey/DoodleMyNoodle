using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimClampedStatComponent : SimStatComponent
{
    public int MaxValue = 10;
    public int MinValue = 0;

    public void SetClampedStatComponentValues(int MaxValue, int MinValue)
    {
        this.MaxValue = MaxValue;
        this.MinValue = MinValue;
    }

    public override int SetValue(int value)
    {
        int realValue = Mathf.Clamp(value, MinValue, MaxValue);
        return base.SetValue(realValue);
    }
}
