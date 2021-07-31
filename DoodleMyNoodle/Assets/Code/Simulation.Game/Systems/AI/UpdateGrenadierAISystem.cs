using CCC.Fix2D;
using System;
using Unity.Collections;
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
    public GrenadierAIState State;
    public Entity AttackTarget;
    public fix NoActionUntilTime;
    public int LastPatrolTurn;
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
    }
}