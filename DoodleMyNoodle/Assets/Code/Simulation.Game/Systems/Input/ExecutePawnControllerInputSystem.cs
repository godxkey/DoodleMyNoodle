using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using CCC.Fix2D;
using Unity.Collections;


// THIS CLASS SHOULD NOT BE SERIALIZABLE
public abstract class PawnControllerInputBase
{
    public Entity PawnController;

    protected PawnControllerInputBase(Entity pawnController)
    {
        PawnController = pawnController;
    }
}

/// <summary>
/// This system executes the queued inputs
/// </summary>
[UpdateInGroup(typeof(InputSystemGroup))]
public class ExecutePawnControllerInputSystem : SimGameSystemBase
{
    public readonly List<PawnControllerInputBase> Inputs = new List<PawnControllerInputBase>();

    protected override void OnUpdate()
    {
        foreach (var input in Inputs)
        {
            ExecuteInput(input);
        }
        Inputs.Clear();
    }

    private void ExecuteInput(PawnControllerInputBase input)
    {
        // Getting the Pawn from the input
        Entity pawn = Entity.Null;
        if (TryGetComponent(input.PawnController, out ControlledEntity controlledEntity))
        {
            pawn = controlledEntity.Value;

            if (!EntityManager.Exists(pawn))
                pawn = Entity.Null;
        }

        // Handling different types of Sim Inputs
        switch (input)
        {
            case PawnControllerInputSetPawnName nameInput:
                if (pawn != Entity.Null)
                {
                    EntityManager.SetOrAddComponentData(pawn, new Name() { Value = nameInput.Name });
                }

                break;

            case PawnControllerInputNextTurn pawnInputNextTurn:
                EntityManager.SetOrAddComponentData(pawnInputNextTurn.PawnController, new ReadyForNextTurn() { Value = pawnInputNextTurn.ReadyForNextTurn });
                break;

            case PawnControllerInputUseItem useItemInput:
                if (pawn != Entity.Null)
                    ExecuteUseItemInput(useItemInput, pawn);
                break;

            case PawnControllerInputUseObjectGameAction useGameAction:
                if (pawn != Entity.Null)
                    ExecuteGameAction(useGameAction, pawn);
                break;

            case PawnControllerInputClickSignalEmitter useInteractableInput:
                if (pawn != Entity.Null)
                    ExecuteUseInteractableInput(useInteractableInput, pawn);
                break;

            case PawnControllerInputEquipItem pawnInputEquipItem:
                if (pawn != Entity.Null)
                    ExecuteEquipItemInput(pawnInputEquipItem, pawn);
                break;

            case PawnControllerInputDropItem pawnInputDropItem:
                if (pawn != Entity.Null)
                    ExecuteDropItemInput(pawnInputDropItem, pawn);
                break;
        }
    }

    private void ExecuteGameAction(PawnControllerInputUseObjectGameAction useGameAction, Entity pawn)
    {
        void LogDiscardReason(string str)
        {
            Log.Info($"Discarding {useGameAction.PawnController}'s input : {str}");
        }

        Entity entityObject = FindInteractableInRange(useGameAction.ObjectPosition, pawn);

        GameAction gameAction = GetGameActionFromEntity(entityObject);

        if (gameAction == null)
        {
            LogDiscardReason($"Object {entityObject}'s gameActionId is invalid.");
            return;
        }

        GameAction.UseContext useContext = new GameAction.UseContext()
        {
            InstigatorPawn = pawn,
            InstigatorPawnController = useGameAction.PawnController,
            Item = entityObject
        };

        if (!gameAction.TryUse(Accessor, useContext, useGameAction.GameActionData, out string debugReason))
        {
            LogDiscardReason($"Can't Trigger {gameAction} because: {debugReason}");
            return;
        }
    }

    private void ExecuteDropItemInput(PawnControllerInputDropItem pawnInputDropItem, Entity pawn)
    {
        FixTranslation pawnPos = GetComponent<FixTranslation>(pawn);

        // Searching for a chest entity
        Entity chestEntity = FindChestInRange(pawnPos, pawn);

        // Didn't found an inventory, let's spawn one
        if (chestEntity == Entity.Null)
        {
            InteractableInventoryPrefabReferenceSingletonComponent chestPrefab = GetSingleton<InteractableInventoryPrefabReferenceSingletonComponent>();
            chestEntity = EntityManager.Instantiate(chestPrefab.Prefab);
            SetComponent(chestEntity, pawnPos);
        }

        // Does the player have an inventory with a valid item at 'ItemIndex' ?
        if (!EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> pawnInventory) ||
            pawnInventory.Length <= pawnInputDropItem.ItemIndex)
        {
            return;
        }

