using CCC.Fix2D;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct BruteAIData : IComponentData
{
    public Entity AttackTarget;
}

[UpdateInGroup(typeof(SpecificAISystemGroup))]
public class UpdateBruteAISystem : SimSystemBase
{
    public struct GlobalCache
    {
        public ProfileMarkers ProfileMarkers;
        public TileWorld TileWorld;
        public ActorWorld ActorWorld;
        public PhysicsWorld PhysicsWorld;
        public fix Time;
    }

    public struct GlobalBuffers
    {
        public NativeList<int> TargetBuffer;
        public Pathfinding.PathResult PathBuffer;

        public GlobalBuffers(Allocator allocator)
        {
            TargetBuffer = new NativeList<int>(allocator);
            PathBuffer = new Pathfinding.PathResult(allocator);
        }

        public void Dispose()
        {
            TargetBuffer.Dispose();
            PathBuffer.Dispose();
        }
    }


    public struct AgentCache
    {
        public fix AttackRange;
        public ActorWorld.Pawn PawnData;
        public Entity Pawn => PawnData.Entity;
        public Team Team => PawnData.Team;
        public fix2 PawnPosition => PawnData.Position;
        public int2 PawnTile => Helpers.GetTile(PawnPosition);
        public BlobAssetReference<Collider> ItemProjectileCollider;
    }

    private UpdateActorWorldSystem _updateActorWorldSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private ProfileMarkers _profileMarkers;

    public struct ProfileMarkers
    {
    }

    private struct AttackRequest
    {
        public Entity Controller;
        public fix2 AttackPos;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        _profileMarkers = new ProfileMarkers() { };
        _updateActorWorldSystem = World.GetOrCreateSystem<UpdateActorWorldSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnUpdate()
    {
        _updateActorWorldSystem.ActorWorldDependency.Complete();

        GlobalBuffers globalBuffers = new GlobalBuffers(Allocator.TempJob);
        GlobalCache globalCache = new GlobalCache()
        {
            ProfileMarkers = _profileMarkers,
            TileWorld = CommonReads.GetTileWorld(Accessor),
            ActorWorld = _updateActorWorldSystem.ActorWorld,
            PhysicsWorld = _physicsWorldSystem.PhysicsWorldSafe,
            Time = Time.ElapsedTime,
        };

        NativeList<AttackRequest> attackRequest = new NativeList<AttackRequest>(Allocator.TempJob);

        Entities
            .WithDisposeOnCompletion(globalBuffers)
            .ForEach((Entity controller, ref BruteAIData agentData, ref AIDestination aiDestination, ref ReadyForNextTurn readyForNextTurn, ref AIActionCooldown actionCooldown,
                in AIThinksThisFrameToken thinksThisFrame, in ControlledEntity pawn) =>
            {
                if (!thinksThisFrame)
                    return;

                AgentCache agentCache = new AgentCache()
                {
                    PawnData = globalCache.ActorWorld.GetPawn(globalCache.ActorWorld.GetPawnIndex(pawn)),
                };

                agentCache.AttackRange = fix(1.1);

                // Find attack item settings
                {
                    var pawnInventory = GetBuffer<InventoryItemReference>(pawn);
                    var meleeAttackActionId = GameActionBank.GetActionId<GameActionMeleeAttack>();
                    Entity meleeAttackItem = Entity.Null;
                    for (int i = 0; i < pawnInventory.Length; i++)
                    {
                        if (GetComponent<GameActionId>(pawnInventory[i].ItemEntity) == meleeAttackActionId)
                        {
                            meleeAttackItem = pawnInventory[i].ItemEntity;
                            break;
                        }
                    }

                    if (meleeAttackItem != Entity.Null)
                    {
                        var meleeAttackSettings = GetComponent<GameActionMeleeAttack.Settings>(meleeAttackItem);
                        agentCache.AttackRange = meleeAttackSettings.Range;
                    }
                }

                // Clear previous target
                {
                    bool clearPreviousTarget = true;
                    if (agentData.AttackTarget != Entity.Null)
                    {
                        if (HasComponent<FixTranslation>(agentData.AttackTarget))
                        {
                            fix2 previousTargetPos = GetComponent<FixTranslation>(agentData.AttackTarget);

                            // disabled now that AIs are omniscient
                            //if (distancesq(previousTargetPos, agentCache.PawnPosition) < SimulationGameConstants.AISightDistanceSq)
                            {
                                clearPreviousTarget = false;
                            }
                        }
                    }

                    if (clearPreviousTarget)
                    {
                        agentData.AttackTarget = Entity.Null;
                    }
                }

                // Find new most suitable target
                if (FindAttackTarget(ref globalCache, ref globalBuffers, ref agentCache, ref agentData, out Entity newAttackTarget, out fix2 newAttackPosition))
                {
                    SetComponent<AIState>(controller, AIStateEnum.Combat);
                    agentData.AttackTarget = newAttackTarget;

                    if (all(almostEqual(agentCache.PawnPosition, newAttackPosition, epsilon: fix(0.1))))
                    {
                        aiDestination.HasDestination = false;
                        actionCooldown.NoActionUntilTime = globalCache.Time + 1;

                        attackRequest.Add(new AttackRequest()
                        {
                            Controller = controller,
                            AttackPos = GetComponent<FixTranslation>(agentData.AttackTarget),
                        });
                    }
                    else
                    {
                        aiDestination.HasDestination = true;
                        aiDestination.Position = newAttackPosition;

                        // If no more AP => readyForNextTurn    (except if in AirControl, because we can move while having 0 ap)
                        if (GetComponent<ActionPoints>(pawn).Value <= 0 && GetComponent<NavAgentFootingState>(pawn).Value != NavAgentFooting.AirControl)
                            readyForNextTurn.Value = true;
                    }
                }
                else
                {
                    SetComponent<AIState>(controller, AIStateEnum.Patrol);
                    // patrol is handled by generic system
                }
            }).Schedule();

        Dependency.Complete();

        foreach (var item in attackRequest)
        {
            var gameActionArg = new GameActionParameterPosition.Data(item.AttackPos); // hard coded speed at 3 for now

            bool success = CommonWrites.TryInputUseItem<GameActionMeleeAttack>(Accessor, item.Controller, gameActionArg);

            if (!success)
            {
                SetComponent<ReadyForNextTurn>(item.Controller, true);
            }
        }

        attackRequest.Dispose();
    }

