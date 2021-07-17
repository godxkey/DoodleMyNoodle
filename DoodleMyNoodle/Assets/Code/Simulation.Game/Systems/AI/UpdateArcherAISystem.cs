using CCC.Fix2D;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

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

[UpdateInGroup(typeof(AISystemGroup))]
public class UpdateArcherAISystem : SimSystemBase
{
    public static fix DETECT_RANGE => 10;
    public static fix2 PAWN_EYES_OFFSET => fix2(0, fix(0.15));

    private EntityQuery _attackableGroup;

    // used in debug display, todo: make this better!
    public static NativeList<int2> _path;
    public static NativeList<int2> _shootingPositions;
    public static NativeList<Entity> _shootingTargets;

    private NativeList<Entity> _enemies;
    private ComponentDataFromEntity<FixTranslation> _positions;
    private TileWorld _tileWorld;
    private readonly int2[] _shootingDirections = new int2[]
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
        Profiler.BeginSample("Update Archer Mental State");

        _positions = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true);
        _tileWorld = CommonReads.GetTileWorld(Accessor);

        Entities.ForEach((Entity controller, ref ArcherAIData agentData, in Team controllerTeam, in ControlledEntity pawn) =>
        {
            UpdateMentalState(controller, controllerTeam, ref agentData, pawn);
        }).WithoutBurst().Run();

        Profiler.EndSample();

        int currentTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        Profiler.BeginSample("Try Act Archer");
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
        Profiler.EndSample();
    }

    private void UpdateMentalState(Entity controller, Team controllerTeam, ref ArcherAIData agentData, Entity agentPawn)
    {
        (Entity newAttackTarget, int2 newShootTile) = FindTargetAndShootPos(controllerTeam, agentPawn, agentData.AttackTarget);

        if (newAttackTarget != Entity.Null)
        {
            agentData.AttackTarget = newAttackTarget;
            agentData.ShootTile = newShootTile;

            if (Helpers.GetTile(GetComponent<FixTranslation>(agentPawn)).Equals(newShootTile))
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

    private (Entity attackTarget, int2 shootTile) FindTargetAndShootPos(Team controllerTeam, Entity agentPawn, Entity previousAttackTarget)
    {
        _enemies.Clear();
        CommonReads.PawnSenses.FindAllPawnsInSight(Accessor, _attackableGroup, agentPawn, excludeTeam: controllerTeam, _enemies);

        // If the archer has spotted an enemy once, it can track it through walls (compensates for lack of memory)
        if (HasComponent<FixTranslation>(previousAttackTarget))
        {
            Entity targetController = GetComponent<Controllable>(previousAttackTarget).CurrentController;
            if (TryGetComponent(targetController, out Team team) && team != controllerTeam)
            {
                _enemies.AddUnique(previousAttackTarget);
            }
        }

        if (_enemies.Length == 0)
        {
            return (Entity.Null, default);
        }

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

        foreach (Entity enemy in _enemies)
        {
            int2 enemyTile = Helpers.GetTile(_positions[enemy]);

            // search all shooting positions in direction
            for (int i = 0; i < _shootingDirections.Length; i++)
            {
                // Search specific direction
                for (int dist = 1; dist < shootingDistances[i]; dist++)
                {
                    int2 potentialTile = enemyTile + (_shootingDirections[i] * dist);

                    // Stop if tile is terrain or invalid
                    var tileEntity = _tileWorld.GetEntity(potentialTile);
                    var flags = _tileWorld.GetFlags(tileEntity);
                    if (flags.IsOutOfGrid || flags.IsTerrain)
                    {
                        break;
                    }

                    // Add potential shooting pos if we can stand on it
                    if (_tileWorld.CanStandOn(potentialTile))
                    {
                        _shootingPositions.Add(potentialTile);
                        _shootingTargets.Add(enemy);
                    }

                    // stop if actor blocking the way
                    bool anyActorBlockingTheWay = false;
                    //var tileActors = _tileActorBuffers[tileEntity];
                    //foreach (var actor in tileActors)
                    //{
                    //    if (actor != agentPawn && EntityManager.HasComponent<Health>(actor))
                    //    {
                    //        anyActorBlockingTheWay = true;
                    //        break;
                    //    }
                    //}
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

        fix2 pawnPos = GetComponent<FixTranslation>(agentPawn);
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

        if (Pathfinding.FindNavigablePath(Accessor, agentTile, agentData.ShootTile, Pathfinding.MAX_PATH_LENGTH, _path))
        {
            minimalMoveCost = Pathfinding.CalculateTotalCost(_path.Slice(0, min(2, _path.Length)));
        }

        // verify pawn has enough ap to move at least once
        if (TryGetComponent(pawn, out ActionPoints ap) && ap.Value < minimalMoveCost)
        {
            return false;
        }

        return CommonWrites.TryInputUseItem<GameActionMove>(Accessor, controller, agentData.ShootTile);
    }

    private bool Act_Attacking(Entity controller, Team team, ref ArcherAIData agentData, ControlledEntity pawn)
    {
        int2 enemyTile = Helpers.GetTile(GetComponent<FixTranslation>(agentData.AttackTarget));
        int2 shootingTile = agentData.ShootTile;

        int2 dir = sign(enemyTile - shootingTile);

        var gameActionArg = new GameActionParameterVector.Data((fix2)dir * 5); // hard coded speed at 3 for now

        return CommonWrites.TryInputUseItem<GameActionThrow>(Accessor, controller, gameActionArg);
    }
}