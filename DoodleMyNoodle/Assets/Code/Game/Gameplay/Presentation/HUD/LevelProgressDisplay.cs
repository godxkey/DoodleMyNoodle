using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class LevelProgressDisplay : GamePresentationBehaviour
{
    [SerializeField] private Slider _slider;

    public override void PresentationUpdate()
    {
        if (SimWorld.TryGetSingleton<GameOverDestinationToReachSingleton>(out var destinationToReach))
        {
            fix progress = fixMath.clamp(Cache.GroupPosition.x / destinationToReach.XPosition, 0, 1);
            _slider.value = (float)progress;
        }
    }
}