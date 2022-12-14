using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

[UpdateInGroup(typeof(MovementSystemGroup))]
public partial class UpdateMovementSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PhysicsVelocity velocity, in MoveSpeed moveSpeed, in CanMove canMove, in OffsetFromTarget offsetFromTarget, in DesiredRangeFromTarget desiredDistance) =>
        {
            if (!canMove)
                return;

            fix targetVelX = CalculateTargetHorizontalVelocity(moveSpeed, offsetFromTarget, desiredDistance, deltaTime);
            velocity.Linear.x = fixMath.moveTowards(velocity.Linear.x, targetVelX, deltaTime * SimulationGameConstants.ActorAcceleration);
        }).Schedule();


        Entities
            .WithNone<PhysicsVelocity>()
            .ForEach((ref FixTranslation position, in MoveSpeed moveSpeed, in CanMove canMove, in OffsetFromTarget offsetFromTarget, in DesiredRangeFromTarget desiredDistance) =>
        {
            if (!canMove)
                return;

            position.Value.x += CalculateTargetHorizontalVelocity(moveSpeed, offsetFromTarget, desiredDistance, deltaTime) * deltaTime;
        }).Schedule();
    }

    private static fix CalculateTargetHorizontalVelocity(in MoveSpeed moveSpeed, in OffsetFromTarget offsetFromTarget, in DesiredRangeFromTarget desiredDistance, in fix deltaTime)
    {
        if (!desiredDistance.Value.IsValid)
        {
            return moveSpeed.Value;
        }

        fix destinationOffset = desiredDistance.Value.Clamp(offsetFromTarget.Distance) * sign(offsetFromTarget);
        fix delta = destinationOffset - offsetFromTarget;
        fix cappedMoveSpeed = min(abs(moveSpeed.Value), abs(delta) / deltaTime);
        return sign(delta) * cappedMoveSpeed;
    }
}
