using CCC.Fix2D;
using System;
using UnityEngine;
using UnityEngineX;

public class DelayUntilDestroyedDisplay : BindedPresentationEntityComponent
{
    private fix previousTime = 0;

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorld.TryGetComponent(SimEntity, out DestroyAfterDelay destroyAfterDelay) && SimWorld.TryGetComponent(SimEntity, out FixTranslation fixTranslation))
        {
            if (destroyAfterDelay.Delay.Type == TimeValue.ValueType.Seconds)
            {
                fix deltaTime = SimWorld.Time.ElapsedTime - destroyAfterDelay.TrackedTime.Value;

                if ((deltaTime - previousTime) >= 1)
                {
                    previousTime = deltaTime;
                    int timePassedSeconds = fixMath.roundToInt(destroyAfterDelay.Delay.Value - deltaTime);
                    FloatingTextSystem.Instance.RequestText(fixTranslation.Value.ToUnityVec(), timePassedSeconds.ToString(), Color.white);
                }
            }
            else
            {
                if (destroyAfterDelay.TrackedTime.Value != previousTime)
                {
                    previousTime = destroyAfterDelay.TrackedTime.Value;
                    int timePassedSeconds = fixMath.roundToInt(destroyAfterDelay.Delay.Value - destroyAfterDelay.TrackedTime.Value);
                    FloatingTextSystem.Instance.RequestText(fixTranslation.Value.ToUnityVec(), timePassedSeconds.ToString(), Color.white);
                }
            }
        }
    }
}