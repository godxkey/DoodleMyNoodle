using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;


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
public class ClearPawnControllerInputSystem : SimComponentSystem
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
public class ExecutePawnControllerInputSystem : SimComponentSystem
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
        if (EntityManager.TryGetComponentData(input.PawnController, out ControlledEntity controlledEntity))
        {
            pawn = controlledEntity.Value;
            
            if (!EntityManager.Exists(pawn))
                pawn = Entity.Null;
        }

        if (pawn == Entity.Null)
            return;
        
        // Handling different types of Sim Inputs
        switch (input)
        {
            case PawnStartingInventorySelectionInput equipStartingInventoryInput:
                if(pawn != Entity.Null)
                {
                    DynamicBuffer<InventoryItemPrefabReference> startingInventory = default;
                    Entities
                        .WithAll<ItemKitTag>()
                        .ForEach((DynamicBuffer<InventoryItemPrefabReference> inventoryItems, ref SimAssetId assetID) =>
                        {
                            if (equipStartingInventoryInput.KitNumber == assetID.Value)
                            {
                                startingInventory = inventoryItems;
                            }
                        });

                    if (startingInventory.IsCreated)
                    {
                        CommonWrites.InstantiateToEntityInventory(Accessor, pawn, startingInventory);
                    }
                }
                break;

            case PawnCharacterNameInput nameInput:
                if (pawn != Entity.Null)
                {
                    EntityManager.SetOrAddComponentData(pawn, new Name() { Value = nameInput.Name });
                }
                
                break;

            case PawnInputNextTurn pawnInputNextTurn:
                EntityManager.SetOrAddComponentData(pawnInputNextTurn.PawnController, new ReadyForNextTurn() { Value = pawnInputNextTurn.ReadyForNextTurn });
                break;
        
            case PawnControllerInputUseItem useItemInput:
                if(pawn != Entity.Null)
                    ExecuteUseGameActionInput(useItemInput, pawn);
                break;

            case PawnControllerInputUseInteractable useInteractableInput:
                if(pawn != Entity.Null)
                    ExecuteUseGameActionInput(useInteractableInput, pawn);
                break;

            case PawnInputEquipItem pawnInputEquipItem:
                if(pawn != Entity.Null)
                    ExecuteEquipItemInput(pawnInputEquipItem, pawn);
                break;

            case PawnInputDropItem pawnInputDropItem:
                if(pawn != Entity.Null)
                    ExecuteDropItemInput(pawnInputDropItem, pawn);
                break;
        }
    }

    private void ExecuteDropItemInput(PawnInputDropItem pawnInputDropItem, Entity pawn)
    {
        FixTranslation pawnTranslation = EntityManager.GetComponentData<FixTranslation>(pawn);
        Entity tile = CommonReads.GetTileEntity(Accessor, Helpers.GetTile(pawnTranslation));
        if (tile == Entity.Null)
        {
            return;
        }

        // Does the player have an inventory with a valid item at 'ItemIndex' ?
        if (!EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> p) ||
            p.Length <= pawnInputDropItem.ItemIndex)
        {
            return;
        }

        // Searching for an Inventory addon on tile
        DynamicBuffer<InventoryItemReference> addonInventoryItemBuffer = default;

        Entity groundInventoryEntity = CommonReads.GetFirstTileAddonWithComponent<InventoryItemReference>(Accessor, tile);
        if(groundInventoryEntity != Entity.Null)
        {
            addonInventoryItemBuffer = EntityManager.GetBuffer<InventoryItemReference>(groundInventoryEntity);
        }

        // Didn't found an inventory, let's spawn one
        if (!addonInventoryItemBuffer.IsCreated)
        {
            InteractableInventoryPrefabReference interactableInventoryPrefab = GetSingleton<InteractableInventoryPrefabReference>();
            groundInventoryEntity = EntityManager.Instantiate(interactableInventoryPrefab.Prefab);
            EntityManager.SetComponentData(groundInventoryEntity, pawnTranslation);
            CommonWrites.AddTileAddon(Accessor, groundInventoryEntity, tile);

            addonInventoryItemBuffer = Accessor.AddBuffer<InventoryItemReference>(groundInventoryEntity);
        }

        // Move item from player's inventory to ground inventory
        if (!CommonReads.IsInventoryFull(Accessor, groundInventoryEntity))
        {
            if (EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> pawnInventory))
            {
                InventoryItemReference itemToMove = pawnInventory[pawnInputDropItem.ItemIndex];
                if (CommonWrites.TryIncrementStackableItemInInventory(Accessor, groundInventoryEntity, itemToMove.ItemEntity))
                {
                    CommonWrites.DecrementStackableItemInInventory(Accessor, pawn, itemToMove.ItemEntity);
                }
                else
                {
                    // We did not find any stackable in destination inventory, but we want to transfer only one if stackable
                    if (EntityManager.TryGetComponentData(itemToMove.ItemEntity, out ItemStackableData stackableData))
                    {
                        Entity newItemEntity = EntityManager.Instantiate(itemToMove.ItemEntity);
                        EntityManager.SetComponentData(newItemEntity, new ItemStackableData() { Value = 1 });
                        EntityManager.GetBuffer<InventoryItemReference>(groundInventoryEntity).Add(new InventoryItemReference() { ItemEntity = newItemEntity });
                        CommonWrites.DecrementStackableItemInInventory(Accessor, pawn, itemToMove.ItemEntity);
                    }
                    else
                    {
                        pawnInventory.RemoveAt(pawnInputDropItem.ItemIndex);
                        addonInventoryItemBuffer.Add(itemToMove);
                    }
                }
            }
        }
    }

    private void ExecuteEquipItemInput(PawnInputEquipItem pawnInputEquipItem, Entity pawn)
    {
        Entity tile = CommonReads.GetTileEntity(Accessor, pawnInputEquipItem.ItemEntityPosition);
        if (tile == Entity.Null)
        {
            return;
        }

        // Find ground inventory
        Entity groundInventoryEntity = CommonReads.GetFirstTileAddonWithComponent<InventoryItemReference>(Accessor, tile);
        if (groundInventoryEntity == Entity.Null)
        {
            return;
        }

        // Get item buffer
        DynamicBuffer<InventoryItemReference> itemsBuffer = EntityManager.GetBuffer<InventoryItemReference>(groundInventoryEntity);
        if (itemsBuffer.Length <= pawnInputEquipItem.ItemIndex)
        {
            return;
        }

        // Get item to move
        InventoryItemReference item = itemsBuffer[pawnInputEquipItem.ItemIndex];

        if(!CommonReads.IsInventoryFull(Accessor, pawn))
        {
            // Trying to increment stack on pawn and if succeed decrement original item on ground
            if (CommonWrites.TryIncrementStackableItemInInventory(Accessor, pawn, item.ItemEntity))
            {
                CommonWrites.DecrementStackableItemInInventory(Accessor, groundInventoryEntity, item.ItemEntity);
            }
            else
            {
                // Move item from ground inventory to player's inventory
                if (EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> pawnInventory))
                {
                    // We did not find any stackable in destination inventory, but we want to transfer only one if stackable
                    if (EntityManager.TryGetComponentData(item.ItemEntity, out ItemStackableData stackableData))
                    {
                        Entity newItemEntity = EntityManager.Instantiate(item.ItemEntity);
                        EntityManager.SetComponentData(newItemEntity, new ItemStackableData() { Value = 1 });
                        pawnInventory.Add(new InventoryItemReference() { ItemEntity = newItemEntity });
                        CommonWrites.DecrementStackableItemInInventory(Accessor, groundInventoryEntity, item.ItemEntity);
                    }
                    else
                    {
                        itemsBuffer.RemoveAt(pawnInputEquipItem.ItemIndex);
                        pawnInventory.Add(item);
                    }
                }
            }
        }
    }

    private void ExecuteUseGameActionInput(PawnControllerInputBase inputUseGameAction, Entity pawn)
    {
        switch (inputUseGameAction)
        {
            case PawnControllerInputUseItem useItemInput:
                ExecuteUseItemInput(useItemInput, pawn);
                break;

            case PawnControllerInputUseInteractable useInteractableInput:
                ExecuteUseInteractableInput(useInteractableInput, pawn);
                break;
        }
    }

    private void ExecuteUseItemInput(PawnControllerInputUseItem inputUseItem, Entity pawn)
    {
        void LogDiscardReason(string str)
        {
            Log.Info($"[{nameof(ExecutePawnControllerInputSystem)}::ExecuteUseItemInput] " +
                $"Discarding input {inputUseItem} : {str}");
        }

        if(!EntityManager.TryGetBuffer(pawn, out DynamicBuffer<InventoryItemReference> inventory))
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

        GameAction gameAction = GetGameActionFromEntity(item);

        if (gameAction == null)
        {
            LogDiscardReason($"Item {item}'s gameActionId is invalid.");
            return;
        }

        GameAction.UseContext useContext = new GameAction.UseContext()
        {
            InstigatorPawn = pawn,
            InstigatorPawnController = inputUseItem.PawnController,
            Entity = item
        };

        if(!gameAction.TryUse(Accessor, useContext, inputUseItem.GameActionData))
        {
            LogDiscardReason($"Can't Trigger {gameAction}");
            return;
        }
    }

    private void ExecuteUseInteractableInput(PawnControllerInputUseInteractable inputUseInteractable, Entity pawn)
    {
        void LogDiscardReason(string str)
        {
            Log.Info($"[{nameof(ExecutePawnControllerInputSystem)}::ExecuteUseInteractableInput] " +
                $"Discarding input {inputUseInteractable} : {str}");
        }

        FixTranslation pawnPosition = EntityManager.GetComponentData<FixTranslation>(pawn);
        fix3 interactablePosition = new fix3(inputUseInteractable.InteractablePosition.x, 
                                             inputUseInteractable.InteractablePosition.y,
                                             0);

        fix distanceBetween = fix3.DistanceSquared(pawnPosition.Value, interactablePosition);
        fix maxDistanceToInteract = (fix)1.1;
        if(distanceBetween > maxDistanceToInteract) // range to interact, hard coded for now
        {
            return;
        }

        Entity tile = CommonReads.GetTileEntity(Accessor, inputUseInteractable.InteractablePosition);
        if(tile == Entity.Null)
        {
            return;
        }

        Entity interactableEntity = CommonReads.GetFirstTileAddonWithComponent<Interactable>(Accessor, tile);
        if (interactableEntity == Entity.Null)
        {
            return;
        }

        GameAction gameAction = GetGameActionFromEntity(interactableEntity);

        if (gameAction == null)
        {
            LogDiscardReason($"Interactable {interactableEntity}'s gameActionId is invalid.");
            return;
        }

        GameAction.UseContext useContext = new GameAction.UseContext()
        {
            InstigatorPawn = pawn,
            InstigatorPawnController = inputUseInteractable.PawnController,
            Entity = interactableEntity
        };

        // currently no use parameters
        if (!gameAction.TryUse(Accessor, useContext, null))
        {
            LogDiscardReason($"Can't Trigger {gameAction}");
            return;
        }
    }

    private GameAction GetGameActionFromEntity(Entity entity)
    {
        if (EntityManager.TryGetComponentData(entity, out GameActionId gameActionId) && gameActionId.IsValid)
        {
            return GameActionBank.GetAction(gameActionId);
        }

        return null;
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
}