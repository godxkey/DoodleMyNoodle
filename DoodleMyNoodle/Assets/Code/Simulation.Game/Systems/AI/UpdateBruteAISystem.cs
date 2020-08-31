using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct BruteAIData : IComponentData
{
    public BruteAIState State;
    public Entity AttackTarget;
    public fix NoActionUntilTime;
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
        Entities.ForEach((Entity controller, ref Team controllerTeam, ref BruteAIData bruteData, ref ControlledEntity brutePawn) =>
        {
            UpdateMentalState(controller, controllerTeam, ref bruteData, brutePawn);
        });

        int currentTeam = CommonReads.GetCurrentTurnTeam(Accessor);
        fix time = Time.ElapsedTime;

        Entities
            .ForEach((Entity controller, ref Team team, ref BruteAIData bruteData, ref ControlledEntity pawn, ref ReadyForNextTurn readyForNextTurn) =>
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
                if (!IsReadyToAct(time, controller, team, bruteData, pawn))
                {
                    return;
                }

                if (Act(controller, team, bruteData, pawn))
                {
                    bruteData.NoActionUntilTime = time + 1;
                }
                else
                {
                    readyForNextTurn.Value = true;
                }
            });
    }

    private void UpdateMentalState(in Entity controller, in Team controllerTeam, ref BruteAIData bruteData, in Entity brutePawn)
    {
        switch (bruteData.State)
        {
            case BruteAIState.Patrol:
                UpdateMentalState_Patrol(controller, controllerTeam, ref bruteData, brutePawn);
                break;
            case BruteAIState.PositionForAttack:
                UpdateMentalState_PositionForAttack(controller, controllerTeam, ref bruteData, brutePawn);
                break;
            case BruteAIState.Attack:
                UpdateMentalState_Attacking(controller, controllerTeam, ref bruteData, brutePawn);
                break;
        }
    }

    private void UpdateMentalState_Patrol(in Entity controller, in Team controllerTeam, ref BruteAIData bruteData, in Entity brutePawn)
    {
        // TDLR: Search for enemy within range (no line of sight required)
        fix3 brutePos = EntityManager.GetComponentData<FixTranslation>(brutePawn);
        int2 bruteTile = Helpers.GetTile(brutePos);

        var positions = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true);
        var attackableEntities = _attackableGroup.ToEntityArray(Allocator.Temp);

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
            if (!CommonReads.FindNavigablePath(Accessor, bruteTile, enemyTile, maxCost: AGGRO_WALK_RANGE, _path))
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
            bruteData.AttackTarget = closest;

            if (lengthmanhattan(closestTile - bruteTile) == 1)
            {
                bruteData.State = BruteAIState.Attack;
            }
            else
            {
                bruteData.State = BruteAIState.PositionForAttack;
            }
            Log.Method("new mental state: " + bruteData.State);
        }
    }

    private void UpdateMentalState_PositionForAttack(in Entity controller, in Team controllerTeam, ref BruteAIData bruteData, in Entity brutePawn)
    {
        if (!EntityManager.Exists(bruteData.AttackTarget) || !EntityManager.TryGetComponentData(bruteData.AttackTarget, out FixTranslation enemyPos))
        {
            bruteData.State = BruteAIState.Patrol;
            Log.Method("new mental state: " + bruteData.State);
            return;
        }

        int2 enemyTile = Helpers.GetTile(enemyPos);
        int2 bruteTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(brutePawn));
        if (lengthmanhattan(enemyTile - bruteTile) == 1)
        {
            bruteData.State = BruteAIState.Attack;
            Log.Method("new mental state: " + bruteData.State);
            return;
        }
    }

    private void UpdateMentalState_Attacking(in Entity controller, in Team controllerTeam, ref BruteAIData bruteData, in Entity brutePawn)
    {
        if (!EntityManager.Exists(bruteData.AttackTarget) || !EntityManager.TryGetComponentData(bruteData.AttackTarget, out FixTranslation enemyPos))
        {
            bruteData.State = BruteAIState.Patrol;
            Log.Method("new mental state: " + bruteData.State);
            return;
        }

        int2 enemyTile = Helpers.GetTile(enemyPos);
        int2 bruteTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(brutePawn));
        if (lengthmanhattan(enemyTile - bruteTile) != 1)
        {
            bruteData.State = BruteAIState.PositionForAttack;
            Log.Method("new mental state: " + bruteData.State);
            return;
        }
    }

    private bool IsReadyToAct(in fix time, in Entity controller, in Team team, in BruteAIData bruteData, in ControlledEntity pawn)
    {
        if (time < bruteData.NoActionUntilTime)
        {
            return false;
        }

        if (EntityManager.TryGetBuffer(pawn, out DynamicBuffer<PathPosition> path) && path.Length > 0)
        {
            return false;
        }

        return true;
    }

    private bool Act(in Entity controller, in Team team, in BruteAIData bruteData, in ControlledEntity pawn)
    {
        switch (bruteData.State)
        {
            case BruteAIState.Patrol:
                return Act_Patrol(controller, team, bruteData, pawn);

            case BruteAIState.PositionForAttack:
                return Act_PositionForAttack(controller, team, bruteData, pawn);

            case BruteAIState.Attack:
                return Act_Attacking(controller, team, bruteData, pawn);
        }

        return false;
    }

    private bool Act_Patrol(in Entity controller, in Team team, in BruteAIData bruteData, in ControlledEntity pawn)
    {
        Log.Method();
        return false;
    }

    private bool Act_PositionForAttack(in Entity controller, in Team team, in BruteAIData bruteData, in ControlledEntity pawn)
    {
        int2 bruteTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(pawn));
        int2 enemyTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(bruteData.AttackTarget));

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
            if (CommonReads.FindNavigablePath(Accessor, bruteTile, tile, Pathfinding.MAX_PATH_COST, _path))
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
            Log.Method("cannot move");
            return false;
        }

        // verify pawn has enough ap to move at least once
        if (EntityManager.TryGetComponentData(pawn, out ActionPoints ap) && ap.Value < closestMinimalMoveCost)
        {
            Log.Method("not enough ap: " + ap.Value);
            return false;
        }

        Log.Method("use boots");
        return CommonWrites.TryInputUseItem<GameActionMove>(Accessor, controller, closestTile.Value);
    }

    private bool Act_Attacking(in Entity controller, in Team team, in BruteAIData bruteData, in ControlledEntity pawn)
    {
        Log.Method();
        int2 attackTile = Helpers.GetTile(EntityManager.GetComponentData<FixTranslation>(bruteData.AttackTarget));

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