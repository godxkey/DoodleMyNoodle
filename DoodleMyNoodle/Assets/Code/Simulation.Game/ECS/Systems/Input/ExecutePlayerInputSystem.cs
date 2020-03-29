using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using static fixMath;

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
            // temporary
            case SimInputKeycode keycodeInput:
            {
                Entity pawn = GetPlayerPawn(playerEntity);

                if (pawn != Entity.Null)
                {
                    if (EntityManager.TryGetComponentData(pawn, out FixTranslation pawnPos))
                    {
                        if (keycodeInput.state == SimInputKeycode.State.Pressed)
                        {
                            switch (keycodeInput.keyCode)
                            {
                                case UnityEngine.KeyCode.LeftArrow:
                                case UnityEngine.KeyCode.A:
                                    EntityManager.SetOrAddComponentData(pawn, new Destination()
                                    {
                                        Value = round(pawnPos.Value) + fix3(-1, 0, 0)
                                    });
                                    break;

                                case UnityEngine.KeyCode.RightArrow:
                                case UnityEngine.KeyCode.D:
                                    EntityManager.SetOrAddComponentData(pawn, new Destination()
                                    {
                                        Value = round(pawnPos.Value) + fix3(1, 0, 0)
                                    });
                                    break;

                                case UnityEngine.KeyCode.UpArrow:
                                case UnityEngine.KeyCode.W:
                                    EntityManager.SetOrAddComponentData(pawn, new Destination()
                                    {
                                        Value = round(pawnPos.Value) + fix3(0, 1, 0)
                                    });
                                    break;

                                case UnityEngine.KeyCode.DownArrow:
                                case UnityEngine.KeyCode.S:
                                    EntityManager.SetOrAddComponentData(pawn, new Destination()
                                    {
                                        Value = round(pawnPos.Value) + fix3(0, -1, 0)
                                    });
                                    break;

                                // Damage Health Debug 
                                case UnityEngine.KeyCode.M:
                                    EntityManager.SetComponentData(pawn, new Health()
                                    {
                                        Value = EntityManager.GetComponentData<Health>(pawn).Value - 1
                                    });
                                    break;

                            }
                        }
                    }

                    if (keycodeInput.keyCode == UnityEngine.KeyCode.T && keycodeInput.state == SimInputKeycode.State.Pressed
                        && EntityManager.HasComponent<FixTranslation>(pawn))
                    {
                        fix2 newPosition = World.Random().NextFixVector2(
                            min: fix2(-5, -5),
                            max: fix2(5, 5));

                        EntityManager.SetComponentData(pawn, new FixTranslation() { Value = fix3(newPosition, 0) });
                    }
                }
                break;
            }
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
