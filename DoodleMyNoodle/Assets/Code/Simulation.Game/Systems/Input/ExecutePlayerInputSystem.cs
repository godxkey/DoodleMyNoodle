using System.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static fixMath;
using static Unity.Mathematics.math;

[NetSerializable]
public class SimPlayerInput : SimInput
{
    // this will be assigned by the server when its about to enter the simulation
    public PersistentId SimPlayerId;
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
                Entity playerEntity = FindPlayerEntity(playerInput.SimPlayerId);
                ExecutePlayerInput(playerInput, playerEntity);
            }
        }
    }

    private Entity FindPlayerEntity(PersistentId simPlayerId)
    {
        Entity playerEntity = Entity.Null;
        Entities.ForEach((Entity entity, ref PersistentId id, ref PlayerTag playerTag) =>
        {
            if (id == simPlayerId)
            {
                playerEntity = entity;
                return;
            }
        });

        return playerEntity;
    }

    private void ExecutePlayerInput(SimPlayerInput input, Entity playerEntity)
    {
        // fbessette: For now, we simply do a switch. 
        //            In the future, we'll probably want to implement something dynamic instead

        switch (input)
        {
            case SimPlayerInputNextTurn NextTurnInput:
                if (Accessor.HasSingleton<NextTurnInputCounter>())
                {
                    int currentNextTurnInputCounter = Accessor.GetSingleton<NextTurnInputCounter>().Value;
                    Accessor.SetSingleton(new NextTurnInputCounter() { Value = currentNextTurnInputCounter + 1 });
                }
                else
                {
                    Accessor.SetOrCreateSingleton(new NextTurnInputCounter() { Value = 1 });
                }
                
                break;

            case SimPlayerInputUseItem ItemUsedInput:
                ExecutePawnControllerInputSystem pawnControllerInputSystem = World.GetOrCreateSystem<ExecutePawnControllerInputSystem>();
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