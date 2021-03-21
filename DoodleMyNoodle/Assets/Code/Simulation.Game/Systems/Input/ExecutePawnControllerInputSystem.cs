using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using CCC.Fix2D;


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
/// This system makes sure inputs sent from the previous frame don't leak to the next. 
/// This is a necessary precaution because inputs are not natively serialized in the sim world
/// </summary>
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class ClearPawnControllerInputSystem : SimSystemBase
{
    private ExecutePawnControllerInputSystem _executeSys;

    protected override void OnCreate()
    {
        base.OnCreate();

        _executeSys = World.GetOrCreateSystem<ExecutePawnControllerInputSystem>();
    }

    protected override void OnUpdate()
    {
        foreach (PawnControllerInputBase input in _executeSys.Inputs)
        {
            Log.Warning($"The PawnControllerInput {input} seems to have been queued too late. " +
                $"Use [UpdateBefore({nameof(ExecutePawnControllerInputSystem)})] to make sure you deliver the input in time.");
        }
        _executeSys.Inputs.Clear();
    }
}

/// <summary>
/// This system executes the queued inputs
/// </summary>
[UpdateInGroup(typeof(InputSystemGroup))]
public class ExecutePawnControllerInputSystem : SimSystemBase
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

            case PawnControllerInputUseInteractable useInteractableInput:
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

        Entity entityObject = CommonReads.FindFirstTileActorWithComponent<GameActionId>(Accessor, useGameAction.ObjectPosition);

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
            Entity = entityObject
        };

        if (!gameAction.TryUse(Accessor, useContext, useGameAction.GameActionData, out string debugReason))
        {
            LogDiscardReason($"Can't Trigger {gameAction} because: {debugReason}");
            return;
        }
    }

    private void ExecuteDropItemInput(PawnControllerInputDropItem pawnInputDropItem, Entity pawn)
    {
        FixTranslation pawnTranslation = GetComponent<FixTranslation>(pawn);
        int2 pawnTile = Helpers.GetTile(pawnTranslation);
        var tileWorld = CommonReads.GetTileWorld(Accessor);
        if (!tileWorld.IsValid(pawnTile))
        {
            return;
        }

        // Does the player have an inventory with a valid item at 'ItemIndex' ?
        if (!EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> p) ||
            p.Length <= pawnInputDropItem.ItemIndex)
        {
            return;
        }

        // Searching for a chest entity
        DynamicBuffer<InventoryItemReference> chestInventory = default;

        Entity chestEntity = CommonReads.FindFirstTileActorWithComponent<InventoryItemReference, Interactable>(Accessor, tileWorld, pawnTile);
        if (chestEntity != Entity.Null)
        {
            chestInventory = GetBuffer<InventoryItemReference>(chestEntity);
        }

        // Didn't found an inventory, let's spawn one
        if (!chestInventory.IsCreated)
        {
            InteractableInventoryPrefabReferenceSingletonComponent chestPrefab = GetSingleton<InteractableInventoryPrefabReferenceSingletonComponent>();
            chestEntity = EntityManager.Instantiate(chestPrefab.Prefab);
            SetComponent(chestEntity, pawnTranslation);

            chestInventory = Accessor.GetBuffer<InventoryItemReference>(chestEntity);
        }

        // Move item from player's inventory to chest inventory
        if (!CommonReads.IsInventoryFull(Accessor, chestEntity))
        {
            DynamicBuffer<InventoryItemReference> pawnInventory = GetBuffer<InventoryItemReference>(pawn);

            Entity itemToMove = pawnInventory[pawnInputDropItem.ItemIndex].ItemEntity;
            if (CommonWrites.TryIncrementStackableItemInInventory(Accessor, chestEntity, itemToMove, chestInventory))
            {
                CommonWrites.DecrementStackableItemInInventory(Accessor, pawn, itemToMove);

                List<ItemPassiveEffect> itemPassiveEffects = GetItemPassiveEffects(itemToMove);

                ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
                {
                    InstigatorPawn = pawn,
                    ItemEntity = itemToMove
                };

                foreach (ItemPassiveEffect itemPassiveEffect in itemPassiveEffects)
                {
                    if (itemPassiveEffect != null)
                    {
                        itemPassiveEffect.Unequip(Accessor, itemContext);
                    }
                }
            }
            else
            {
                Entity entityToGiveToChest;

                // We did not find any stackable in destination inventory, but we want to transfer only one if stackable
                if (HasComponent<ItemStackableData>(itemToMove))
                {
                    entityToGiveToChest = EntityManager.Instantiate(itemToMove);
                    SetComponent(entityToGiveToChest, new ItemStackableData() { Value = 1 });
                    CommonWrites.DecrementStackableItemInInventory(Accessor, pawn, itemToMove);
                }
                else
                {
                    entityToGiveToChest = itemToMove;
                    pawnInventory.RemoveAt(pawnInputDropItem.ItemIndex);
                }

                chestInventory = GetBuffer<InventoryItemReference>(chestEntity);
                chestInventory.Add(new InventoryItemReference() { ItemEntity = entityToGiveToChest });

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
            }
        }
    }

    private void ExecuteEquipItemInput(PawnControllerInputEquipItem pawnInputEquipItem, Entity pawn)
    {
        int2 itemEntityTile = pawnInputEquipItem.ItemEntityPosition;
        var tileWorld = CommonReads.GetTileWorld(Accessor);
        if (!tileWorld.IsValid(itemEntityTile))
        {
            return;
        }

        // Pawn has inventory ?
        if (!EntityManager.HasComponent<InventoryItemReference>(pawn))
            return;

        // Find chest inventory
        Entity chestEntity = CommonReads.FindFirstTileActorWithComponent<InventoryItemReference, Interactable>(Accessor, itemEntityTile);
        if (chestEntity == Entity.Null)
        {
            return;
        }

        // Get item buffer
        DynamicBuffer<InventoryItemReference> chestInventory = GetBuffer<InventoryItemReference>(chestEntity);
        if (chestInventory.Length <= pawnInputEquipItem.ItemIndex)
        {
            return;
        }

        // Get item to move
        Entity item = chestInventory[pawnInputEquipItem.ItemIndex].ItemEntity;

        if (!CommonReads.IsInventoryFull(Accessor, pawn))
        {
            // Trying to increment stack on pawn and if succeed decrement original item on chest
            if (CommonWrites.TryIncrementStackableItemInInventory(Accessor, pawn, item, GetBuffer<InventoryItemReference>(pawn)))
            {
                CommonWrites.DecrementStackableItemInInventory(Accessor, chestEntity, item);

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
                        itemPassiveEffect.Equip(Accessor, itemContext);
                    }
                }
            }
            else
            {
                Entity itemToGiveToPawn;

                // We did not find any stackable in destination inventory, but we want to transfer only one if stackable
                if (HasComponent<ItemStackableData>(item))
                {
                    itemToGiveToPawn = EntityManager.Instantiate(item);
                    SetComponent(itemToGiveToPawn, new ItemStackableData() { Value = 1 });
                    CommonWrites.DecrementStackableItemInInventory(Accessor, chestEntity, item);
                }
                else
                {
                    chestInventory.RemoveAt(pawnInputEquipItem.ItemIndex);
                    itemToGiveToPawn = item;
                }

                // Move item from chest inventory to player's inventory
                GetBuffer<InventoryItemReference>(pawn).Add(new InventoryItemReference() { ItemEntity = itemToGiveToPawn });

                List<ItemPassiveEffect> itemPassiveEffects = GetItemPassiveEffects(item);

                ItemPassiveEffect.ItemContext itemContext = new ItemPassiveEffect.ItemContext()
                {
                    InstigatorPawn = pawn,
                    ItemEntity = itemToGiveToPawn
                };

                foreach (ItemPassiveEffect itemPassiveEffect in itemPassiveEffects)
                {
                    if (itemPassiveEffect != null)
                    {
                        itemPassiveEffect.Equip(Accessor, itemContext);
                    }
                }
            }
        }
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
            Entity = item
        };

        if (gameAction != null && !gameAction.TryUse(Accessor, useContext, inputUseItem.GameActionData, out string debugReason))
        {
            LogDiscardReason($"Can't Trigger {gameAction} because: {debugReason}");
            return;
        }
    }

    private void ExecuteUseInteractableInput(PawnControllerInputUseInteractable inputUseInteractable, Entity pawn)
    {
        //void LogDiscardReason(string str)
        //{
        //    Log.Info($"Discarding input {inputUseInteractable} : {str}");
        //}

        Entity interactableEntity = CommonReads.FindFirstTileActorWithComponent<Interactable>(Accessor, inputUseInteractable.InteractablePosition);
        if (interactableEntity == Entity.Null)
        {
            return;
        }

        fix interactableTileDistance = Accessor.GetComponentData<Interactable>(interactableEntity).Range;
        FixTranslation pawnPosition = GetComponent<FixTranslation>(pawn);
        fix3 interactablePosition = Helpers.GetTileCenter(inputUseInteractable.InteractablePosition);

        int tilesBetween = fix.RoundToInt(fix.Abs((interactablePosition.x - pawnPosition.Value.x) + (interactablePosition.y - pawnPosition.Value.y)));
        if (tilesBetween > interactableTileDistance)
        {
            return;
        }

        CommonWrites.Interact(Accessor, interactableEntity, pawn);
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
}


internal partial class CommonWrites
{
    public static void QueuePawnControllerInput(ISimWorldReadWriteAccessor accessor, PawnControllerInputBase input, Entity pawnController)
    {
        input.PawnController = pawnController;
        QueuePawnControllerInput(accessor, input);
    }

    public static void QueuePawnControllerInput(ISimWorldReadWriteAccessor accessor, PawnControllerInputBase input)
    {
        ExecutePawnControllerInputSystem system = accessor.GetOrCreateSystem<ExecutePawnControllerInputSystem>();

        system.Inputs.Add(input);
    }

    public static bool TryInputUseItem<T>(ISimWorldReadWriteAccessor accessor, Entity entityController, int2 tile) where T : GameAction
    {
        return TryInputUseItem<T>(accessor, entityController, new GameActionParameterTile.Data(tile));
    }

    public static bool TryInputUseItem<T>(ISimWorldReadWriteAccessor accessor, Entity entityController, params GameAction.ParameterData[] arguments) where T : GameAction
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
        var useData = GameAction.UseParameters.Create(arguments);

        // create input
        var input = new PawnControllerInputUseItem(entityController, itemIndex, useData);

        // queue input
        CommonWrites.QueuePawnControllerInput(accessor, input);

        return true;
    }
}