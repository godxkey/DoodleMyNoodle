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
        Entities.ForEach((ref PhysicsVelocity velocity, ref ActionPoints ap, in MoveInput moveInputComp, in MoveSpeed moveSpeed, in NavAgentFootingState footing) =>
        {

            if (footing.Value == NavAgentFooting.Ground || footing.Value == NavAgentFooting.Ladder)
            {
                fix2 moveInput = moveInputComp.Value;

                if (footing.Value == NavAgentFooting.Ground)
                {
                    // NB: If we ever get slopped terrain, we might want to set the move perpendicular to the normal of the floor
                    moveInput.y = 0;
                }

                moveInput = clampLength(moveInput, 0, 1);

                if (ap.Value == 0)
                {
                    moveInput *= 0;
                }

                // consume energy
                if (moveInput.x != 0 || moveInput.y != 0)
                {
                    ap.Value = max(0, ap.Value - deltaTime);
                }

                fix2 newVelocity = velocity.Linear;

                newVelocity.x = moveInput.x * moveSpeed;

                if (footing.Value == NavAgentFooting.Ladder)
                {
                    newVelocity.y = moveInput.y * moveSpeed;
                }

                velocity.Linear = newVelocity;
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