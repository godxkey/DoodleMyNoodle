using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using UnityEngineX;
using static fixMath;

[UpdateAfter(typeof(CreateAIPathToDestinationSystem))]
[UpdateInGroup(typeof(AISystemGroup))]
public class DirectAIAlongPathSystem : SimGameSystemBase
{
    private PhysicsWorldSystem _physicsWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnUpdate()
    {
        fix2 gravity = _physicsWorldSystem.PhysicsWorld.StepSettings.GravityFix;
        fix deltaTime = Time.DeltaTime;
        NativeList<Entity> jumpRequests = new NativeList<Entity>(Allocator.Temp);
        var tileWorld = CommonReads.GetTileWorld(Accessor);

        Entities
            .ForEach((Entity controller, DynamicBuffer<AIPathSegment> pathPositions, in ControlledEntity pawn) =>
            {
                if (pathPositions.IsEmpty)
                    return;

                if (pathPositions[0].Value.TransportToReach == Pathfinding.TransportMode.Jump)
                {
                    NavAgentFooting footing = GetComponent<NavAgentFootingState>(pawn);
                    if (footing == NavAgentFooting.Ground)
                    {
                        fix2 agentPos = GetComponent<FixTranslation>(pawn);
                        fix2 segmentPos = pathPositions[0].Value.EndPosition;

                        if (distance(agentPos.x, segmentPos.x) < 1 && distance(agentPos.y, segmentPos.y) > fix.Half)
                        {
                            jumpRequests.Add(controller);
                        }
                    }
                }

            }).Run();

        foreach (var aiEntity in jumpRequests)
        {
            CommonWrites.TryInputUseItem<GameActionBasicJump>(Accessor, aiEntity);
        }

        Entities
            .ForEach((DynamicBuffer<AIPathSegment> pathPositions, ref AIMoveInputLastFrame moveInputLastFrame, in ControlledEntity pawn) =>
            {
                if (pathPositions.IsEmpty && !moveInputLastFrame.WasAttemptingToMove)
                    return;

                if (!HasComponent<MoveInput>(pawn))
                    return;

                fix2 moveInput = new fix2(0, 0);

                if (pathPositions.Length > 0)
                {
                    var moveSpeed = GetComponent<MoveSpeed>(pawn);

                    if (moveSpeed.Value > 0)
                    {
                        fix2 pawnPos = GetComponent<FixTranslation>(pawn);
                        fix moveDist = moveSpeed.Value * deltaTime;

                        bool shouldMoveVertically = GetComponent<NavAgentFootingState>(pawn) == NavAgentFooting.Ladder
                                || (tileWorld.GetFlags(Helpers.GetTile(pawnPos)).IsLadder  && pathPositions[0].Value.TransportToReach != Pathfinding.TransportMode.Drop);

                        fix2 a = pawnPos;
                        fix2 b = pawnPos;
                        fix2 v = new fix2(0);
                        fix vLength = 0;
                        bool canRemovePositions = false;

                        // remove all path points that can be crossed with our 'moveDist'
                        while (moveDist > vLength && pathPositions.Length > 0)
                        {
                            moveDist -= vLength;
                            a = b;

                            if (canRemovePositions)
                            {
                                pathPositions.RemoveAt(0);
                            }
                            canRemovePositions = true;

                            if (pathPositions.Length > 0)
                            {
                                b = pathPositions[0].Value.EndPosition;

                                if (!shouldMoveVertically) // if pawn cannot move vertically, assume path is flat on horizontal axis
                                    b.y = a.y;
                            }

                            v = b - a;
                            vLength = length(v);
                        }

                        fix2 dir = (vLength == 0 ? fix2(0) : v / vLength);
                        fix2 targetPosition = a + (dir * min(moveDist, vLength));

                        fix2 targetVelocity = (targetPosition - pawnPos) / deltaTime;

                        moveInput = targetVelocity / moveSpeed;
                    }
                }

                SetComponent<MoveInput>(pawn, moveInput);

                moveInputLastFrame.WasAttemptingToMove = moveInput != fix2(0, 0);
            }).Schedule();
    }
}
