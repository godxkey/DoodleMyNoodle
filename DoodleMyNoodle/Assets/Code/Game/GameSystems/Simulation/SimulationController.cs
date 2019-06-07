using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationController : GameSystem<SimulationController>
{
    public override bool isSystemReady => true;

    public abstract void SubmitInput(SimInput input);


    protected Simulation simulation => m_simulation;
    Simulation m_simulation = new Simulation();

    public override void OnGameReady()
    {
        base.OnGameReady();

        ChangeSimWorld(new SimWorld());
    }

    void ChangeSimWorld(SimWorld world)
    {
        m_simulation.ChangeWorld(world);
        if (m_simulation.m_world != null)
            m_simulation.m_world.blueprintBank = StupidSimBlueprintBank.instance; // temporary
    }

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        m_simulation.Dispose();
    }
}
