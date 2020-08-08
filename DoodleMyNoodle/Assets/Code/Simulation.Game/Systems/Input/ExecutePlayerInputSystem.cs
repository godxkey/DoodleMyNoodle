using Unity.Entities;

[NetSerializable(baseClass = true)]
public abstract class SimPlayerInput : SimInput
{
    // this will be assigned by the server when its about to enter the simulation
    public PersistentId SimPlayerId;

    public override string ToString()
    {
        return $"{GetType().Name}(player:{SimPlayerId.Value})";
    }
}

[UpdateBefore(typeof(ExecutePawnControllerInputSystem))]
public class ExecutePlayerInputSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in World.TickInputs)
        {
            if (input is SimPlayerInput playerInput)
            {
                Entity playerEntity = CommonReads.FindPlayerEntity(Accessor, playerInput.SimPlayerId);
                ExecutePlayerInput(playerInput, playerEntity);
            }
        }
    }

    private void ExecutePlayerInput(SimPlayerInput input, Entity playerEntity)
    {
        // fbessette: For now, we simply do a switch. 
        //            In the future, we'll probably want to implement something dynamic instead
        ExecutePawnControllerInputSystem pawnControllerInputSystem = World.GetOrCreateSystem<ExecutePawnControllerInputSystem>();
        switch (input)
        {
            case SimPlayerStartingInventorySelectionInput StartingInventorySelected:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputStartingInventorySelection(playerEntity, StartingInventorySelected.KitNumber));
                break;

            case SimPlayerCharacterNameInput NameSelected:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputCharacterName(playerEntity, NameSelected.Name));
                break;

            case SimPlayerInputNextTurn NextTurnInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputNextTurn(playerEntity, NextTurnInput.ReadyForNextTurn));
                break;

            case SimPlayerInputUseItem ItemUsedInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseItem(playerEntity, ItemUsedInput.ItemIndex, ItemUsedInput.UseData));
                break;

            case SimPlayerInputUseInteractable InteractableUsedInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseInteractable(playerEntity, InteractableUsedInput.InteractablePosition));
                break;

            case SimPlayerInputSetPawnDoodle setPawnDoodleInput:
            {
                Entity pawn = GetPlayerPawn(playerEntity);
                if(pawn != Entity.Null)
                {
                    EntityManager.SetOrAddComponentData<DoodleId>(pawn, new DoodleId() { Guid = setPawnDoodleInput.DoodleId });
                }
                break;
            }
            case SimPlayerInputEquipItem EquipItemInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputEquipItem(playerEntity, EquipItemInput.ItemIndex, EquipItemInput.ItemEntityPosition));
                break;

            case SimPlayerInputDropItem DropItemInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputDropItem(playerEntity, DropItemInput.ItemIndex));
                break;
        }
    }

    private Entity GetPlayerPawn(Entity playerEntity)
    {
        if (EntityManager.TryGetComponentData(playerEntity, out ControlledEntity controlledEntity))
        {
            Entity pawn = controlledEntity.Value;

            if (EntityManager.Exists(pawn))
            {
                return pawn;
            }
        }

        return Entity.Null;
    }
}

public partial class CommonReads
{
    public static Entity FindPlayerEntity(ISimWorldReadAccessor readAccessor, PersistentId simPlayerId)
    {
        Entity playerEntity = Entity.Null;
        readAccessor.Entities.ForEach((Entity entity, ref PersistentId id, ref PlayerTag playerTag) =>
        {
            if (id == simPlayerId)
            {
                playerEntity = entity;
                return;
            }
        });

        return playerEntity;
    }
}