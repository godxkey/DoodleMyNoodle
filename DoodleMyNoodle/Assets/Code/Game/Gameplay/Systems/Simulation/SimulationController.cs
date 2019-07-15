using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationController : GameSystem<SimulationController>
{
    [SerializeField] SimBlueprintBank _blueprintBank;

    public override bool isSystemReady => true;

    public abstract void SubmitInput(SimInput input);


    protected Simulation simulation => _simulation;
    Simulation _simulation = new Simulation();

    public override void OnGameReady()
    {
        _simulation._blueprintBank = _blueprintBank;

        base.OnGameReady();

        ChangeSimWorld(new SimWorld());
    }

    void ChangeSimWorld(SimWorld world)
    {
        _simulation.ChangeWorld(world);
    }

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        _simulation.Dispose();
    }
}
