using CCC.Fix2D;
using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct GrenadierAIData : IComponentData
{
    public Entity AttackTarget;
}

public enum GrenadierAIState
{
    Patrol,
    PositionForAttack,
    Attack
}

[UpdateInGroup(typeof(SpecificAISystemGroup))]
public class UpdateGrenadierAISystem : SimSystemBase
{
    public struct GlobalCache
    {
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
        public fix2 ItemProjectileGravity;
    }

    private NativeList<int2> _path;
    private NativeList<int2> _shootingPositions;
    private NativeList<Entity> _shootingTargets;
    private NativeList<int> _targetBuffer;
    private NativeQueue<Pathfinding.TileCostPair> _floodSearchBuffer;
    private PhysicsWorldSystem _physicsWorldSystem;
    private UpdateActorWorldSystem _updateActorWorldSystem;

    private struct ShootRequest
    {
        public Entity Controller;
        public fix2 ShootVector;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

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
            TileWorld = CommonReads.GetTileWorld(Accessor),
            PhysicsWorld = _physicsWorldSystem.PhysicsWorldSafe,
            ActorWorld = _updateActorWorldSystem.ActorWorld,
            TargetBuffer = _targetBuffer,
            ShootingPositions = _shootingPositions,
            ShootingTargets = _shootingTargets,
            FloodSearchBuffer = _floodSearchBuffer,
            Random = World.Random(),
            TurnCount = CommonReads.GetTurn(Accessor),
            PathBuffer = _path,
            CurrentTurnTeam = CommonReads.GetTurnTeam(Accessor),
            Time = Time.ElapsedTime,
        };

        NativeList<ShootRequest> shootRequests = new NativeList<ShootRequest>(Allocator.TempJob);

