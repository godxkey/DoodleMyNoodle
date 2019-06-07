using System;
using System.Collections;
using System.Collections.Generic;

public partial class Simulation : IDisposable
{
    public static Simulation instance;
    public Simulation() { instance = this; }
    public void Dispose()
    {
        m_world.Dispose();
        if (instance == this)
            instance = null;
    }

    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


    public SimWorld m_world;
    public uint m_tickId = 0;

    public void ChangeWorld(SimWorld world)
    {
        m_world?.Dispose();
        m_world = world;
    }

    public static SimWorld world => instance.m_world;
    public static uint tickId => instance.m_tickId;

    public static void Tick(SimTickData tickData)
    {
        world.Tick_PreInput();

        foreach (SimInput input in tickData.inputs)
        {
            input.Execute(world);
        }

        world.Tick_PostInput();
        instance.m_tickId++;
    }

    public static readonly Fix64 deltaTime = (Fix64)0.2m; // 50 ticks per seconds
}
