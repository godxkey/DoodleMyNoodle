using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimPlayerStatsComponent : SimComponent
{
    // Make it so that here we have a list of simple stats container scriptable object
    public Stat<int> Healthpoints;

    //public int GetCurrentHealthpoints { get { return healthpoints.GetStruct.Value; } }

    //// TODO Add Event for healthchange

    //void Start()
    //{
    //    healthpoints = ;
    //    healthpoints.GetStruct.SetValue(healthpointsStartValue);
    //}

    //public bool IncreaseValue(int value)
    //{
    //    healthpoints.IncreaseValue(value);
    //    return healthpoints.GetStruct.IsDirty;
    //}
}
