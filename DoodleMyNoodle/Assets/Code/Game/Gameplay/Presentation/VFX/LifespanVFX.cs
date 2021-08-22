using System;
using UnityEngineX;

public class LifespanVFX : GamePresentationBehaviour
{
    protected override void OnGamePresentationUpdate() { }

    public void StartVFX(float Duration)
    {
        this.DelayedCall(Duration, () =>
        {
            Destroy(gameObject);
        });
    }
}