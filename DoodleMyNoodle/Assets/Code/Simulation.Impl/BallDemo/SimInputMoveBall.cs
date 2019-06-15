using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NetSerializable]
public class SimInputMoveBall : SimInput
{
    public FixVector2 moveDirection;

    // fbessette:   This should be changed, we should not have an Execute method here
    //              The logic should be done on a component on the ball.
    public override void Execute(SimWorld world)
    {
        SimComponentTransform2D ballTransform;
        world.FindEntityWithComponent(out ballTransform);
        if(ballTransform)
        {
            ballTransform.position += moveDirection * Simulation.deltaTime;
        }
    }
}
