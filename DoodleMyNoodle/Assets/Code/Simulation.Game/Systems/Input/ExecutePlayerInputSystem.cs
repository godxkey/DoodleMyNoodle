using CCC.Fix2D;
using Unity.Entities;

[NetSerializable(IsBaseClass = true)]
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
[UpdateInGroup(typeof(InputSystemGroup))]
public class ExecutePlayerInputSystem : SimSystemBase
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
            case SimPlayerInputSetPawnName setNameInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputSetPawnName(playerEntity, setNameInput.Name));
                break;

            case SimPlayerInputNextTurn nextTurnInput:
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputNextTurn(playerEntity, nextTurnInput.ReadyForNextTurn));
                break;

            case SimPlayerInputUseItem useItemInput:
            {
                // temporary until we can send entities via sim inputs
                foreach (var item in useItemInput.UseData.ParameterDatas)
                {
                    if (item is GameActionParameterEntity.Data entityData)
                    {
                        entityData.Entity = ConvertPositionToEntity(entityData.EntityPos);
                    }
                }
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseItem(playerEntity, useItemInput.ItemIndex, useItemInput.UseData));
                break;
            }

            case SimPlayerInputClickSignalEmitter clickSignalEmitter:
            {
                Entity emitter = ConvertPositionToEntity(clickSignalEmitter.EmitterPosition);
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputClickSignalEmitter(playerEntity, emitter));
                break;
            }

            case SimPlayerInputUseObjectGameAction useGameActionInput:
            {
                // temporary until we can send entities via sim inputs
                foreach (var item in useGameActionInput.UseData.ParameterDatas)
                {
                    if (item is GameActionParameterEntity.Data entityData)
                    {
                        entityData.Entity = ConvertPositionToEntity(entityData.EntityPos);
                    }
                }
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputUseObjectGameAction(playerEntity, useGameActionInput.ObjectPosition, useGameActionInput.UseData));
                break;
            }
            case SimPlayerInputSetPawnDoodle setPawnDoodleInput:
            {
                Entity pawn = GetPlayerPawn(playerEntity);
                if (pawn != Entity.Null)
                {
                    EntityManager.SetOrAddComponentData(pawn, new DoodleId() { Guid = setPawnDoodleInput.DoodleId });
                    EntityManager.SetOrAddComponentData(pawn, new DoodleStartDirection() { IsLookingRight = setPawnDoodleInput.DoodleDirectionIsLookingRight });
                }
                break;
            }
            case SimPlayerInputEquipItem equipItemInput:
            {
                Entity chest = ConvertPositionToEntity(equipItemInput.ItemEntityPosition);
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputEquipItem(playerEntity, equipItemInput.ItemIndex, chest));
                break;
            }
            case SimPlayerInputDropItem dropItemInput:
            {
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputDropItem(playerEntity, dropItemInput.ItemIndex));
                break;
            }
            case SimPlayerInputMovingCharacter movingCharacterInput:
            {
                pawnControllerInputSystem.Inputs.Add(new PawnControllerInputMovingCharacter(playerEntity, movingCharacterInput.Direction));
                break;
            }

        }
    }

    private Entity ConvertPositionToEntity(fix2 position)
    {
        Entity bestEntity = Entity.Null;
        fix bestDistanceSq = fix.MaxValue;
        
        fix minDistance = (fix)0.1f;
        minDistance *= minDistance;

        Entities.ForEach((Entity entity, in FixTranslation pos) =>
        {
            fix distSq = fixMath.distancesq(pos, position);
            if(distSq < bestDistanceSq && distSq < minDistance)
            {
                bestDistanceSq = distSq;
                bestEntity = entity;
            }
        }).Run();
        
        return bestEntity;
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
        Entity result = Entity.Null;
        readAccessor.Entities
                    .WithAll<PlayerTag>()
                    .ForEach((Entity controller, ref PersistentId id) =>
                    {
                        if (id.Value == simPlayerId.Value)
                        {
                            result = controller;
                            return;
                        }
                    });

        return result;
    }
}