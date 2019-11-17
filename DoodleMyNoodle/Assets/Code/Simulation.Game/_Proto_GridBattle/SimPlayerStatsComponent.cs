using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimPlayerStatsComponent : SimComponent
{
    // Make it so that here we have a list of simple stats container scriptable object
    private ClampedStructValue<DirtyValue<int>> healthpoints;
    public int healthpointsStartValue = 0;
    public int GetCurrentHealthpoints { get { return healthpoints.GetStruct.Value; } }

    // TODO Add Event for healthchange

    void Start()
    {
        healthpoints.GetStruct.SetValue(healthpointsStartValue);
    }

    public bool IncreaseValue(int value)
    {
        healthpoints.IncreaseValue(value);
        return healthpoints.GetStruct.IsDirty;
    }
}
