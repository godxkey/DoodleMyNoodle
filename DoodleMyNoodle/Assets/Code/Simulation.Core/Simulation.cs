using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Simulation : IDisposable
{
    public static Simulation instance;
    public Simulation() { instance = this; }
    public void Dispose()
    {
        if (instance == this)
            instance = null;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public SimWorld world;


    public static void Tick(SimTickData tickData)
    {
        foreach (SimInput input in tickData.inputs)
        {
            input.Execute(instance.world);
        }
    }
}
