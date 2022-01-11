using CCC.Fix2D;
using System;
using UnityEngine;
using UnityEngineX;

public class LifetimeDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private int _maximumLifetimeForDisplay = 5;
    private int _previousDisplayedRemaining = 0;

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetComponent(SimEntity, out RemainingLifetime remainingLifetime) && SimWorld.TryGetComponent(SimEntity, out FixTranslation fixTranslation))
        {
            int displayedRemaining = fixMath.ceilToInt(remainingLifetime);
            if (displayedRemaining != _previousDisplayedRemaining && displayedRemaining <= _maximumLifetimeForDisplay)
            {
                _previousDisplayedRemaining = displayedRemaining;
                FloatingTextSystem.Instance.RequestText(fixTranslation.Value.ToUnityVec(), displayedRemaining.ToString(), Color.white);
            }
        }
    }
}