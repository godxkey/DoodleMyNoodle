using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimCommandMoveBall : SimCommand
{
    static readonly Fix64 speed = 6;

    public FixVector3 moveDirection;

    // fbessette:   This should be changed, we should not have an Execute method here
    //              The logic should be done on a component on the ball.
    public override void Execute()
    {
        Simulation.ForEveryEntityWithComponent<SimTransform>((ballTransform) =>
        {
            ballTransform.localPosition += moveDirection * Simulation.deltaTime * speed;
        });
    }
}
