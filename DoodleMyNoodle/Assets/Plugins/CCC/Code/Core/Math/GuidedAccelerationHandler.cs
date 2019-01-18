using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GuidedAccelerationHandler
{
    [SerializeField] float maxTargetAcceleration;

    public float CatchUpMultiplier { get; set; } = 1.1f;
    public float CurrentAcceleration { get; private set; }
    public float MaxTargetAcceleration { get { return maxTargetAcceleration; } set { maxTargetAcceleration = value; } }

    public GuidedAccelerationHandler()
    {
        CurrentAcceleration = 0;
    }
    public GuidedAccelerationHandler(float maxTargetAcceleration)
    {
        CurrentAcceleration = 0;
        this.maxTargetAcceleration = maxTargetAcceleration;
    }
    public void UpdateAcceleration(float targetPosition, float currentPosition, float currentSpeed, float deltaTime)
    {
        if (maxTargetAcceleration <= 0)
            return;

        var d = targetPosition - currentPosition;
        var a = maxTargetAcceleration * Mathf.Sign(d);
        var idealETA = Mathf.Sqrt(2 * d / a);

        float idealVelocity = 0;
        if (Mathf.Abs(idealETA) < deltaTime)
        {
            // Nous ne devons pas dépacer la cible en 1 frame
            idealVelocity = d / deltaTime;
        }
        else
        {
            // Nous visons la vitesse moyenne entre la frame courrante et la frame suivante
            idealVelocity = (2 * idealETA - deltaTime) * a / 2;
        }

        var wantedAcceleration = (idealVelocity - currentSpeed) / deltaTime;
        CurrentAcceleration = Mathf.Clamp(wantedAcceleration, -maxTargetAcceleration * CatchUpMultiplier, maxTargetAcceleration * CatchUpMultiplier);
    }
}
