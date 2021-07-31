using CCC.Fix2D;
using CCC.Fix2D.Debugging;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;
using Collider = CCC.Fix2D.Collider;

public struct ArcherAIData : IComponentData
{
    public ArcherAIState State;
    public Entity AttackTarget;
    public fix2 ShootPosition;
    public fix NoActionUntilTime;
    public int LastPatrolTurn;
}

public enum ArcherAIState
{
    Patrol,
    PositionForAttack,
    Attack
}

[UpdateInGroup(typeof(AISystemGroup))]
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

    private EntityQuery _attackableGroup;

    // used in debug display, todo: make this better!
    public static NativeList<int2> _path;
    public static NativeList<int2> _shootingPositions;
    public static NativeList<Entity> _shootingTargets;

    private NativeList<int> _targetBuffer;
    private NativeQueue<Pathfinding.TileCostPair> _floodSearchBuffer;
    private PhysicsDebugStreamSystem _debugDrawSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private UpdateActorWorldSystem _updateActorWorldSystem;
    private ProfileMarkers _profileMarkers;


    public struct ProfileMarkers
    {
        public ProfilerMarker Test;
        public ProfilerMarker CheckIfCanShootNow;
        public ProfilerMarker FindPawnsInSight;
        public ProfilerMarker FloodSearch;
        public ProfilerMarker UpdateMentalState;
        public ProfilerMarker TryAct;
        public ProfilerMarker CanShootAnyTargetsFromTilePredicate;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        _profileMarkers = new ProfileMarkers()
        {
            Test = new ProfilerMarker("UpdateArcherAISystem.Test"),
            CheckIfCanShootNow = new ProfilerMarker("UpdateArcherAISystem.CheckIfCanShootNow"),
            FindPawnsInSight = new ProfilerMarker("UpdateArcherAISystem.FindPawnsInSight"),
            FloodSearch = new ProfilerMarker("UpdateArcherAISystem.FloodSearch"),
            UpdateMentalState = new ProfilerMarker("UpdateArcherAISystem.UpdateMentalState"),
            TryAct = new ProfilerMarker("UpdateArcherAISystem.TryAct"),
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
        _attackableGroup = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Health>(),
            ComponentType.ReadOnly<Controllable>(),
            ComponentType.ReadOnly<FixTranslation>(),
            ComponentType.Exclude<DeadTag>());

        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _targetBuffer.Dispose();
        _shootingPositions.Dispose();
        _shootingTargets.Dispose();
        _path.Dispose();
        _attackableGroup.Dispose();
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
            ProfileMarkers = _profileMarkers
        };

        globalCache.ProfileMarkers.UpdateMentalState.Begin();

        Entities
            .ForEach((Entity controller, ref ArcherAIData agentData, in ControlledEntity pawn) =>
        {
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
                    agentData.AttackTarget = newAttackTarget;
                    agentData.ShootPosition = newShootPosition;

                    if (all(almostEqual(agentCache.PawnPosition, newShootPosition, epsilon: fix(0.1))))
                    {
                        agentData.State = ArcherAIState.Attack;
                    }
                    else
                    {
                        agentData.State = ArcherAIState.PositionForAttack;
                    }
                }
                else
                {
                    agentData.State = ArcherAIState.Patrol;
                }
            }
        }).Schedule();

        globalCache.ProfileMarkers.UpdateMentalState.End();

        int currentTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        globalCache.ProfileMarkers.TryAct.Begin();
        Entities
            .ForEach((Entity controller, ref ReadyForNextTurn readyForNextTurn, ref ArcherAIData agentData, in ControlledEntity pawn, in Team team) =>
            {
                // Can the corresponding team play ?
                if (team.Value != currentTeam)
                {
                    return;
                }

                // have we already said 'ready for next turn' ?
                if (readyForNextTurn)
                {
                    return;
                }

                // wait until ready for action
                if (!IsReadyToAct(time, controller, team, agentData, pawn))
                {
                    return;
                }

                if (Act(controller, team, ref agentData, pawn))
                {
                    agentData.NoActionUntilTime = time + 1;
                }
                else
                {
                    readyForNextTurn.Value = true;
                }
            }).WithoutBurst().Run();
        globalCache.ProfileMarkers.TryAct.End();
    }

    private struct CanShootAnyTargetsFromTilePredicate : ITilePredicate
    {
        public GlobalCache GlobalCache;

        public NativeList<int> Targets;
        public AgentCache AgentCache;

        public Entity ResultTarget;

        public bool Evaluate(int2 tile)
        {
            using (GlobalCache.ProfileMarkers.CanShootAnyTargetsFromTilePredicate.Auto())
            {
                var tilePos = Helpers.GetTileCenter(tile);
                foreach (int pawnIndex in Targets)
                {
                    ref ActorWorld.Pawn enemyData = ref GlobalCache.ActorWorld.GetPawn(pawnIndex);

                    if (CanShoot(ref GlobalCache.PhysicsWorld, ref AgentCache, tilePos, ref enemyData))
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
                Filter = CollisionFilter.FromLayers(SimulationGameConstants.PHYSICS_LAYER_TERRAIN, SimulationGameConstants.PHYSICS_LAYER_CHARACTER)
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
                AgentCache = agentCache,
                GlobalCache = globalCache
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

    private bool IsReadyToAct(fix time, Entity controller, Team team, ArcherAIData agentData, ControlledEntity pawn)
    {
        if (time < agentData.NoActionUntilTime)
        {
            return false;
        }

        if (EntityManager.TryGetBuffer(pawn, out DynamicBuffer<PathPosition> path) && path.Length > 0)
        {
            return false;
        }

        return true;
    }

    private bool Act(Entity controller, Team team, ref ArcherAIData agentData, ControlledEntity pawn)
    {
        switch (agentData.State)
        {
            case ArcherAIState.Patrol:
                return Act_Patrol(controller, team, ref agentData, pawn);

            case ArcherAIState.PositionForAttack:
                return Act_PositionForAttack(controller, team, ref agentData, pawn);

            case ArcherAIState.Attack:
                return Act_Attacking(controller, team, ref agentData, pawn);
        }

        return false;
    }

    private bool Act_Patrol(Entity controller, Team team, ref ArcherAIData agentData, ControlledEntity pawn)
    {
        int turnCount = CommonReads.GetTurn(Accessor);

        // we don't move more than once per turn
        if (turnCount == agentData.LastPatrolTurn)
            return false;

        agentData.LastPatrolTurn = turnCount;

        // find random tile in 1 range
        int2 agentTile = Helpers.GetTile(GetComponent<FixTranslation>(pawn));

        int2[] potentialDestinations = new int2[]
        {
            agentTile + int2(0,1),
            agentTile + int2(0,-1),
            agentTile + int2(1,0),
            agentTile + int2(-1,0),
        };

        var random = World.Random();
        random.Shuffle(potentialDestinations);

        int2? destination = null;

        foreach (var tile in potentialDestinations)
        {
            if (Pathfinding.FindNavigablePath(Accessor, agentTile, tile, maxLength: 1, _path))
            {
                destination = tile;
                break;
            }
        }

        if (destination.HasValue)
        {
            return CommonWrites.TryInputUseItem<GameActionMove>(Accessor, controller, destination.Value);
        }

        return false;
    }

    private bool Act_PositionForAttack(Entity controller, Team team, ref ArcherAIData agentData, ControlledEntity pawn)
    {
        int2 agentTile = Helpers.GetTile(GetComponent<FixTranslation>(pawn));
        fix minimalMoveCost = default;

        if (Pathfinding.FindNavigablePath(Accessor, agentTile, Helpers.GetTile(agentData.ShootPosition), Pathfinding.MAX_PATH_LENGTH, _path))
        {
            minimalMoveCost = Pathfinding.CalculateTotalCost(_path.Slice(0, min(2, _path.Length)));
        }

        // verify pawn has enough ap to move at least once
        if (TryGetComponent(pawn, out ActionPoints ap) && ap.Value < minimalMoveCost)
        {
            return false;
        }

        return CommonWrites.TryInputUseItem<GameActionMove>(Accessor, controller, agentData.ShootPosition);
    }

    private bool Act_Attacking(Entity controller, Team team, ref ArcherAIData agentData, ControlledEntity pawn)
    {
        fix2 dir = normalize(GetComponent<FixTranslation>(agentData.AttackTarget) - agentData.ShootPosition);

        var gameActionArg = new GameActionParameterVector.Data(dir * 5); // hard coded speed at 3 for now

        return CommonWrites.TryInputUseItem<GameActionThrow>(Accessor, controller, gameActionArg);
    }
}