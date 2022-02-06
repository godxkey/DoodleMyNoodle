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

            case PawnControllerInputUseItem useItemInput:
                if (pawn != Entity.Null)
                    ExecuteUseItemInput(useItemInput, pawn);
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

    private void ExecuteDropItemInput(PawnControllerInputDropItem pawnInputDropItem, Entity pawn)
    {
        // Todo : Delete Item ?
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

        if (EntityManager.TryGetComponentData(item, out ItemAction itemAction))
        {
            GameAction gameAction = GetGameActionFromEntity(itemAction.ActionPrefab);

            CommonWrites.ExecuteGameAction(Accessor, item, itemAction.ActionPrefab);
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
}