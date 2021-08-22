﻿using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(AISystemGroup))]
[UpdateAfter(typeof(SpecificAISystemGroup))]
public class UpdateAIPatrolSystem : SimSystemBase
{
    private NativeList<int2> _path;

    protected override void OnCreate()
    {
        base.OnCreate();

        _path = new NativeList<int2>(Allocator.Persistent);
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _path.Dispose();
    }

    protected override void OnUpdate()
    {
        var tileWorld = CommonReads.GetTileWorld(Accessor);
        var turnCount = CommonReads.GetTurn(Accessor);
        var pathBuffer = _path;
        var time = Time.ElapsedTime;
        FixRandom random = World.Random();

        Entities
            .ForEach((ref AIPatrolData patrolData, ref AIDestination aiDestination, ref ReadyForNextTurn readyForNextTurn, ref AIActionCooldown actionCooldown,
                in AIPlaysThisFrameToken playsThisFrameToken, in AIState aiState, in ControlledEntity pawn) =>
            {
                if (!playsThisFrameToken || aiState.Value != AIStateEnum.Patrol)
                    return;

                // If no more AP => readyForNextTurn
                if (GetComponent<MoveEnergy>(pawn).Value <= 0 && GetComponent<ActionPoints>(pawn) == 0)
                {
                    readyForNextTurn.Value = true;
                    return;
                }

                bool hasPlayed = false;

                // we don't move more than once per turn
                if (turnCount != patrolData.LastPatrolTurn)
                {
                    var pawnPos = GetComponent<FixTranslation>(pawn);

                    // find random tile in 1 range
                    int2 agentTile = Helpers.GetTile(pawnPos);

                    int2? destination = null;
                    const int POTENTIAL_DESTINATIONS_LENGTH = 4;
                    unsafe
                    {
                        int2* potentialDestinations = stackalloc int2[POTENTIAL_DESTINATIONS_LENGTH]
                        {
                            agentTile + int2(0, 1),
                            agentTile + int2(0, -1),
                            agentTile + int2(1, 0),
                            agentTile + int2(-1, 0),
                        };

                        random.Shuffle<int2>(potentialDestinations, 4);

                        for (int i = 0; i < POTENTIAL_DESTINATIONS_LENGTH; i++)
                        {
                            var tile = potentialDestinations[i];
                            pathBuffer.Clear();
                            if (Pathfinding.FindNavigablePath(tileWorld, agentTile, tile, maxLength: 1, pathBuffer))
                            {
                                destination = tile;
                                break;
                            }
                        }
                    }

                    if (destination.HasValue)
                    {
                        hasPlayed = true;
                        aiDestination.Position = Helpers.GetTileCenter(destination.Value);
                        aiDestination.HasDestination = true;
                    }
                }

                if (hasPlayed)
                {
                    actionCooldown.NoActionUntilTime = time + 1;
                    patrolData.LastPatrolTurn = turnCount;
                }
                else
                {
                    aiDestination.HasDestination = false;
                    readyForNextTurn.Value = true;
                }

            }).Schedule();
    }
}