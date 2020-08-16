using System;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public class ApplyGravitySystem : SimJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        fix dt = Time.DeltaTime;
        fix graAcc = (fix)1.5;

        inputDeps = Entities.ForEach((ref Velocity velocity, in PhysicsGravity grav) =>
        {
            velocity.Value += fix3(0, graAcc * dt * grav.Scale, 0);
        }).Schedule(inputDeps);

        return inputDeps;
    }
}