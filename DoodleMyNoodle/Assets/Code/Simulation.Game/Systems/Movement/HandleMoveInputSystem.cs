using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using Unity.MathematicsX;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateBefore(typeof(StickToLadderSystem))]
public class HandleMoveInputSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, ref PhysicsVelocity velocity, ref MoveEnergy moveEnergy, in MoveInput moveInput, in MoveSpeed moveSpeed, in NavAgentFootingState footing) =>
        {
            if (footing.Value == NavAgentFooting.Ground || footing.Value == NavAgentFooting.Ladder)
            {
                fix2 move = moveInput.Value;

                if (footing.Value == NavAgentFooting.Ground)
                {
                    // NB: If we ever get slopped terrain, we might want to set the move perpendicular to the normal of the floor
                    move.y = 0;
                }

                move = clampLength(move, 0, 1);

                if (move.x != 0 || move.y != 0)
                {
                    moveEnergy.Value = max(0, moveEnergy.Value - deltaTime);
                }

                velocity.Linear = move * moveSpeed;
            }
        }).Run();

        Entities.ForEach((ref PhysicsVelocity velocity, in MoveInput moveInput, in MoveSpeed moveSpeed, in NavAgentFootingState footing, in AirControl airControl) =>
        {
            if (footing.Value == NavAgentFooting.AirControl)
            {
                velocity.Linear.x = movetowards(
                    current: velocity.Linear.x,
                    target: clamp(moveInput.Value.x, -1, 1) * moveSpeed,
                    maxDistanceDelta: airControl);
            }
        }).Run();
    }
}