        Entities
            .ForEach((Entity controller, ref GrenadierAIData agentData, ref AIDestination aiDestination, ref ReadyForNextTurn readyForNextTurn, ref AIActionCooldown actionCooldown,
                in AIThinksThisFrameToken thinksThisFrame, in ControlledEntity pawn) =>
            {
                if (!thinksThisFrame)
                    return;

                AgentCache agentCache = new AgentCache()
                {
                    Controller = controller,
                    PawnData = globalCache.ActorWorld.GetPawn(globalCache.ActorWorld.GetPawnIndex(pawn)),
                    ItemProjectileGravity = globalCache.PhysicsWorld.StepSettings.GravityFix,
                };

                // Find throw item settings
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
                        var throwSettings = GetComponent<GameActionThrow.Settings>(throwItem);
                        var projectilePrefab = throwSettings.ProjectilePrefab;

                        if (HasComponent<PhysicsGravity>(projectilePrefab))
                        {
                            agentCache.ItemProjectileGravity *= GetComponent<PhysicsGravity>(projectilePrefab).ScaleFix;
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
                            actionCooldown.NoActionUntilTime = globalCache.Time + SimulationGameConstants.AIPauseDurationAfterShoot;

                            fix2 d = GetComponent<FixTranslation>(agentData.AttackTarget) - agentCache.PawnPosition;
                            fix2 dir = normalize(d);
                            fix2 shootVector;

                            // aim in front of target to account for bounces
                            d.x *= SimulationGameConstants.AIGrenadierShootDistanceRatio;

                            if (agentCache.ItemProjectileGravity == fix2.zero)
                            {
                                shootVector = SimulationGameConstants.AIShootSpeedIfNoGravity * dir;
                            }
                            else
                            {
                                shootVector = fixMath.Trajectory.SmallestLaunchVelocity(d.x, d.y, agentCache.ItemProjectileGravity);
                            }

                            Helpers.AI.FuzzifyThrow(ref shootVector, ref globalCache.Random, GetComponent<AIFuzzyThrowSettings>(controller));

                            shootRequests.Add(new ShootRequest()
                            {
                                Controller = controller,
                                ShootVector = shootVector,
                            });
                        }
                        else
                        {
                            aiDestination.HasDestination = true;
                            aiDestination.Position = newShootPosition;

                            // If no more AP => readyForNextTurn
                            if (GetComponent<ActionPoints>(pawn).Value <= 0)
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
            var shootVecArg = new GameActionParameterVector.Data(item.ShootVector);
            var originateFromCenter = new GameActionParameterBool.Data(true);

            bool success = CommonWrites.TryInputUseItem<GameActionThrow>(Accessor, item.Controller, shootVecArg, originateFromCenter);

            if (!success)
            {
                SetComponent<ReadyForNextTurn>(item.Controller, true);
            }
        }

        shootRequests.Dispose();
    }

    private unsafe struct CanShootAnyTargetsFromTilePredicate : ITilePredicate
    {
        public void* GlobalCachePtr;
        public void* AgentCachePtr;

        public NativeList<int> Targets;

        public Entity ResultTarget;

        public bool Evaluate(int2 tile)
        {
            return Evaluate(Helpers.GetTileCenter(tile));
        }

        public bool Evaluate(fix2 position)
        {
            var globalCache = UnsafeUtility.AsRef<GlobalCache>(GlobalCachePtr);
            var agentCache = UnsafeUtility.AsRef<AgentCache>(AgentCachePtr);

            foreach (int pawnIndex in Targets)
            {
                ref ActorWorld.Pawn enemyData = ref globalCache.ActorWorld.GetPawn(pawnIndex);

                if (CanShoot(ref globalCache.PhysicsWorld, ref agentCache, position, ref enemyData))
                {
                    ResultTarget = enemyData.Entity;
                    return true;
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
            return true;

        var dir = agent2Target / distance;
        RaycastInput input = new RaycastInput()
        {
            Start = (float2)(shootStartPos + dir * fix(0.7)),
            End = (float2)(targetPos - dir * fix(0.7)),
            Filter = SimulationGameConstants.Physics.CollideWithTerrainFilter.Data
        };

        if (!physicsWorld.CastRay(input))
        {
            return true;
        }

        return false;
    }

    private static unsafe bool FindTargetAndShootPos(ref GlobalCache globalCache, ref AgentCache agentCache, ref GrenadierAIData agentData, out Entity resultAttackTarget, out fix2 resultShootPosition)
    {
        resultAttackTarget = Entity.Null;
        resultShootPosition = default;

        Entity previousTarget = agentData.AttackTarget;

        // Find all enemy pawns in sight
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

        if (globalCache.TargetBuffer.Length == 0)
        {
            return false;
        }

        var predicate = new CanShootAnyTargetsFromTilePredicate()
        {
            Targets = globalCache.TargetBuffer,
            AgentCachePtr = UnsafeUtility.AddressOf(ref agentCache),
            GlobalCachePtr = UnsafeUtility.AddressOf(ref globalCache)
        };

        // Check if we can shoot it right now
        if (predicate.Evaluate(agentCache.PawnPosition))
        {
            resultAttackTarget = predicate.ResultTarget;
            resultShootPosition = agentCache.PawnPosition;
            return true;
        }

        // Using a flood fill, search for any position from which we can shoot a target. This will naturally return the closest tile
        var pathfindingContext = new Pathfinding.Context(
            globalCache.TileWorld, 
            globalCache.PhysicsWorld, 
            agentCache.PawnData.BodyIndex, 
            maxCost: SimulationGameConstants.AISearchForShootPositionMaxCost);

        if (Pathfinding.NavigableFloodSearch(pathfindingContext,
                                             agentCache.PawnTile,
                                             globalCache.FloodSearchBuffer,
                                             ref predicate,
                                             out int2 position))
        {
            resultAttackTarget = predicate.ResultTarget;
            resultShootPosition = Helpers.GetTileCenter(position);
            return true;
        }

        return false;
    }


    private static fix2 GetShootVector(fix2 agentPos, fix2 targetPos, fix2 gravity)
    {
        fix2 v = targetPos - agentPos;
        fix l = length(v);
        if (l < 1)
            return v;

        return Trajectory.SmallestLaunchVelocity(v.x, v.y, gravity);

        //mathX.Trajectory.Position
        //fix2 dir = v / l;
        //fix angle = angle2d(dir);
        //fix angleToUp = fixMath.degrees(fixMath.acos(fix2.Dot(fix2.up, ajustedDir)));
        //int signedAngle = dir.x > 0 ? 1 : -1;

        //// positive anti horaire
        //// négatif horaire
        //if (angleToUp < 46)
        //{
        //    ajustedDir = fixMath.rotate(ajustedDir, fixMath.radians(signedAngle * 20));
        //}
        //else if (angleToUp < 91)
        //{
        //    ajustedDir = fixMath.rotate(ajustedDir, fixMath.radians(signedAngle * 45));
        //}
        //else if (angleToUp < 181)
        //{
        //    ajustedDir = signedAngle * fix2.right;
        //}

        //var gameActionArg = new GameActionParameterVector.Data(new fix2((fix)ajustedDir.x, (fix)ajustedDir.y) * (fixMath.length(dir.x))); // hard coded speed

        //return CommonWrites.TryInputUseItem<GameActionThrow>(Accessor, controller, gameActionArg);
    }

    /*
    public static fix DETECT_RANGE => 10;
    public static fix2 PAWN_EYES_OFFSET => fix2(0, fix(0.15));

    private EntityQuery _attackableGroup;

    // used in debug display, todo: make this better!
    public static NativeList<int2> _path;
    public static NativeList<int2> _shootingPositions;
    public static NativeList<Entity> _shootingTargets;

    private NativeList<Entity> _enemies;
    private ComponentDataFromEntity<FixTranslation> _positions;

    protected override void OnCreate()
    {
        base.OnCreate();

        _enemies = new NativeList<Entity>(Allocator.Persistent);
        _shootingPositions = new NativeList<int2>(Allocator.Persistent);
        _shootingTargets = new NativeList<Entity>(Allocator.Persistent);
        _path = new NativeList<int2>(Allocator.Persistent);
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

        _enemies.Dispose();
        _shootingPositions.Dispose();
        _shootingTargets.Dispose();
        _path.Dispose();
        _attackableGroup.Dispose();
    }

    protected override void OnUpdate()
    {
        Profiler.BeginSample("Update Grenadier Mental State");

        _positions = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true);

        Entities.ForEach((Entity controller, ref GrenadierAIData agentData, in ControlledEntity pawn, in Team controllerTeam) =>
        {
            UpdateMentalState(controller, controllerTeam, ref agentData, pawn);
        })
            .WithoutBurst()
            .Run();

        Profiler.EndSample();

        int currentTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        Profiler.BeginSample("Try Act Grenadier");
        Entities
            .ForEach((Entity controller, ref GrenadierAIData agentData, ref ReadyForNextTurn readyForNextTurn, in ControlledEntity pawn, in Team team) =>
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
            })
            .WithoutBurst()
            .Run();
        Profiler.EndSample();
    }

    private void UpdateMentalState(Entity controller, Team controllerTeam, ref GrenadierAIData agentData, Entity agentPawn)
    {
        Entity newAttackTarget = FindTarget(controllerTeam, agentPawn, agentData.AttackTarget);

        if (newAttackTarget != Entity.Null)
        {
            agentData.AttackTarget = newAttackTarget;

            fix2 distanceToTarget = GetComponent<FixTranslation>(agentPawn).Value - GetComponent<FixTranslation>(newAttackTarget).Value;

            if (distanceToTarget.length <= 6) // HARDCODED RANGE TO SHOOT GRENADES
            {
                agentData.State = GrenadierAIState.Attack;
            }
            else
            {
                agentData.State = GrenadierAIState.PositionForAttack;
            }
        }
        else
        {
            agentData.State = GrenadierAIState.Patrol;
        }
    }

    private Entity FindTarget(Team controllerTeam, Entity agentPawn, Entity previousAttackTarget)
    {
        _enemies.Clear();
        CommonReads.PawnSenses.FindAllPawnsNearby(Accessor, _attackableGroup, agentPawn, excludeTeam: controllerTeam, _enemies);

        int2 agentTile = Helpers.GetTile(Accessor.GetComponent<FixTranslation>(agentPawn).Value);
        Entity closestEnemy = Entity.Null;
        int2 closestEnemyPos = new int2(0,0);
        foreach (Entity enemy in _enemies)
        {
            // Check Closest
            int2 enemyTile = Helpers.GetTile(_positions[enemy]);

            if ((closestEnemy == Entity.Null) || fixMath.lengthsq((fix2)(enemyTile - agentTile)) < fixMath.lengthsq((fix2)(closestEnemyPos - agentTile)))
            {
                closestEnemy = enemy;
                closestEnemyPos = enemyTile;
            }
        }

        return closestEnemy;
    }

    private bool IsReadyToAct(fix time, Entity controller, Team team, GrenadierAIData agentData, ControlledEntity pawn)
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

    private bool Act(Entity controller, Team team, ref GrenadierAIData agentData, ControlledEntity pawn)
    {
        switch (agentData.State)
        {
            case GrenadierAIState.Patrol:
                return Act_Patrol(controller, team, ref agentData, pawn);

            case GrenadierAIState.PositionForAttack:
                return Act_PositionForAttack(controller, team, ref agentData, pawn);

            case GrenadierAIState.Attack:
                return Act_Attacking(controller, team, ref agentData, pawn);
        }

        return false;
    }

    private bool Act_Patrol(Entity controller, Team team, ref GrenadierAIData agentData, ControlledEntity pawn)
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

    private bool Act_PositionForAttack(Entity controller, Team team, ref GrenadierAIData agentData, ControlledEntity pawn)
    {
        int2 agentTile = Helpers.GetTile(GetComponent<FixTranslation>(pawn));
        int2 enemyTile = Helpers.GetTile(GetComponent<FixTranslation>(agentData.AttackTarget));
        fix minimalMoveCost = default;

        if (Pathfinding.FindNavigablePath(Accessor, agentTile, enemyTile, Pathfinding.MAX_PATH_LENGTH, _path))
        {
            // TODO : faire que l'AI s'en va s'il est trop proche
            if (_path.Length <= 6)
            {
                return false;
            }

            _path.RemoveRangeWithBeginEnd(0, _path.Length - 6);

            minimalMoveCost = Pathfinding.CalculateTotalCost(_path.Slice(0, min(2, _path.Length)));
        }
        else
        {
            return false;
        }

        // verify pawn has enough ap to move at least once
        if (TryGetComponent(pawn, out ActionPoints ap) && ap.Value < minimalMoveCost)
        {
            return false;
        }

        return CommonWrites.TryInputUseItem<GameActionMove>(Accessor, controller, _path[0]);
    }

    private bool Act_Attacking(Entity controller, Team team, ref GrenadierAIData agentData, ControlledEntity pawn)
    {
        int2 pawnTile = Helpers.GetTile(GetComponent<FixTranslation>(pawn));
        int2 enemyTile = Helpers.GetTile(GetComponent<FixTranslation>(agentData.AttackTarget));

        int2 dir = enemyTile - pawnTile;
        fix2 ajustedDir = new fix2(dir.x, dir.y);
        ajustedDir.Normalize();
        fix angleToUp = fixMath.degrees(fixMath.acos(fix2.Dot(fix2.up, ajustedDir)));
        int signedAngle = dir.x > 0 ? 1 : -1;

        // positive anti horaire
        // négatif horaire
        if (angleToUp < 46)
        {
            ajustedDir =  fixMath.rotate(ajustedDir, fixMath.radians(signedAngle * 20));
        }
        else if (angleToUp < 91)
        {
            ajustedDir = fixMath.rotate(ajustedDir, fixMath.radians(signedAngle * 45));
        }
        else if (angleToUp < 181)
        {
            ajustedDir = signedAngle * fix2.right;
        }

        var gameActionArg = new GameActionParameterVector.Data(new fix2((fix)ajustedDir.x, (fix)ajustedDir.y) * (fixMath.length(dir.x))); // hard coded speed

        return CommonWrites.TryInputUseItem<GameActionThrow>(Accessor, controller, gameActionArg);
    }*/
}