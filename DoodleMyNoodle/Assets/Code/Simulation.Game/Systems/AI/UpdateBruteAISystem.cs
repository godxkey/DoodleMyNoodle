using CCC.Fix2D;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.MathematicsX;
using UnityEngine.Profiling;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct BruteAIData : IComponentData
{
    public BruteAIState State;
    public Entity AttackTarget;
    public fix NoActionUntilTime;
    public int LastPatrolTurn;
}

public enum BruteAIState
{
    Patrol,
    PositionForAttack,
    Attack
}

[UpdateInGroup(typeof(AISystemGroup))]
public class UpdateBruteAISystem : SimComponentSystem
{
    private readonly fix AGGRO_WALK_RANGE = 10;

    private EntityQuery _attackableGroup;
    private NativeList<int2> _path;
    private ComponentDataFromEntity<FixTranslation> _positions;
    private NativeArray<Entity> _attackableEntities;

    protected override void OnCreate()
    {
        base.OnCreate();

        _path = new NativeList<int2>(Allocator.Persistent);
        _attackableGroup = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Health>(),
            ComponentType.ReadOnly<Controllable>(),
            ComponentType.ReadOnly<FixTranslation>(),
            ComponentType.Exclude<DeadTag>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _path.Dispose();
        _attackableGroup.Dispose();
    }

    protected override void OnUpdate()
    {
        Profiler.BeginSample("Update Brute Mental State");
        _positions = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true);
        _attackableEntities = _attackableGroup.ToEntityArray(Allocator.TempJob);

        Entities.ForEach((Entity controller, ref Team controllerTeam, ref BruteAIData agentData, ref ControlledEntity pawn) =>
        {
            UpdateMentalState(controller, controllerTeam, ref agentData, pawn);
        });
        Profiler.EndSample();

        int currentTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        Profiler.BeginSample("Try Act Brute");
        Entities
            .ForEach((Entity controller, ref Team team, ref BruteAIData agentData, ref ControlledEntity pawn, ref ReadyForNextTurn readyForNextTurn) =>
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


        _attackableEntities.Dispose();
    }

