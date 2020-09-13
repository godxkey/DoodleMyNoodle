using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;

public struct ArcherAIData : IComponentData
{
    public ArcherAIState State;
    public Entity AttackTarget;
    public int2 ShootTile;
    public fix NoActionUntilTime;
    public int LastPatrolTurn;
}

public enum ArcherAIState
{
    Patrol,
    PositionForAttack,
    Attack
}

[UpdateAfter(typeof(RefillActionPointsSystem))] // TODO: use groups!
[UpdateAfter(typeof(DestroyAIControllersSystem))] // TODO: use groups!
[UpdateAfter(typeof(UpdateTileActorReferenceSystem))] // TODO: use groups!
[UpdateBefore(typeof(ExecutePawnControllerInputSystem))]
public class UpdateArcherAISystem : SimComponentSystem
{
    public static fix DETECT_RANGE => 10;
    public static fix2 PAWN_EYES_OFFSET => fix2(0, fix(0.15));

    private EntityQuery _attackableGroup;
    public static NativeList<int2> _path;
    public static NativeList<int2> _shootingPositions;
    public static NativeList<Entity> _shootingTargets;
    private NativeList<Entity> _enemies;

    protected override void OnCreate()
    {
        base.OnCreate();

        _enemies = new NativeList<Entity>(Allocator.Persistent);
        _shootingPositions = new NativeList<int2>(Allocator.Persistent);
        _shootingTargets = new NativeList<Entity>(Allocator.Persistent);

        _path = new NativeList<int2>(Allocator.Persistent);
        _attackableGroup = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Health>(),
            ComponentType.ReadOnly<ControllableTag>(),
            ComponentType.ReadOnly<FixTranslation>());

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
        Profiler.BeginSample("Update Archer Mental State");
        Entities.ForEach((Entity controller, ref Team controllerTeam, ref ArcherAIData agentData, ref ControlledEntity pawn) =>
        {
            UpdateMentalState(controller, controllerTeam, ref agentData, pawn);
        });
        Profiler.EndSample();

