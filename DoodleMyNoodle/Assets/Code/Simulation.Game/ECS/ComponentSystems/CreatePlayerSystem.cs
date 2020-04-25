﻿using Unity.Entities;
using Unity.Collections;

public class CreatePlayerSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in World.TickInputs)
        {
            if (input is SimInputPlayerCreate createPlayerInput)
            {
                var newPlayerEntity = EntityManager.CreateEntity(
                    typeof(PlayerTag),
                    typeof(PersistentId),
                    typeof(Name),
                    typeof(ControlledEntity));

                // set persistent id
                EntityManager.SetComponentData(newPlayerEntity, this.MakeUniquePersistentId());

                // cap player name at 30 characters
                string playerName = createPlayerInput.PlayerName;
                if (playerName.Length > 30)
                    playerName = playerName.Substring(0, 30);

                // set name
                EntityManager.SetComponentData(newPlayerEntity, new Name() { Value = playerName });

                // assign controllable entity if possible
                Entity uncontrolledEntity = FindUncontrolledPawn();
                if (uncontrolledEntity != Entity.Null)
                {
                    EntityManager.SetComponentData(newPlayerEntity, new ControlledEntity() { Value = uncontrolledEntity });
                }

                // FOR DEBUGGING ONLY
#if UNITY_EDITOR
                EntityManager.SetName(newPlayerEntity, $"Player({playerName})");
#endif
            }
        }
    }

    private Entity FindUncontrolledPawn()
    {
        Entity uncontrolledEntity = Entity.Null;

        Entities
            .WithNone<InstantiateAndUseDefaultControllerTag>() // entities with this tag will have their DefaultController spawned
            .WithAll<ControllableTag>()
            .ForEach((Entity controllableEntity) =>
        {
            if (!CommonReads.IsPawnControlled(Accessor, controllableEntity))
            {
                uncontrolledEntity = controllableEntity;
                return;
            }
        });

        return uncontrolledEntity;
    }
}

public partial class CommonReads
{
    public static Entity GetPawnController(ISimWorldReadAccessor accessor, Entity pawn)
    {
        Entity result = Entity.Null;
        accessor.Entities.ForEach((Entity controller, ref ControlledEntity x) =>
        {
            if (pawn == x.Value)
            {
                result = controller;
                return;
            }
        });

        return result;
    }

    public static bool IsPawnControlled(ISimWorldReadAccessor accessor, Entity pawn)
    {
        return GetPawnController(accessor, pawn) != Entity.Null;
    }
}
