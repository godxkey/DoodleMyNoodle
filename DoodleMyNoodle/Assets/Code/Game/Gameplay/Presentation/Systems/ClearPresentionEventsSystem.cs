using System;
using UnityEngine;
using UnityEngineX;

public class ClearPresentionEventsSystem : GamePresentationSystem<ClearPresentionEventsSystem>
{
    private void LateUpdate()
    {
        // Dispose Persistent Native Array Entity
        foreach (var gameActionEvent in PresentationEvents.GameActionEvents.SinceLastPresUpdate)
        {
            if (gameActionEvent.GameActionContext.Targets.IsCreated)
                gameActionEvent.GameActionContext.Targets.Dispose();
        }

        PresentationEvents.Clear();
    }
}