    private void UpdateMentalState(Entity controller, Team controllerTeam, ref BruteAIData agentData, Entity agentPawn)
    {
        switch (agentData.State)
        {
            case BruteAIState.Patrol:
                UpdateMentalState_Patrol(controller, controllerTeam, ref agentData, agentPawn);
                break;
            case BruteAIState.PositionForAttack:
            case BruteAIState.Attack:
            {
                // if target does not exit anymore, back to patrol
                if (!EntityManager.Exists(agentData.AttackTarget) ||
                    !EntityManager.TryGetComponentData(agentData.AttackTarget, out FixTranslation enemyPos))
                {
                    agentData.State = BruteAIState.Patrol;
                    return;
                }

                // if target's controller is not our enemy anymore, back to patrol
                var targetController = EntityManager.GetComponentData<Controllable>(agentData.AttackTarget).CurrentController;
                if (!EntityManager.Exists(targetController) ||
                    !EntityManager.TryGetComponentData(targetController, out Team team) ||
                    team == controllerTeam)
                {
                    agentData.State = BruteAIState.Patrol;
                    return;
                }

                int2 enemyTile = Helpers.GetTile(enemyPos);
                int2 agentTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(agentPawn));
                if (mathX.lengthmanhattan(enemyTile - agentTile) == 1)
                {
                    agentData.State = BruteAIState.Attack;
                }
                else
                {
                    agentData.State = BruteAIState.PositionForAttack;
                }
                break;
            }
        }
    }

    private void UpdateMentalState_Patrol(Entity controller, Team controllerTeam, ref BruteAIData agentData, Entity agentPawn)
    {
        // TDLR: Search for enemy within range (no line of sight required)
        fix2 pawnPos = EntityManager.GetComponentData<FixTranslation>(agentPawn);
        int2 agentTile = Helpers.GetTile(pawnPos);

        Entity closest = Entity.Null;
        fix closestDist = fix.MaxValue;
        int2 closestTile = default;

        foreach (var enemy in _attackableEntities)
        {
            // Find enemy controller in other teams
            Entity enemyController = CommonReads.GetPawnController(Accessor, enemy);

            if (enemyController == Entity.Null)
                continue;

            if (!EntityManager.TryGetComponentData(enemyController, out Team enemyTeam))
            {
                continue;
            }

            if (enemyTeam == controllerTeam)
                continue;

            int2 enemyTile = Helpers.GetTile(_positions[enemy].Value);
            // try find path to enemy
            if (!Pathfinding.FindNavigablePath(Accessor, agentTile, enemyTile, maxLength: AGGRO_WALK_RANGE, _path))
            {
                continue;
            }

            // If distance is closer, record enemy
            fix dist = Pathfinding.CalculateTotalCost(_path.Slice());
            if (dist <= AGGRO_WALK_RANGE && dist < closestDist)
            {
                closest = enemy;
                closestDist = dist;
                closestTile = enemyTile;
            }
        }

        // Change state!
        if (closest != Entity.Null)
        {
            agentData.AttackTarget = closest;

            if (mathX.lengthmanhattan(closestTile - agentTile) == 1)
            {
                agentData.State = BruteAIState.Attack;
            }
            else
            {
                agentData.State = BruteAIState.PositionForAttack;
            }
        }
    }

    private bool IsReadyToAct(fix time, Entity controller, Team team, BruteAIData agentData, ControlledEntity pawn)
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

    private bool Act(Entity controller, Team team, ref BruteAIData agentData, ControlledEntity pawn)
    {
        switch (agentData.State)
        {
            case BruteAIState.Patrol:
                return Act_Patrol(controller, team, ref agentData, pawn);

            case BruteAIState.PositionForAttack:
                return Act_PositionForAttack(controller, team, ref agentData, pawn);

            case BruteAIState.Attack:
                return Act_Attacking(controller, team, ref agentData, pawn);
        }

        return false;
    }

    private bool Act_Patrol(Entity controller, Team team, ref BruteAIData agentData, ControlledEntity pawn)
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

    private bool Act_PositionForAttack(Entity controller, Team team, ref BruteAIData agentData, ControlledEntity pawn)
    {
        int2 agentTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(pawn));
        int2 enemyTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(agentData.AttackTarget));

        int2[] potentialDestinations = new int2[]
        {
            enemyTile + int2(0,1),
            enemyTile + int2(0,-1),
            enemyTile + int2(1,0),
            enemyTile + int2(-1,0),
        };

        int2? closestTile = null;
        fix closestCost = fix.MaxValue;
        fix closestMinimalMoveCost = default;

        foreach (var tile in potentialDestinations)
        {
            if (Pathfinding.FindNavigablePath(Accessor, agentTile, tile, Pathfinding.MAX_PATH_LENGTH, _path))
            {
                fix cost = Pathfinding.CalculateTotalCost(_path.Slice());
                if (cost < closestCost)
                {
                    closestTile = tile;
                    closestCost = cost;

                    // find cost to move 1 step
                    closestMinimalMoveCost = Pathfinding.CalculateTotalCost(_path.Slice(0, min(2, _path.Length)));
                }
            }
        }

        if (closestTile == null)
        {
            return false;
        }

        // verify pawn has enough ap to move at least once
        if (EntityManager.TryGetComponentData(pawn, out ActionPoints ap) && ap.Value < closestMinimalMoveCost)
        {
            return false;
        }

        return CommonWrites.TryInputUseItem<GameActionMove>(Accessor, controller, closestTile.Value);
    }

    private bool Act_Attacking(Entity controller, Team team, ref BruteAIData agentData, ControlledEntity pawn)
    {
        fix2 targetPos = EntityManager.GetComponentData<FixTranslation>(agentData.AttackTarget);

        return CommonWrites.TryInputUseItem<GameActionMeleeAttack>(Accessor, controller, targetPos);
    }
}