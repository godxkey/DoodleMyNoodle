using CCC.Fix2D;
using System;
using System.Linq;
using Unity.Animation;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public class FillDynamicChestsSystem : SimGameSystemBase
{
    private struct WorldContext
    {
        public fix TeamHealthRatio;
        public fix AverageConsumablesPerPlayer;
    }

    protected override void OnUpdate()
    {
        FillChests();
        MarkChestsCloseToPlayerToFill();
    }

    private void MarkChestsCloseToPlayerToFill()
    {
        NativeList<fix2> playerPositions = new NativeList<fix2>(Allocator.TempJob);

        Entities
            .WithAll<PlayerTag>()
            .ForEach((in ControlledEntity pawn) =>
            {
                if (HasComponent<FixTranslation>(pawn))
                {
                    playerPositions.Add(GetComponent<FixTranslation>(pawn));
                }
            }).Schedule();

        Entities
            .WithDisposeOnCompletion(playerPositions)
            .WithAll<DynamicChestFormulaRef>()
            .ForEach((ref DynamicChestFillOnNextUpdateToken fillToken, in FixTranslation position) =>
            {
                foreach (var p in playerPositions)
                {
                    fillToken.Value |= distancemanhattan(position, p) < SimulationGameConstants.DynamicChestTriggerRadius;
                }
            }).Schedule();
    }

    private void FillChests()
    {
        NativeList<Entity> chestsToFill = new NativeList<Entity>(Allocator.Temp);
        Entities
            .WithAll<DynamicChestFormulaRef>()
            .ForEach((Entity chest, in DynamicChestFillOnNextUpdateToken fillToken) =>
            {
                if (fillToken.Value)
                {
                    chestsToFill.Add(chest);
                }
            }).Run();


        if (chestsToFill.Length > 0)
        {
            foreach (var chest in chestsToFill)
            {
                Entity formulaEntity = GetComponent<DynamicChestFormulaRef>(chest).FormulaEntity;

                if (EntityManager.HasComponent<DynamicChestFormulaItemEntry>(formulaEntity) &&
                    HasComponent<DynamicChestFormulaSettings>(formulaEntity))
                {
                    var worldContext = GetWorldContext();
                    var itemPoolBuffer = GetBuffer<DynamicChestFormulaItemEntry>(formulaEntity);
                    var forumlaSettings = GetComponent<DynamicChestFormulaSettings>(formulaEntity);
                    FillChest(worldContext, chest, itemPoolBuffer, forumlaSettings);
                }

                EntityManager.RemoveComponent<DynamicChestFormulaRef>(chest);
                EntityManager.RemoveComponent<DynamicChestFillOnNextUpdateToken>(chest);
            }
        }
    }

    private WorldContext GetWorldContext()
    {
        WorldContext context = new WorldContext();

        // Health Ratio
        {
            fix totalHealthRatios = 0;
            int playerCount = 0;
            Entities.WithAll<PlayerTag>()
                .ForEach((in ControlledEntity pawn, in Active active) =>
                {
                    if (active && HasComponent<Health>(pawn) && HasComponent<MaximumFix<Health>>(pawn))
                    {
                        totalHealthRatios += (fix)GetComponent<Health>(pawn).Value / GetComponent<MaximumFix<Health>>(pawn).Value;
                        playerCount++;
                    }
                }).Run();

            context.TeamHealthRatio = playerCount == 0 ? 1 : totalHealthRatios / playerCount;
        }


        // Consumables
        {
            var inventoryFromEntity = GetBufferFromEntity<InventoryItemReference>(isReadOnly: true);
            int playerCount = 0;
            int totalConsumable = 0;
            Entities.WithAll<PlayerTag>()
                .ForEach((in ControlledEntity pawn, in Active active) =>
                {
                    if (active && inventoryFromEntity.HasComponent(pawn))
                    {
                        var inventory = inventoryFromEntity[pawn];
                        foreach (var itemRef in inventory)
                        {
                            if (HasComponent<StackableFlag>(itemRef.ItemEntity)
                            && GetComponent<StackableFlag>(itemRef.ItemEntity).Value)
                            {
                                totalConsumable += itemRef.Stacks;
                            }
                        }
                        playerCount++;
                    }
                }).Run();

            context.AverageConsumablesPerPlayer = playerCount == 0 ? 0 : (fix)totalConsumable / playerCount;
        }

        return context;
    }

    [ConsoleVar(Name = "LogDynamicChests", Save = ConsoleVarAttribute.SaveMode.PlayerPrefs)]
    private static bool s_logDynamicChests;

    private void FillChest(WorldContext worldContext,
        Entity chest,
        DynamicBuffer<DynamicChestFormulaItemEntry> formulaPool,
        DynamicChestFormulaSettings formulaSettings)
    {

        fix teamHPMultiplier = (fix)AnimationCurveEvaluator.Evaluate((float)worldContext.TeamHealthRatio, formulaSettings.TeamHealthRatioBudget.GetAnimationCurveBlobAssetRef());
        fix teamConsumablesMultiplier = (fix)AnimationCurveEvaluator.Evaluate((float)worldContext.AverageConsumablesPerPlayer, formulaSettings.ConsumablesPerPawnBudget.GetAnimationCurveBlobAssetRef());

        fix startingBudget = formulaSettings.BudgetBase * teamHPMultiplier * teamConsumablesMultiplier;
        startingBudget = clamp(startingBudget, formulaSettings.BudgetMin, formulaSettings.BudgetMax);

        FixLottery<(Entity item, fix cost)> itemLottery = new FixLottery<(Entity item, fix cost)>(Allocator.Temp, formulaPool.Length);
        foreach (var item in formulaPool)
        {
            itemLottery.Add((item.Item, item.BudgetCost), item.Weight);
        }

        FixRandom random = World.Random();

        NativeList<(Entity item, int stacks)> pickedItems = new NativeList<(Entity item, int stacks)>(Allocator.Temp);

        fix remainingBudget = startingBudget;
        while (remainingBudget > 0)
        {
            RemoveItemsOutOfBudget();

            if (itemLottery.Length == 0)
                break;

            // Pick  item
            (Entity item, fix cost) = itemLottery.Pick(ref random);

            // Adjust budget
            remainingBudget -= cost;

            // Add item in list
            int existingIndex = -1;
            for (int i = 0; i < pickedItems.Length; i++)
            {
                if (pickedItems[i].item == item)
                {
                    existingIndex = i;
                    break;
                }
            }

            // Already in list ? increment stack
            if (existingIndex != -1)
            {
                pickedItems[existingIndex] = (item, pickedItems[existingIndex].stacks + 1);
            }
            else
            {

                pickedItems.Add((item, 1));
            }

        }

        // local function
        void RemoveItemsOutOfBudget()
        {
            for (int i = itemLottery.Length - 1; i >= 0; i--)
            {
                if (itemLottery[i].cost > remainingBudget)
                {
                    itemLottery.RemoveAt(i);
                }
            }
        }

        if (s_logDynamicChests)
        {
            string chestName(Entity e)
#if UNITY_EDITOR
                => EntityManager.GetName(e);
#else
                => e.ToString();
#endif

            Log.Info($"Dynamic Chest --  " +
                $"BUDGET:{startingBudget}" +
                $"  =  " +
                $"budgetBase:{formulaSettings.BudgetBase}" +
                $"  *  " +
                $"teamHPMultiplier:{teamHPMultiplier}" +
                $"  *  " +
                $"teamConsumablesMultiplier:{teamConsumablesMultiplier}" +
                $"\n(" +
                $"Min:{formulaSettings.BudgetMin}  Max:{formulaSettings.BudgetMax}" +
                $",  " +
                $"TeamHealthRatio:{worldContext.TeamHealthRatio}" +
                $",  " +
                $"AverageConsumablesPerPlayer:{worldContext.AverageConsumablesPerPlayer}" +
                $")\n\n" +
                $"ITEMS: {string.Join(", ", pickedItems.ToArray().Select(p => $"{chestName(p.item)} x {p.stacks}"))}");
        }

        // Give items
        ItemTransationBatch transation = new ItemTransationBatch()
        {
            Destination = chest,
            Source = null,
            ItemsAndStacks = pickedItems.AsArray()
        };

        CommonWrites.ExecuteItemTransaction(Accessor, transation);
    }
}
