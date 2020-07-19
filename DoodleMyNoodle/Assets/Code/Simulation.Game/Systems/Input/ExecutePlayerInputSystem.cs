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
                pawnControllerInputSystem.Inputs.Add(new PawnStartingInventorySelectionInput(playerEntity, StartingInventorySelected.KitNumber));
                break;

            case SimPlayerCharacterNameInput NameSelected:
                pawnControllerInputSystem.Inputs.Add(new PawnCharacterNameInput(playerEntity, NameSelected.Name));
                break;

            case SimPlayerInputNextTurn NextTurnInput:
                pawnControllerInputSystem.Inputs.Add(new PawnInputNextTurn(playerEntity, NextTurnInput.ReadyForNextTurn));
                break;

            case SimPlayerInputUseItem ItemUsedInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseItem(playerEntity, ItemUsedInput.ItemIndex, ItemUsedInput.UseData));
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