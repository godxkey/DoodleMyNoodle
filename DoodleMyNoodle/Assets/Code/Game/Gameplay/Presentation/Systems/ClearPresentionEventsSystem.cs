using System;
using UnityEngine;
using UnityEngineX;

public class ClearPresentionEventsSystem : GamePresentationSystem<ClearPresentionEventsSystem>
{
    private void LateUpdate()
    {
        PresentationEvents.Clear();
    }
}