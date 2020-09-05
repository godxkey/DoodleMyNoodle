using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
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

[UpdateAfter(typeof(RefillActionPointsSystem))] // TODO: use groups!
[UpdateAfter(typeof(DestroyAIControllersSystem))] // TODO: use groups!
public class UpdateBruteAISystem : SimComponentSystem
{
    private readonly fix AGGRO_WALK_RANGE = 10;

    private EntityQuery _attackableGroup;
    private NativeList<int2> _path;

    protected override void OnCreate()
    {
        base.OnCreate();

        _path = new NativeList<int2>(Allocator.Persistent);
        _attackableGroup = EntityManager.CreateEntityQuery(
            ComponentType.ReadOnly<Health>(),
            ComponentType.ReadOnly<ControllableTag>(),
            ComponentType.ReadOnly<FixTranslation>());
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _path.Dispose();
        _attackableGroup.Dispose();
    }

    protected override void OnUpdate()
    {
        Profiler.BeginSample("ForEach - Update Mental State");
        Entities.ForEach((Entity controller, ref Team controllerTeam, ref BruteAIData agentData, ref ControlledEntity pawn) =>
        {
            UpdateMentalState(controller, controllerTeam, ref agentData, pawn);
        });
        Profiler.EndSample();

        Profiler.BeginSample("ForEach - Get Turn Team");
        int currentTeam = CommonReads.GetTurnTeam(Accessor);
        fix time = Time.ElapsedTime;
        Profiler.EndSample();

        Profiler.BeginSample("ForEach - Try Act");
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
    }

    private void UpdateMentalState(in Entity controller, in Team controllerTeam, ref BruteAIData agentData, in Entity agentPawn)
    {
        switch (agentData.State)
        {
            case BruteAIState.Patrol:
                UpdateMentalState_Patrol(controller, controllerTeam, ref agentData, agentPawn);
                break;
            case BruteAIState.PositionForAttack:
                UpdateMentalState_PositionForAttack(controller, controllerTeam, ref agentData, agentPawn);
                break;
            case BruteAIState.Attack:
                UpdateMentalState_Attacking(controller, controllerTeam, ref agentData, agentPawn);
                break;
        }
    }

    private void UpdateMentalState_Patrol(in Entity controller, in Team controllerTeam, ref BruteAIData agentData, in Entity agentPawn)
    {
        // TDLR: Search for enemy within range (no line of sight required)
        fix3 pawnPos = EntityManager.GetComponentData<FixTranslation>(agentPawn);
        int2 agentTile = Helpers.GetTile(pawnPos);

        var positions = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true);
        var attackableEntities = _attackableGroup.ToEntityArray(Allocator.TempJob);

        Entity closest = Entity.Null;
        fix closestDist = fix.MaxValue;
        int2 closestTile = default;

        foreach (var enemy in attackableEntities)
        {
            // Find enemy controller in other teams
            Entity enemyController = CommonReads.GetPawnController(Accessor, enemy);

            if (enemyController == Entity.Null)
                continue;

            if (!EntityManager.TryGetComponentData(enemyController, out Team enemyTeam))
                continue;

            if (enemyTeam == controllerTeam)
                continue;

            int2 enemyTile = Helpers.GetTile(positions[enemy].Value);
            // try find path to enemy
            if (!CommonReads.FindNavigablePath(Accessor, agentTile, enemyTile, maxLength: AGGRO_WALK_RANGE, _path))
                continue;

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

            if (lengthmanhattan(closestTile - agentTile) == 1)
            {
                agentData.State = BruteAIState.Attack;
            }
            else
            {
                agentData.State = BruteAIState.PositionForAttack;
            }
        }

        attackableEntities.Dispose();
    }

    private void UpdateMentalState_PositionForAttack(in Entity controller, in Team controllerTeam, ref BruteAIData agentData, in Entity agentPawn)
    {
        if (!EntityManager.Exists(agentData.AttackTarget) || !EntityManager.TryGetComponentData(agentData.AttackTarget, out FixTranslation enemyPos))
        {
            agentData.State = BruteAIState.Patrol;
            return;
        }

        int2 enemyTile = Helpers.GetTile(enemyPos);
        int2 agentTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(agentPawn));
        if (lengthmanhattan(enemyTile - agentTile) == 1)
        {
            agentData.State = BruteAIState.Attack;
            return;
        }
    }

    private void UpdateMentalState_Attacking(in Entity controller, in Team controllerTeam, ref BruteAIData agentData, in Entity agentPawn)
    {
        if (!EntityManager.Exists(agentData.AttackTarget) || !EntityManager.TryGetComponentData(agentData.AttackTarget, out FixTranslation enemyPos))
        {
            agentData.State = BruteAIState.Patrol;
            return;
        }

        int2 enemyTile = Helpers.GetTile(enemyPos);
        int2 agentTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(agentPawn));
        if (lengthmanhattan(enemyTile - agentTile) != 1)
        {
            agentData.State = BruteAIState.PositionForAttack;
            return;
        }
    }

    private bool IsReadyToAct(in fix time, in Entity controller, in Team team, in BruteAIData agentData, in ControlledEntity pawn)
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

    private bool Act(in Entity controller, in Team team, ref BruteAIData agentData, in ControlledEntity pawn)
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

    private bool Act_Patrol(in Entity controller, in Team team, ref BruteAIData agentData, in ControlledEntity pawn)
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
            if (CommonReads.FindNavigablePath(Accessor, agentTile, tile, maxLength: 1, _path))
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

    private bool Act_PositionForAttack(in Entity controller, in Team team, ref BruteAIData agentData, in ControlledEntity pawn)
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
            if (CommonReads.FindNavigablePath(Accessor, agentTile, tile, Pathfinding.MAX_PATH_LENGTH, _path))
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

    private bool Act_Attacking(in Entity controller, in Team team, ref BruteAIData agentData, in ControlledEntity pawn)
    {
        int2 attackTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(agentData.AttackTarget));

        return CommonWrites.TryInputUseItem<GameActionMeleeAttack>(Accessor, controller, attackTile);
    }
}

static internal partial class CommonWrites
{
    public static bool TryInputUseItem<T>(ISimWorldReadWriteAccessor accessor, Entity entityController, int2 tile) where T : GameAction
    {
        if (!accessor.TryGetComponentData(entityController, out ControlledEntity pawn))
            return false;

        if (pawn == Entity.Null)
            return false;

        // get pawn's item
        Entity item = CommonReads.FindFirstItemWithGameAction<T>(accessor, pawn, out int itemIndex);
        if (item == Entity.Null)
            return false;

        // check item can be used
        var gameAction = GameActionBank.GetAction<T>();
        if (gameAction == null || !gameAction.CanBeUsedInContext(accessor, new GameAction.UseContext(entityController, pawn, item)))
            return false;

        // create game action's use data
        var useData = GameAction.UseParameters.Create(
            new GameActionParameterTile.Data(0, tile));

        // create input
        var input = new PawnControllerInputUseItem(entityController, itemIndex, useData);

        // queue input
        CommonWrites.QueuePawnControllerInput(accessor, input);

        return true;
    }
}