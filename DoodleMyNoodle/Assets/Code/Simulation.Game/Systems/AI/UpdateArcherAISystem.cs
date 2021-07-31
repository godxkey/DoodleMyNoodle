using CCC.Fix2D;
using CCC.Fix2D.Debugging;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using static fixMath;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;
using Collider = CCC.Fix2D.Collider;

public struct ArcherAIData : IComponentData
{
    public Entity AttackTarget;
}

[UpdateInGroup(typeof(SpecificAISystemGroup))]
public class UpdateArcherAISystem : SimSystemBase
{
    public static fix DETECT_RANGE => 10;
    public static fix2 PAWN_EYES_OFFSET => fix2(0, fix(0.15));

    public struct GlobalCache
    {
        public ProfileMarkers ProfileMarkers;
        public PhysicsDebugStreamSystem.Context DebugContext;
        public PhysicsWorld PhysicsWorld;
        public TileWorld TileWorld;
        public ActorWorld ActorWorld;
        public NativeList<int> TargetBuffer;
        public NativeList<int2> ShootingPositions;
        public NativeList<Entity> ShootingTargets;
        public NativeQueue<Pathfinding.TileCostPair> FloodSearchBuffer;
        public FixRandom Random;
        public int TurnCount;
        public NativeList<int2> PathBuffer;
        public fix Time;
        public Team CurrentTurnTeam;
    }

    public struct AgentCache
    {
        public Entity Controller;
        public ActorWorld.Pawn PawnData;
        public Entity Pawn => PawnData.Entity;
        public Team Team => PawnData.Team;
        public fix2 PawnPosition => PawnData.Position;
        public int2 PawnTile => Helpers.GetTile(PawnPosition);
        public BlobAssetReference<Collider> ItemProjectileCollider;
    }

    private NativeList<int2> _path;
    private NativeList<int2> _shootingPositions;
    private NativeList<Entity> _shootingTargets;
    private NativeList<int> _targetBuffer;
    private NativeQueue<Pathfinding.TileCostPair> _floodSearchBuffer;
    private PhysicsDebugStreamSystem _debugDrawSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private UpdateActorWorldSystem _updateActorWorldSystem;
    private ProfileMarkers _profileMarkers;

    private static readonly SharedStatic<CollisionFilter> s_charactersAndTerrainFilter = SharedStatic<CollisionFilter>.GetOrCreate<UpdateArcherAISystem, CharactersAndTerrainFilterKey>();
    private class CharactersAndTerrainFilterKey { }

    public struct ProfileMarkers
    {
        public ProfilerMarker Test;
        public ProfilerMarker CheckIfCanShootNow;
        public ProfilerMarker FindPawnsInSight;
        public ProfilerMarker FloodSearch;
        public ProfilerMarker UpdateMentalState;
        public ProfilerMarker CanShootAnyTargetsFromTilePredicate;
    }

    private struct ShootRequest
    {
        public Entity Controller;
        public fix2 ShootVector;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        s_charactersAndTerrainFilter.Data = CollisionFilter.FromLayers(SimulationGameConstants.PHYSICS_LAYER_TERRAIN, SimulationGameConstants.PHYSICS_LAYER_CHARACTER);

        _profileMarkers = new ProfileMarkers()
        {
            Test = new ProfilerMarker("UpdateArcherAISystem.Test"),
            CheckIfCanShootNow = new ProfilerMarker("UpdateArcherAISystem.CheckIfCanShootNow"),
            FindPawnsInSight = new ProfilerMarker("UpdateArcherAISystem.FindPawnsInSight"),
            FloodSearch = new ProfilerMarker("UpdateArcherAISystem.FloodSearch"),
            UpdateMentalState = new ProfilerMarker("UpdateArcherAISystem.UpdateMentalState"),
            CanShootAnyTargetsFromTilePredicate = new ProfilerMarker("UpdateArcherAISystem.CanShootAnyTargetsFromTilePredicate"),
        };

        _debugDrawSystem = World.GetOrCreateSystem<PhysicsDebugStreamSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _updateActorWorldSystem = World.GetOrCreateSystem<UpdateActorWorldSystem>();
        _targetBuffer = new NativeList<int>(Allocator.Persistent);
        _shootingPositions = new NativeList<int2>(Allocator.Persistent);
        _shootingTargets = new NativeList<Entity>(Allocator.Persistent);
        _path = new NativeList<int2>(Allocator.Persistent);
        _floodSearchBuffer = new NativeQueue<Pathfinding.TileCostPair>(Allocator.Persistent);

        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _targetBuffer.Dispose();
        _shootingPositions.Dispose();
        _shootingTargets.Dispose();
        _path.Dispose();
        _floodSearchBuffer.Dispose();
    }