        var item = pawnInventory[pawnInputDropItem.ItemIndex].ItemEntity;

        CommonWrites.MoveItemAll(Accessor, item, source: pawn, destination: chestEntity);


        /* // Disabled passive system code
        List<ItemPassiveEffect> itemPassiveEffects = GetItemPassiveEffects(entityToGiveToChest);

        ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
        {
            InstigatorPawn = pawn,
            ItemEntity = entityToGiveToChest
        };

        foreach (ItemPassiveEffect itemPassiveEffect in itemPassiveEffects)
        {
            if (itemPassiveEffect != null)
            {
                itemPassiveEffect.Unequip(Accessor, itemContext);
            }
        }
         */
    }

    private void ExecuteEquipItemInput(PawnControllerInputEquipItem pawnInputEquipItem, Entity pawn)
    {
        // Pawn has inventory ?
        if (!EntityManager.HasComponent<InventoryItemReference>(pawn))
            return;

        // Find chest inventory
        Entity chestEntity = pawnInputEquipItem.ChestEntity;
        if (!EntityManager.HasComponent<InventoryItemReference>(chestEntity))
        {
            return;
        }

        // Get item buffer
        DynamicBuffer<InventoryItemReference> chestInventory = GetBuffer<InventoryItemReference>(chestEntity);
        if (chestInventory.Length <= pawnInputEquipItem.ItemIndex)
        {
            return;
        }

        Entity item = chestInventory[pawnInputEquipItem.ItemIndex].ItemEntity;

        CommonWrites.MoveItemAll(Accessor, item, source: chestEntity, destination: pawn);

        /* Disabled passive code
         * 
            ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
            {
                InstigatorPawn = pawn,
                ItemEntity = item
            };

            foreach (ItemPassiveEffect itemPassiveEffect in itemPassiveEffects)
            {
                if (itemPassiveEffect != null)
                {
                    itemPassiveEffect.Equip(Accessor, itemContext);
                }
            }
         * */
    }

    private void ExecuteUseItemInput(PawnControllerInputUseItem inputUseItem, Entity pawn)
    {
        void LogDiscardReason(string str)
        {
            Log.Info($"Discarding {inputUseItem.PawnController}'s input : {str}");
        }

        if (!EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            LogDiscardReason($"Pawn has no {nameof(DynamicBuffer<InventoryItemReference>)}.");
            return;
        }

        if (inputUseItem.ItemIndex < 0 || inputUseItem.ItemIndex >= inventory.Length)
        {
            LogDiscardReason($"Item {inputUseItem.ItemIndex} is out of range ({inventory.Length}).");
            return;
        }

        Entity item = inventory[inputUseItem.ItemIndex].ItemEntity;

        // ITEM PASSIVES

        List<ItemPassiveEffect> itemPassiveEffects = GetItemPassiveEffects(item);

        ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
        {
            InstigatorPawn = pawn,
            ItemEntity = item
        };

        foreach (ItemPassiveEffect itemPassiveEffect in itemPassiveEffects)
        {
            if (itemPassiveEffect != null)
            {
                itemPassiveEffect.Use(Accessor, itemContext);
            }
        }

        // GAME ACTIONS

        GameAction gameAction = GetGameActionFromEntity(item);

        GameAction.UseContext useContext = new GameAction.UseContext()
        {
            InstigatorPawn = pawn,
            InstigatorPawnController = inputUseItem.PawnController,
            Item = item
        };

        if (gameAction != null && !gameAction.TryUse(Accessor, useContext, inputUseItem.GameActionData, out string debugReason))
        {
            LogDiscardReason($"Can't Trigger {gameAction} because: {debugReason}");
            return;
        }
    }

    private void ExecuteUseInteractableInput(PawnControllerInputClickSignalEmitter inputClickEmitter, Entity pawn)
    {
        Entity emitter = inputClickEmitter.Emitter;

        if (TryGetComponent(emitter, out SignalEmissionType emissionType))
        {
            if (emissionType.Value == ESignalEmissionType.OnClick || emissionType.Value == ESignalEmissionType.ToggleOnClick)
            {
                World.GetOrCreateSystem<SetSignalSystem>().EmitterClickRequests.Add(emitter);
            }
        }
    }

    private GameAction GetGameActionFromEntity(Entity entity)
    {
        if (TryGetComponent(entity, out GameActionId gameActionId) && gameActionId.IsValid)
        {
            return GameActionBank.GetAction(gameActionId);
        }

        return null;
    }

    private List<ItemPassiveEffect> GetItemPassiveEffects(Entity entity)
    {
        List<ItemPassiveEffect> result = new List<ItemPassiveEffect>();
        if (EntityManager.TryGetBuffer(entity, out DynamicBuffer<ItemPassiveEffectId> itemPassiveEffectIds))
        {
            foreach (ItemPassiveEffectId itemPassiveEffectId in itemPassiveEffectIds)
            {
                result.Add(ItemPassiveEffectBank.GetItemPassiveEffect(itemPassiveEffectId));
            }
        }

        return result;
    }

    private Entity FindInteractableInRange(fix2 targetPosition, Entity pawn)
    {
        return ValidateInRange(pawn, FindNearestInteractable(targetPosition));
    }

    private Entity FindChestInRange(fix2 targetPosition, Entity pawn)
    {
        return ValidateInRange(pawn, FindNearestChest(targetPosition));
    }

    private Entity ValidateInRange(Entity pawn, Entity closest)
    {
        if (closest == Entity.Null)
            return Entity.Null;

        fix2 closestPosition = GetComponent<FixTranslation>(closest);

        fix2 pawnPosition = GetComponent<FixTranslation>(pawn);
        if (fixMath.distancemanhattan(closestPosition, pawnPosition) > SimulationGameConstants.InteractibleMaxDistanceManhattan)
            return Entity.Null;

        return closest;
    }

    private Entity FindNearestInteractable(fix2 targetPosition)
    {
        fix closestDistance = fix.MaxValue;
        Entity closestEntity = Entity.Null;

        Entities
            .ForEach((Entity entity, in FixTranslation position, in InteractableFlag interactable) =>
            {
                if (!interactable)
                    return;

                fix distance = fixMath.lengthmanhattan(position.Value - targetPosition);
                if (distance < closestDistance)
                {
                    closestEntity = entity;
                    closestDistance = distance;
                }
            }).Run();

        return closestEntity;
    }

    private Entity FindNearestChest(fix2 targetPosition)
    {
        fix closestDistance = fix.MaxValue;
        Entity closestEntity = Entity.Null;

        Entities
            .WithAll<InventoryItemReference>()
            .ForEach((Entity entity, in FixTranslation position, in InteractableFlag interactable) =>
            {
                if (!interactable)
                    return;

                fix distance = fixMath.lengthmanhattan(position.Value - targetPosition);
                if (distance < closestDistance)
                {
                    closestEntity = entity;
                    closestDistance = distance;
                }
            }).Run();

        return closestEntity;
    }
}


