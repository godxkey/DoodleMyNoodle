using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SimulationController : GameSystem<SimulationController>
{
    [SerializeField] SimBlueprintProviderPrefab _bpProviderPrefab;
    [SerializeField] SimBlueprintProviderSceneObject _bpProviderSceneObject;

    public override bool SystemReady => true;

    public abstract void SubmitInput(SimInput input);

    public override void OnGameReady()
    {
        Time.fixedDeltaTime = (float)SimulationConstants.TIME_STEP;


        SimulationCoreSettings settings = new SimulationCoreSettings();
        settings.BlueprintProviders = new List<ISimBlueprintProvider>()
        {
            _bpProviderPrefab,
            _bpProviderSceneObject
        };

        SimulationView.Initialize(settings);

        base.OnGameReady();
    }


    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (SimulationView.IsRunningOrReadyToRun)
            SimulationView.Dispose();
    }
}