    private static bool FindAttackTarget(ref GlobalCache globalCache, ref GlobalBuffers globalBuffers, ref AgentCache agentCache, ref BruteAIData agentData, out Entity newAttackTarget, out fix2 newAttackPosition)
    {
        ActorWorld.PawnQueryInput input = new ActorWorld.PawnQueryInput()
        {
            ExcludeDead = true,
            ExcludeTeam = agentCache.Team,
            RequiresLineOfSight = false,
            EyeLocation = agentCache.PawnPosition + SimulationGameConstants.AIEyeOffset,
            //SightRange = SimulationGameConstants.AISightDistance,
            TileWorld = globalCache.TileWorld
        };

        globalBuffers.TargetBuffer.Clear();
        globalCache.ActorWorld.FindPawns(input, globalBuffers.TargetBuffer);

        // If the brute has spotted an enemy once, it can track it through walls (compensates for lack of memory)
        {
            int previousTargetPawnIndex = globalCache.ActorWorld.GetPawnIndex(agentData.AttackTarget);
            if (previousTargetPawnIndex != -1)
            {
                ref ActorWorld.Pawn previousTargetPawn = ref globalCache.ActorWorld.GetPawn(previousTargetPawnIndex);
                if (previousTargetPawn.Team != agentCache.Team && !previousTargetPawn.Dead)
                {
                    globalBuffers.TargetBuffer.AddUnique(previousTargetPawnIndex);
                }
            }
        }

        int closestEnemy = -1;
        fix closestDist = fix.MaxValue;
        fix2 closestAttackPosition = default;

        var pathfindingContext = new Pathfinding.Context(globalCache.TileWorld, globalCache.PhysicsWorld, agentCache.PawnData.BodyIndex, maxCost: Pathfinding.AgentCapabilities.DefaultMaxCost);

        fix attackRange = agentCache.AttackRange;
        foreach (int enemyIndex in globalBuffers.TargetBuffer)
        {
            ref ActorWorld.Pawn enemy = ref globalCache.ActorWorld.GetPawn(enemyIndex);
            fix2 enemyPos = enemy.Position;
            fix enemyRadius = enemy.Radius;

            // try find path to enemy
            if (!Pathfinding.FindNavigablePath(
                context: pathfindingContext,
                startPos: agentCache.PawnPosition,
                goalPos: enemyPos,
                reachDistance: attackRange + enemyRadius,
                result: ref globalBuffers.PathBuffer))
            {
                continue;
            }

            var path = globalBuffers.PathBuffer;

            // If distance is closer, record enemy
            fix dist = path.TotalCost;
            if (dist < closestDist)
            {
                closestEnemy = enemyIndex;
                closestDist = dist;
                closestAttackPosition = path.Segments.IsEmpty
                    ? agentCache.PawnPosition
                    : path.Segments.Last().EndPosition;
            }
        }

        // Change state!
        if (closestEnemy != -1)
        {
            ref var enemy = ref globalCache.ActorWorld.GetPawn(closestEnemy);
            newAttackTarget = enemy.Entity;
            newAttackPosition = closestAttackPosition;
            return true;
        }

        newAttackTarget = default;
        newAttackPosition = default;
        return false;
    }
}