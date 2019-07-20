using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationController : GameSystem<SimulationController>
{
    [SerializeField] SimBlueprintBank _blueprintBank;

    public override bool isSystemReady => true;

    public abstract void SubmitInput(SimInput input);

    public override void OnGameReady()
    {
        Simulation.Initialize(_blueprintBank);

        base.OnGameReady();
    }


    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        Simulation.Shutdown();
    }
}