internal partial class CommonWrites
{
    public static void QueuePawnControllerInput(ISimGameWorldReadWriteAccessor accessor, PawnControllerInputBase input, Entity pawnController)
    {
        input.PawnController = pawnController;
        QueuePawnControllerInput(accessor, input);
    }

    public static void QueuePawnControllerInput(ISimGameWorldReadWriteAccessor accessor, PawnControllerInputBase input)
    {
        ExecutePawnControllerInputSystem system = accessor.GetOrCreateSystem<ExecutePawnControllerInputSystem>();

        system.Inputs.Add(input);
    }

    public static bool TryInputUseItem<T>(ISimGameWorldReadWriteAccessor accessor, Entity entityController, int2 tile) where T : GameAction
    {
        return TryInputUseItem<T>(accessor, entityController, new GameActionParameterTile.Data(tile));
    }

    public static bool TryInputUseItem<T>(ISimGameWorldReadWriteAccessor accessor, Entity entityController, fix2 pos) where T : GameAction
    {
        return TryInputUseItem<T>(accessor, entityController, new GameActionParameterPosition.Data(pos));
    }

    public static bool TryInputUseItem<T>(ISimGameWorldReadWriteAccessor accessor, Entity entityController, params GameAction.ParameterData[] arguments) where T : GameAction
    {
        if (!accessor.TryGetComponent(entityController, out ControlledEntity pawn))
            return false;

        if (pawn == Entity.Null)
            return false;

        // get pawn's item
        Entity item = CommonReads.FindFirstItemWithGameAction<T>(accessor, pawn, out int itemIndex);
        if (item == Entity.Null)
            return false;

        // check item can be used
        var gameAction = GameActionBank.GetAction<T>();
        var useContext = new GameAction.UseContext()
        {
            InstigatorPawnController = entityController,
            InstigatorPawn = pawn,
            Item = item
        };
        if (gameAction == null || !gameAction.CanBeUsedInContext(accessor, useContext))
            return false;

        // create game action's use data
        var useData = GameAction.UseParameters.Create(arguments);

        // create input
        var input = new PawnControllerInputUseItem(entityController, itemIndex, useData);

        // queue input
        CommonWrites.QueuePawnControllerInput(accessor, input);

        return true;
    }
}