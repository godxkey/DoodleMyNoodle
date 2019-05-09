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
        instance.world.Tick_PreInput();

        foreach (SimInput input in tickData.inputs)
        {
            input.Execute(instance.world);
        }

        instance.world.Tick_PostInput();
    }

    public static readonly Fix64 deltaTime = (Fix64)0.2m; // 50 ticks per seconds
}