        int currentTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        Profiler.BeginSample("Try Act Archer");
        Entities
            .ForEach((Entity controller, ref Team team, ref ArcherAIData agentData, ref ControlledEntity pawn, ref ReadyForNextTurn readyForNextTurn) =>
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
            });
        Profiler.EndSample();
    }

    private void UpdateMentalState(in Entity controller, in Team controllerTeam, ref ArcherAIData agentData, in Entity agentPawn)
    {
        (Entity attackTarget, int2 shootPos) = FindTargetAndShootPos(controllerTeam, agentPawn);

        if (attackTarget != Entity.Null)
        {
            agentData.AttackTarget = attackTarget;
            agentData.ShootTile = shootPos;

            if (Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(agentPawn)).Equals(shootPos))
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

    private (Entity attackTarget, int2 shootPos) FindTargetAndShootPos(in Team controllerTeam, in Entity agentPawn)
    {
        _enemies.Clear();
        CommonReads.PawnSenses.FindAllPawnsInSight(Accessor, _attackableGroup, agentPawn, excludeTeam: controllerTeam, _enemies);

        if (_enemies.Length == 0)
        {
            return (Entity.Null, default);
        }

        int2[] shootingDirections = new int2[]
        {
            int2(-1, 0),
            int2(0, -1),
            int2(1, 0),
            int2(0, 1),
            int2(1, 1),
            int2(-1, 1),
            int2(1, -1),
            int2(-1, -1),
        };
        int d = floorToInt(CommonReads.PawnSenses.SIGHT_RANGE);
        int diagonalD = floorToInt(sin(fix.Pi / fix(4)) * CommonReads.PawnSenses.SIGHT_RANGE);
        int[] shootingDistances = new int[]
        {
            d,
            d,
            d,
            d,
            diagonalD,
            diagonalD,
            diagonalD,
            diagonalD,
        };

        _shootingPositions.Clear();
        _shootingTargets.Clear();
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);

        BufferFromEntity<TileActorReference> tileActorBuffers = GetBufferFromEntity<TileActorReference>(isReadOnly: true);
        ComponentDataFromEntity<FixTranslation> positions = GetComponentDataFromEntity<FixTranslation>();

        foreach (Entity enemy in _enemies)
        {
            int2 enemyTile = Helpers.GetTile(positions[enemy]);

            // search all shooting positions in direction
            for (int i = 0; i < shootingDirections.Length; i++)
            {
                // Search specific direction
                for (int dist = 1; dist < shootingDistances[i]; dist++)
                {
                    int2 potentialTile = enemyTile + (shootingDirections[i] * dist);

                    // Stop if tile is terrain or invalid
                    var tileEntity = tileWorld.GetEntity(potentialTile);
                    var flags = tileWorld.GetFlags(tileEntity);
                    if (flags.IsOutOfGrid || flags.IsTerrain)
                    {
                        break;
                    }

                    // Add potential shooting pos if we can stand on it
                    if (tileWorld.CanStandOn(potentialTile))
                    {
                        _shootingPositions.Add(potentialTile);
                        _shootingTargets.Add(enemy);
                    }

                    // stop is actor blocking the way
                    bool anyActorBlockingTheWay = false;
                    var tileActors = tileActorBuffers[tileEntity];
                    foreach (var actor in tileActors)
                    {
                        if (actor != agentPawn && EntityManager.HasComponent<Health>(actor))
                        {
                            anyActorBlockingTheWay = true;
                            break;
                        }
                    }
                    if (anyActorBlockingTheWay)
                    {
                        break;
                    }
                }
            }
        }

        if (_shootingPositions.Length == 0)
        {
            return (Entity.Null, default);
        }

        fix3 pawnPos = EntityManager.GetComponentData<FixTranslation>(agentPawn);
        int2 pawnTile = Helpers.GetTile(pawnPos);
        Pathfinding.FindCheapestNavigablePathFromMany(Accessor, pawnTile, _shootingPositions.Slice(), Pathfinding.MAX_PATH_LENGTH, _path);

        if (_path.Length == 0)
        {
            return (Entity.Null, default);
        }

        int2 attackPos = _path[_path.Length - 1];
        Entity attackTarget = _shootingTargets[_shootingPositions.IndexOf(attackPos)];

        return (attackTarget, attackPos);
    }

    private bool IsReadyToAct(in fix time, in Entity controller, in Team team, in ArcherAIData agentData, in ControlledEntity pawn)
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

    private bool Act(in Entity controller, in Team team, ref ArcherAIData agentData, in ControlledEntity pawn)
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

    private bool Act_Patrol(in Entity controller, in Team team, ref ArcherAIData agentData, in ControlledEntity pawn)
    {
        int turnCount = CommonReads.GetTurn(Accessor);

        // we don't move more than once per turn
        if (turnCount == agentData.LastPatrolTurn)
            return false;

        agentData.LastPatrolTurn = turnCount;

        // find random tile in 1 range
        int2 agentTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(pawn));

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

    private bool Act_PositionForAttack(in Entity controller, in Team team, ref ArcherAIData agentData, in ControlledEntity pawn)
    {
        int2 agentTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(pawn));
        fix minimalMoveCost = default;

        if (Pathfinding.FindNavigablePath(Accessor, agentTile, agentData.ShootTile, Pathfinding.MAX_PATH_LENGTH, _path))
        {
            minimalMoveCost = Pathfinding.CalculateTotalCost(_path.Slice(0, min(2, _path.Length)));
        }

        // verify pawn has enough ap to move at least once
        if (EntityManager.TryGetComponentData(pawn, out ActionPoints ap) && ap.Value < minimalMoveCost)
        {
            return false;
        }

        return CommonWrites.TryInputUseItem<GameActionMove>(Accessor, controller, agentData.ShootTile);
    }

    private bool Act_Attacking(in Entity controller, in Team team, ref ArcherAIData agentData, in ControlledEntity pawn)
    {
        int2 enemyTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(agentData.AttackTarget));
        int2 shootingTile = agentData.ShootTile;

        int2 dir = sign(enemyTile - shootingTile);

        return CommonWrites.TryInputUseItem<GameActionThrowProjectile>(Accessor, controller, shootingTile + dir);
    }
}