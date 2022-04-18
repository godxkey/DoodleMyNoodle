using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class ApplyMovementSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsVelocity velocity, in MoveSpeed moveSpeed, in CanMove canMove) =>
        {
            if (canMove)
            {
                velocity.Linear.x = moveSpeed.Value;
            }
        }).Schedule();

        fix deltaTime = Time.DeltaTime;

        Entities.WithNone<PhysicsVelocity>().ForEach((ref FixTranslation position, in MoveSpeed moveSpeed, in CanMove canMove) =>
        {
            position.Value.x += canMove ? moveSpeed.Value * deltaTime : 0;
        }).Schedule();
    }
}
