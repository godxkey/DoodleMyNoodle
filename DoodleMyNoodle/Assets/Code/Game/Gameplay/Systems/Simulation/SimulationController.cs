using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationController : GameSystem<SimulationController>
{
    [SerializeField] SimBlueprintBank _blueprintBank;

    public override bool SystemReady => true;

    public abstract void SubmitInput(SimInput input);

    public override void OnGameReady()
    {
        SimulationView.Initialize(_blueprintBank);

        base.OnGameReady();
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationView.IsInitialized)
            SimulationView.Dispose();
    }
}
