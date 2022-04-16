using System;
using UnityEngine;
using UnityEngineX;

public class SimLogEventsDisplaySystem : GamePresentationSystem<SimLogEventsDisplaySystem>
{
    public override void PresentationUpdate()
    {
        foreach (var item in PresentationEvents.LogEvents.SinceLastPresUpdate)
        {
            Log.Info(item.Text);
        }
    }
}