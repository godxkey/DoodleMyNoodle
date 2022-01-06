using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;

[UpdateInGroup(typeof(AISystemGroup))]
[UpdateAfter(typeof(SpecificAISystemGroup))]
public class UpdateAIPatrolSystem : SimSystemBase
{
    private PhysicsWorldSystem _physicsWorldSystem;
    private UpdateActorWorldSystem _actorWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _actorWorldSystem = World.GetOrCreateSystem<UpdateActorWorldSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
    }

    protected override void OnUpdate()
    {
        var tileWorld = CommonReads.GetTileWorld(Accessor);
        var turnCount = CommonReads.GetTurn(Accessor);
        Pathfinding.PathResult pathBuffer = new Pathfinding.PathResult(Allocator.TempJob);
        var time = Time.ElapsedTime;
        FixRandom random = World.Random();
        var physicsWorld = _physicsWorldSystem.PhysicsWorldSafe;
        var entity2PhysicsBody = _physicsWorldSystem.EntityToPhysicsBody;

        Entities
            .WithDisposeOnCompletion(pathBuffer)
            .ForEach((ref AIPatrolData patrolData, ref AIDestination aiDestination, ref ReadyForNextTurn readyForNextTurn, ref AIActionCooldown actionCooldown,
                in AIThinksThisFrameToken thinksThisFrame, in AIState aiState, in ControlledEntity pawn) =>
            {
                if (!thinksThisFrame || aiState.Value != AIStateEnum.Patrol)
                    return;

                // If no more AP => readyForNextTurn
                if (GetComponent<ActionPoints>(pawn).Value <= 0)
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

                    if (!entity2PhysicsBody.Lookup.TryGetValue(pawn, out int physicsBodyIndex))
                        physicsBodyIndex = -1;

                    var pathfindingContext = new Pathfinding.Context(tileWorld, physicsWorld, physicsBodyIndex, maxCost: Pathfinding.AgentCapabilities.Default.Walk1TileCost);
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
                            if (Pathfinding.FindNavigablePath(
                                context: pathfindingContext,
                                startPos: pawnPos,
                                goalPos: Helpers.GetTileCenter(tile),
                                reachDistance: 0,
                                result: ref pathBuffer))
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