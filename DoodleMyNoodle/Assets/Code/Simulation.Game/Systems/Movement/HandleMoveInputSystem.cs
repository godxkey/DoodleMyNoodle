using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateBefore(typeof(StickToLadderSystem))]
public class HandleMoveInputSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PhysicsVelocity velocity, in MoveInput moveInput, in MoveSpeed moveSpeed, in NavAgentFootingState footing) =>
        {
            fix2 move = moveInput.Value;

            if (footing.Value == NavAgentFooting.Ground || footing.Value == NavAgentFooting.None)
            {
                // NB: If we ever get slopped terrain, we might want to set the move perpendicular to the normal of the floor
                move.y = 0;
            }

            move = clampLength(move, 0, 1);

            if (footing.Value == NavAgentFooting.Ground || footing.Value == NavAgentFooting.Ladder)
            {
                velocity.Linear = move * moveSpeed;
            }
            else if (footing.Value == NavAgentFooting.AirControl)
            {
                velocity.Linear.x = movetowards(velocity.Linear.x, move.x * moveSpeed, (fix)0.2);
            }
        }).Run();
    }
}