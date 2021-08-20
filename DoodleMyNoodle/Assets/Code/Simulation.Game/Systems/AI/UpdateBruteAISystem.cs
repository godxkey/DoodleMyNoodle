using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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
        public NativeList<int> TargetBuffer;
        public NativeList<int2> PathBuffer;
        public fix Time;
    }

    public struct AgentCache
    {
        public ActorWorld.Pawn PawnData;
        public Entity Pawn => PawnData.Entity;
        public Team Team => PawnData.Team;
        public fix2 PawnPosition => PawnData.Position;
        public int2 PawnTile => Helpers.GetTile(PawnPosition);
        public BlobAssetReference<Collider> ItemProjectileCollider;
    }

    private NativeList<int2> _path;
    private NativeList<int> _targetBuffer;
    private UpdateActorWorldSystem _updateActorWorldSystem;
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
        _targetBuffer = new NativeList<int>(Allocator.Persistent);
        _path = new NativeList<int2>(Allocator.Persistent);

        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _targetBuffer.Dispose();
        _path.Dispose();
    }

    protected override void OnUpdate()
    {
        _updateActorWorldSystem.ActorWorldDependency.Complete();

        GlobalCache globalCache = new GlobalCache()
        {
            TileWorld = CommonReads.GetTileWorld(Accessor),
            ActorWorld = _updateActorWorldSystem.ActorWorld,
            TargetBuffer = _targetBuffer,
            ProfileMarkers = _profileMarkers,
            PathBuffer = _path,
            Time = Time.ElapsedTime,
        };

        NativeList<AttackRequest> attackRequest = new NativeList<AttackRequest>(Allocator.TempJob);

        Entities
            .ForEach((Entity controller, ref BruteAIData agentData, ref AIDestination aiDestination, ref ReadyForNextTurn readyForNextTurn, ref AIActionCooldown actionCooldown,
                in AIPlaysThisFrameToken playsThisFrameToken, in ControlledEntity pawn) =>
            {
                if (!playsThisFrameToken)
                    return;

                AgentCache agentCache = new AgentCache()
                {
                    PawnData = globalCache.ActorWorld.GetPawn(globalCache.ActorWorld.GetPawnIndex(pawn)),
                };

                // Clear previous target
                {
                    bool clearPreviousTarget = true;
                    if (agentData.AttackTarget != Entity.Null)
                    {
                        if (HasComponent<FixTranslation>(agentData.AttackTarget))
                        {
                            fix2 previousTargetPos = GetComponent<FixTranslation>(agentData.AttackTarget);

                            if (distancesq(previousTargetPos, agentCache.PawnPosition) < SimulationGameConstants.AISightDistanceSq)
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
                if (FindAttackTarget(ref globalCache, ref agentCache, ref agentData, out Entity newAttackTarget, out fix2 newAttackPosition))
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

                        // If no more AP => readyForNextTurn
                        if (GetComponent<MoveEnergy>(pawn).Value <= 0 && GetComponent<ActionPoints>(pawn) == 0)
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

    private static bool FindAttackTarget(ref GlobalCache globalCache, ref AgentCache agentCache, ref BruteAIData agentData, out Entity newAttackTarget, out fix2 newAttackPosition)
    {
        ActorWorld.PawnSightQueryInput input = new ActorWorld.PawnSightQueryInput()
        {
            ExcludeDead = true,
            ExcludeTeam = agentCache.Team,
            EyeLocation = agentCache.PawnPosition + SimulationGameConstants.AIEyeOffset,
            SightRange = SimulationGameConstants.AISightDistance,
            TileWorld = globalCache.TileWorld
        };

        globalCache.TargetBuffer.Clear();
        globalCache.ActorWorld.FindAllPawnsInSight(input, globalCache.TargetBuffer);

        // If the brute has spotted an enemy once, it can track it through walls (compensates for lack of memory)
        {
            int previousTargetPawnIndex = globalCache.ActorWorld.GetPawnIndex(agentData.AttackTarget);
            if (previousTargetPawnIndex != -1)
            {
                ref ActorWorld.Pawn previousTargetPawn = ref globalCache.ActorWorld.GetPawn(previousTargetPawnIndex);
                if (previousTargetPawn.Team != agentCache.Team && !previousTargetPawn.Dead)
                {
                    globalCache.TargetBuffer.AddUnique(previousTargetPawnIndex);
                }
            }
        }

        int closestEnemy = -1;
        fix closestDist = fix.MaxValue;
        fix2 closestAttackPosition = default;

        foreach (int enemyIndex in globalCache.TargetBuffer)
        {
            ref var enemy = ref globalCache.ActorWorld.GetPawn(enemyIndex);

            int2 enemyTile = enemy.Tile;

            // try find path to enemy
            if (!Pathfinding.FindNavigablePath(globalCache.TileWorld, agentCache.PawnTile, enemyTile, maxLength: Pathfinding.MAX_PATH_LENGTH, globalCache.PathBuffer))
            {
                continue;
            }

            // If distance is closer, record enemy
            fix dist = Pathfinding.CalculateTotalCost(globalCache.PathBuffer.Slice());
            if (dist < closestDist)
            {
                closestEnemy = enemyIndex;
                closestDist = dist;
                closestAttackPosition = globalCache.PathBuffer.Length >= 2 ? Helpers.GetTileCenter(globalCache.PathBuffer[globalCache.PathBuffer.Length - 2]) : agentCache.PawnPosition;
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