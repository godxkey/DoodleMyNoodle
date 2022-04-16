using System;
using UnityEngineX;

public class LifespanVFX : GamePresentationBehaviour
{
    public void StartVFX(float Duration)
    {
        this.DelayedCall(Duration, () =>
        {
            Destroy(gameObject);
        });
    }
}