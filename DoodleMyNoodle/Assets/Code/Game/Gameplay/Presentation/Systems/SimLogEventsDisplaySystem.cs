using System;
using UnityEngine;
using UnityEngineX;

public class SimLogEventsDisplaySystem : GamePresentationSystem<SimLogEventsDisplaySystem>
{
    public override void OnPostSimulationTick()
    {
        var evnts = PresentationEvents.LogEvents.SinceLastSimTick;
        if (evnts.AnyRemaining)
        {
            Log.Info("[SimLogEventsDisplaySystem.OnPostSimulationTick] Begin process logs");
            foreach (var item in evnts)
            {
                Log.Info($"[SimLogEventsDisplaySystem.OnPostSimulationTick] Log: {item.Value}");
            }
            Log.Info("[SimLogEventsDisplaySystem.OnPostSimulationTick] End process logs");
            Log.Info("end process logs");
        }
    }

    protected override void OnGamePresentationUpdate()
    {
        var evnts = PresentationEvents.LogEvents.SinceLastPresUpdate;
        if (evnts.AnyRemaining)
        {
            Log.Info("[SimLogEventsDisplaySystem.OnGamePresentationUpdate] Begin process logs");
            foreach (var item in evnts)
            {
                Log.Info($"[SimLogEventsDisplaySystem.OnGamePresentationUpdate] Log: {item.Value}");
            }
            Log.Info("[SimLogEventsDisplaySystem.OnGamePresentationUpdate] End process logs");
            Log.Info("end process logs");
        }
    }
}