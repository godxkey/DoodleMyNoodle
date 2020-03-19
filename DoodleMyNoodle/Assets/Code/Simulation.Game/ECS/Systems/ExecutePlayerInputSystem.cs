using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using static Unity.Mathematics.math;

[UpdateAfter(typeof(BeginSimulationEntityCommandBufferSystem))]
public class ExecutePlayerInputSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in SimInputs)
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
            // temporary
            case SimInputKeycode keycodeInput:
            {
                if (EntityManager.TryGetComponentData(playerEntity, out ControlledEntity controlledEntity))
                {
                    Entity pawn = controlledEntity.Value;

                    if (EntityManager.Exists(pawn) && EntityManager.TryGetComponentData(pawn, out InputAcceleration pawnAcceleration))
                    {
                        if (keycodeInput.state == SimInputKeycode.State.Held)
                        {
                            switch (keycodeInput.keyCode)
                            {
                                case UnityEngine.KeyCode.LeftArrow:
                                case UnityEngine.KeyCode.A:
                                    pawnAcceleration.Value += float3(-1, 0, 0);
                                    break;

                                case UnityEngine.KeyCode.RightArrow:
                                case UnityEngine.KeyCode.D:
                                    pawnAcceleration.Value += float3(1, 0, 0);
                                    break;

                                case UnityEngine.KeyCode.UpArrow:
                                case UnityEngine.KeyCode.W:
                                    pawnAcceleration.Value += float3(0, 1, 0);
                                    break;

                                case UnityEngine.KeyCode.DownArrow:
                                case UnityEngine.KeyCode.S:
                                    pawnAcceleration.Value += float3(0, -1, 0);
                                    break;
                            }

                        }
                        EntityManager.SetComponentData(pawn, pawnAcceleration);
                    }
                }

                break;
            }

        }
    }
}