    protected override void OnUpdate()
    {
        _updateActorWorldSystem.ActorWorldDependency.Complete();

        GlobalCache globalCache = new GlobalCache()
        {
            DebugContext = _debugDrawSystem.GetContext(100),
            TileWorld = CommonReads.GetTileWorld(Accessor),
            PhysicsWorld = _physicsWorldSystem.PhysicsWorldSafe,
            ActorWorld = _updateActorWorldSystem.ActorWorld,
            TargetBuffer = _targetBuffer,
            ShootingPositions = _shootingPositions,
            ShootingTargets = _shootingTargets,
            FloodSearchBuffer = _floodSearchBuffer,
            ProfileMarkers = _profileMarkers,
            Random = World.Random(),
            TurnCount = CommonReads.GetTurn(Accessor),
            PathBuffer = _path,
            CurrentTurnTeam = CommonReads.GetTurnTeam(Accessor),
            Time = Time.ElapsedTime,
        };

        globalCache.ProfileMarkers.UpdateMentalState.Begin();

        NativeList<ShootRequest> shootRequests = new NativeList<ShootRequest>(Allocator.TempJob);

        Entities
            .ForEach((Entity controller, ref ArcherAIData agentData, ref AIDestination aiDestination, ref ReadyForNextTurn readyForNextTurn, ref AIActionCooldown actionCooldown,
                in AIPlaysThisFrameToken playsThisFrameToken, in ControlledEntity pawn) =>
            {
                if (!playsThisFrameToken)
                    return;

                AgentCache agentCache = new AgentCache()
                {
                    Controller = controller,
                    PawnData = globalCache.ActorWorld.GetPawn(globalCache.ActorWorld.GetPawnIndex(pawn)),
                };

                // Find throw item collider
                {
                    var pawnInventory = GetBuffer<InventoryItemReference>(pawn);
                    var throwActionId = GameActionBank.GetActionId<GameActionThrow>();
                    Entity throwItem = Entity.Null;
                    for (int i = 0; i < pawnInventory.Length; i++)
                    {
                        if (GetComponent<GameActionId>(pawnInventory[i].ItemEntity) == throwActionId)
                        {
                            throwItem = pawnInventory[i].ItemEntity;
                            break;
                        }
                    }

                    if (throwItem != Entity.Null)
                    {
                        var projectilePrefab = GetComponent<GameActionThrow.Settings>(throwItem).ProjectilePrefab;
                        if (HasComponent<PhysicsColliderBlob>(projectilePrefab))
                        {
                            agentCache.ItemProjectileCollider = GetComponent<PhysicsColliderBlob>(projectilePrefab).Collider;
                        }
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
                {
                    if (FindTargetAndShootPos(ref globalCache, ref agentCache, ref agentData, out Entity newAttackTarget, out fix2 newShootPosition))
                    {
                        SetComponent<AIState>(controller, AIStateEnum.Combat);
                        agentData.AttackTarget = newAttackTarget;

                        if (all(almostEqual(agentCache.PawnPosition, newShootPosition, epsilon: fix(0.1))))
                        {
                            aiDestination.HasDestination = false;
                            actionCooldown.NoActionUntilTime = globalCache.Time + 1;

                            fix2 dir = normalize(GetComponent<FixTranslation>(agentData.AttackTarget) - agentCache.PawnPosition);

                            shootRequests.Add(new ShootRequest()
                            {
                                Controller = controller,
                                ShootVector = dir * 5, // hard coded speed at 5 for now
                            });
                        }
                        else
                        {
                            aiDestination.HasDestination = true;
                            aiDestination.Position = newShootPosition;

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
                }
            }).Schedule();

        Dependency.Complete();

        foreach (var item in shootRequests)
        {
            var gameActionArg = new GameActionParameterVector.Data(item.ShootVector); // hard coded speed at 3 for now

            bool success = CommonWrites.TryInputUseItem<GameActionThrow>(Accessor, item.Controller, gameActionArg);

            if (!success)
            {
                SetComponent<ReadyForNextTurn>(item.Controller, true);
            }
        }

        shootRequests.Dispose();

        globalCache.ProfileMarkers.UpdateMentalState.End();
    }

    private unsafe struct CanShootAnyTargetsFromTilePredicate : ITilePredicate
    {
        public void* GlobalCachePtr;
        public void* AgentCachePtr;

        public NativeList<int> Targets;

        public Entity ResultTarget;

        public bool Evaluate(int2 tile)
        {
            var globalCache = UnsafeUtility.AsRef<GlobalCache>(GlobalCachePtr);
            var agentCache = UnsafeUtility.AsRef<AgentCache>(AgentCachePtr);

            using (globalCache.ProfileMarkers.CanShootAnyTargetsFromTilePredicate.Auto())
            {
                var tilePos = Helpers.GetTileCenter(tile);
                foreach (int pawnIndex in Targets)
                {
                    ref ActorWorld.Pawn enemyData = ref globalCache.ActorWorld.GetPawn(pawnIndex);

                    if (CanShoot(ref globalCache.PhysicsWorld, ref agentCache, tilePos, ref enemyData))
                    {
                        ResultTarget = enemyData.Entity;
                        return true;
                    }
                }

            }
            return false;
        }
    }

    private static bool CanShoot(ref PhysicsWorld physicsWorld, ref AgentCache agentCache, fix2 shootStartPos, ref ActorWorld.Pawn targetData)
    {
        fix2 targetPos = targetData.Position;
        fix2 agent2Target = targetPos - shootStartPos;
        fix distance = length(agent2Target);

        if (distance < 1)
        {
            return true;
        }

        if (agentCache.ItemProjectileCollider.IsCreated)
        {
            var dir = agent2Target / distance;
            fix projectileRadius = (fix)agentCache.ItemProjectileCollider.Value.Radius;
            ColliderCastInput input = new ColliderCastInput()
            {
                Collider = agentCache.ItemProjectileCollider,
                Start = (float2)shootStartPos,
                End = (float2)(targetPos - dir * (targetData.Radius + projectileRadius + (fix)0.05)),
                Ignore = new IgnoreHit(targetData.BodyIndex)
            };

            if (!physicsWorld.CastCollider(input))
            {
                return true;
            }
        }
        else
        {
            var dir = agent2Target / distance;
            RaycastInput input = new RaycastInput()
            {
                Start = (float2)(shootStartPos + dir * fix(0.7)),
                End = (float2)(targetPos - dir * fix(0.7)),
                Filter = s_charactersAndTerrainFilter.Data
            };

            if (!physicsWorld.CastRay(input))
            {
                return true;
            }
        }

        return false;
    }

    private static unsafe bool FindTargetAndShootPos(ref GlobalCache globalCache, ref AgentCache agentCache, ref ArcherAIData agentData, out Entity resultAttackTarget, out fix2 resultShootPosition)
    {
        resultAttackTarget = Entity.Null;
        resultShootPosition = default;

        Entity previousTarget = agentData.AttackTarget;

        // Find all enemy pawns in sight
        using (globalCache.ProfileMarkers.FindPawnsInSight.Auto())
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
        }

        // If the archer has spotted an enemy once, it can track it through walls (compensates for lack of memory)
        {
            int previousTargetPawnIndex = globalCache.ActorWorld.GetPawnIndex(previousTarget);
            if (previousTargetPawnIndex != -1)
            {
                ref ActorWorld.Pawn previousTargetPawn = ref globalCache.ActorWorld.GetPawn(previousTargetPawnIndex);
                if (previousTargetPawn.Team != agentCache.Team && !previousTargetPawn.Dead)
                {
                    globalCache.TargetBuffer.AddUnique(previousTargetPawnIndex);
                }
            }
        }

        // For all targets in sight, check if we can shoot it right now
        using (globalCache.ProfileMarkers.CheckIfCanShootNow.Auto())
        {
            for (int i = 0; i < globalCache.TargetBuffer.Length; i++)
            {
                ref ActorWorld.Pawn targetData = ref globalCache.ActorWorld.GetPawn(globalCache.TargetBuffer[i]);

                if (CanShoot(ref globalCache.PhysicsWorld, ref agentCache, agentCache.PawnPosition, ref targetData))
                {
                    resultAttackTarget = targetData.Entity;
                    resultShootPosition = agentCache.PawnPosition;
                    return true;
                }
            }
        }

        if (globalCache.TargetBuffer.Length == 0)
        {
            return false;
        }

        // Using a flood fill, search for any position from which we can shoot a target. This will naturally return the closest tile
        using (globalCache.ProfileMarkers.FloodSearch.Auto())
        {
            var predicate = new CanShootAnyTargetsFromTilePredicate()
            {
                Targets = globalCache.TargetBuffer,
                AgentCachePtr = UnsafeUtility.AddressOf(ref agentCache),
                GlobalCachePtr = UnsafeUtility.AddressOf(ref globalCache)
            };

            if (Pathfinding.NavigableFloodSearch(globalCache.TileWorld,
                                                 agentCache.PawnTile,
                                                 SimulationGameConstants.AISearchForPositionMaxCost,
                                                 globalCache.FloodSearchBuffer,
                                                 ref predicate,
                                                 out int2 position))
            {
                resultAttackTarget = predicate.ResultTarget;
                resultShootPosition = Helpers.GetTileCenter(position);
                return true;
            }
        }

        return false;
    }
}