using System;
using UnityEngine;
using UnityEngineX;

public class SimLogEventsDisplaySystem : GamePresentationSystem<SimLogEventsDisplaySystem>
{
    protected override void OnGamePresentationUpdate()
    {
        foreach (var item in PresentationEvents.LogEvents.SinceLastPresUpdate)
        {
            Log.Info(item.Value.Text);
        }
    }
}