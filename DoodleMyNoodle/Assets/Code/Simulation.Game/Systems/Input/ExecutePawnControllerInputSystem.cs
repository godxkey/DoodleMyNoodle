using System.Collections.Generic;
using Unity.Entities;
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
        switch (input)
        {
            case PawnStartingInventorySelectionInput equipItemInput:
                Entities
                    .WithAll<ItemKitTag>()
                    .ForEach((DynamicBuffer<InventoryItemPrefabReference> inventoryItems, ref SimAssetId assetID) =>
                {
                    if (equipItemInput.KitNumber == assetID.Value)
                    {
                        ControlledEntity pawn = EntityManager.GetComponentData<ControlledEntity>(equipItemInput.PawnController);

                        if (EntityManager.Exists(pawn.Value))
                        {
                            CommonWrites.CopyToEntityInventory(Accessor, pawn.Value, inventoryItems);
                        }
                    }
                });
                break;

            case PawnInputNextTurn pawnInputNextTurn:
                EntityManager.SetOrAddComponentData(pawnInputNextTurn.PawnController, new ReadyForNextTurn() { Value = pawnInputNextTurn.ReadyForNextTurn });
                break;

            case PawnControllerInputUseItem useItemInput:
                ExecuteInput(useItemInput);
                break;
        }
    }

    private void ExecuteInput(PawnControllerInputUseItem inputUseItem)
    {
        void LogDiscardReason(string str)
        {
            Log.Info($"[{nameof(ExecutePawnControllerInputSystem)}::ExecuteInput] " +
                $"Discarding input {inputUseItem} : {str}");
        }

        if (!EntityManager.TryGetComponentData(inputUseItem.PawnController, out ControlledEntity controlledEntity))
        {
            LogDiscardReason($"PawnController has no {nameof(ControlledEntity)} component.");
            return;
        }

        Entity pawn = controlledEntity.Value;

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

        if (!EntityManager.TryGetComponentData(item, out GameActionId gameActionId))
        {
            LogDiscardReason($"Item {item} doesn't have a {nameof(GameActionId)} component.");
            return;
        }

        if (!gameActionId.IsValid)
        {
            LogDiscardReason($"Item {item}'s gameActionId is invalid.");
            return;
        }

        GameAction gameAction = GameActionBank.GetAction(gameActionId);

        if (gameAction == null)
        {
            LogDiscardReason($"Item {item}'s gameActionId is invalid.");
            return;
        }

        GameAction.UseContext useContext = new GameAction.UseContext()
        {
            InstigatorPawn = pawn,
            InstigatorPawnController = inputUseItem.PawnController,
            ItemEntity = item
        };

        if (!gameAction.TryUse(Accessor, useContext, inputUseItem.GameActionData))
        {
            LogDiscardReason($"Can't Trigger {gameAction}");
            return;
        }
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