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
    protected override void OnCreate()
    {
        base.OnCreate();
        
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnUpdate()
    {
        fix deltaTime = Time.DeltaTime;

        // Influence velocity from moveInput when pawn is on ground or ladder
        Entities.ForEach((ref PhysicsVelocity velocity, ref ActionPoints ap, in MoveInput moveInput, in MoveSpeed moveSpeed, in NavAgentFootingState footing) =>
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

                if (ap.Value == 0)
                {
                    move *= 0;
                }

                // consume energy
                if (move.x != 0 || move.y != 0)
                {
                    ap.Value = max(0, ap.Value - deltaTime);
                }

                velocity.Linear = move * moveSpeed;
            }
        }).Run();

        // Influence velocity from moveInput when pawn is in air control
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


        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);

        // If pawn is in AirControl and he presses W or S and he's in front of ladder, attach to ladder!
        Entities.ForEach((ref NavAgentFootingState footing, in MoveInput moveInput, in FixTranslation position) =>
        {
            if (footing.Value == NavAgentFooting.AirControl && moveInput.Value.y != 0)
            {
                var tileFlags = tileWorld.GetFlags(Helpers.GetTile(position));

                if (tileFlags.IsLadder)
                {
                    footing.Value = NavAgentFooting.Ladder;
                }
            }
        }).Run();
    }
}