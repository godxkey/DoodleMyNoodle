using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

public class ApplyMovementSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsVelocity velocity, in MoveSpeed moveSpeed, in CanMove canMove) =>
        {
            velocity.Linear.x = canMove ? moveSpeed.Value : 0;
        }).Schedule();
    }
}
