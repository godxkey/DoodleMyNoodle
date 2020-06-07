using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;

public struct HasAlreadyPlayedTag : IComponentData
{

}

/// <summary>
/// This system generate human-like inputs for AIs with the DumbGridBattleAI tag
/// </summary>
[UpdateBefore(typeof(ExecutePawnControllerInputSystem))]
public class GenerateDumbGridBattleAIInputSystem : SimComponentSystem
{
    private EntityQuery _hasAlreadyPlayedEntityQ;

    protected override void OnCreate()
    {
        base.OnCreate();

        _hasAlreadyPlayedEntityQ = EntityManager.CreateEntityQuery(typeof(HasAlreadyPlayedTag));
    }

    protected override void OnUpdate()
    {
        // On turn change, remove the 'HasAlreadyPlayed' tag
        if (HasSingleton<NewTurnEventData>())
        {
            EntityManager.RemoveComponent<HasAlreadyPlayedTag>(_hasAlreadyPlayedEntityQ);
        }

        int currentTeam = CommonReads.GetCurrentTurnTeam(Accessor);

        Entities
            .WithAll<DumbGridBattleAITag>()
            .WithNone<HasAlreadyPlayedTag>()
            .ForEach((Entity controller, ref Team team, ref ControlledEntity controlledPawn) =>
            {
                // Can the corresponding team play ?
                if (team.Value != currentTeam)
                {
                    return;
                }

                Entity pawn = controlledPawn.Value;

                if (EntityManager.TryGetComponentData(pawn, out FixTranslation pawnPos))
                {
                    int2 pawnTile = Helpers.GetTile(pawnPos);

                    int2 attackTile = default;
                    if (HasEnemiesOnTile(controller, pawn, attackTile = pawnTile + int2(-1, 0))) // attack left
                    {
                        PawnPerform_MeleeAttack(controller, pawn, attackTile);
                    }
                    else if (HasEnemiesOnTile(controller, pawn, attackTile = pawnTile + int2(1, 0))) // attack right
                    {
                        PawnPerform_MeleeAttack(controller, pawn, attackTile);
                    }
                    else if (HasEnemiesOnTile(controller, pawn, attackTile = pawnTile + int2(0, 1))) // attack up
                    {
                        PawnPerform_MeleeAttack(controller, pawn, attackTile);
                    }
                    else if (HasEnemiesOnTile(controller, pawn, attackTile = pawnTile + int2(0, -1))) // attac down
                    {
                        PawnPerform_MeleeAttack(controller, pawn, attackTile);
                    }
                }

                switch (World.Random().NextInt(4))
                {
                    case 0:
                        PawnPerform_Move(controller, pawn, int2(-1, 0)); // move left
                        break;
                    case 1:
                        PawnPerform_Move(controller, pawn, int2(-1, 0)); // move left
                        break;
                    case 2:
                        PawnPerform_Move(controller, pawn, int2(0, 1)); // move up
                        break;
                    case 3:
                        PawnPerform_Move(controller, pawn, int2(0, -1)); // move down
                        break;
                }

                EntityManager.AddComponent<HasAlreadyPlayedTag>(controller);
            });
    }

    bool HasEnemiesOnTile(Entity controller, Entity pawn, int2 tile)
    {
        // get team
        if (!EntityManager.TryGetComponentData(controller, out Team controllerTeam))
        {
            return false;
        }


        // Search for at least 1 entity that matches:
        //  - on the desired tile
        //  - has Health
        //  - has a controller on the opposing team

        NativeList<Entity> attackableEntities = new NativeList<Entity>(Allocator.Temp);
        CommonReads.FindEntitiesOnTileWithComponents<Health, ControllableTag>(Accessor, tile, attackableEntities);

        foreach (Entity attackablePawn in attackableEntities)
        {
            Entity enemyController = CommonReads.GetPawnController(Accessor, attackablePawn);
            if (enemyController != Entity.Null && EntityManager.TryGetComponentData(enemyController, out Team team))
            {
                if (team.Value != controllerTeam.Value) // enemy team!
                {
                    return true;
                }
            }
        }

        return false;
    }

    private void PawnPerform_Move(Entity controllerEntity, Entity pawn, int2 tileMoveDelta)
    {
        // if pawn has position
        if (EntityManager.TryGetComponentData(pawn, out FixTranslation pawnPos))
        {
            // get pawn's Move item
            Entity moveItem = CommonReads.FindFirstItemWithGameAction<GameActionMove>(Accessor, pawn, out int moveItemIndex);
            if (moveItem != Entity.Null)
            {
                int2 destinationTile = roundToInt(pawnPos.Value).xy + tileMoveDelta;

                // create game action's use data
                var useData = GameAction.UseData.Create(
                    new GameActionParameterTile.Data(0, destinationTile));

                // create input
                var input = new PawnControllerInputUseItem(controllerEntity, moveItemIndex, useData);

                // queue input
                CommonWrites.QueuePawnControllerInput(Accessor, input);
            }
        }
    }

    private void PawnPerform_MeleeAttack(Entity controllerEntity, Entity pawn, int2 tile)
    {
        // get pawn's Attack item
        Entity item = CommonReads.FindFirstItemWithGameAction<GameActionMeleeAttack>(Accessor, pawn, out int itemIndex);
        if (item != Entity.Null)
        {
            // create game action's use data
            var useData = GameAction.UseData.Create(
                new GameActionParameterTile.Data(0, tile));

            // create input
            var input = new PawnControllerInputUseItem(controllerEntity, itemIndex, useData);

            // queue input
            CommonWrites.QueuePawnControllerInput(Accessor, input);

            DebugService.Log("performe melee attack");
        }
    }
}