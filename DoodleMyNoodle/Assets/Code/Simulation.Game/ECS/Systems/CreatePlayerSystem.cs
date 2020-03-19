using Unity.Entities;
using Unity.Collections;

public class CreatePlayerSystem : SimComponentSystem
{
    EntityQuery _controllableEntitiesQ;
    EntityQuery _entitieControllersQ;

    protected override void OnCreate()
    {
        base.OnCreate();
        
        _controllableEntitiesQ = EntityManager.CreateEntityQuery(typeof(ControllableTag));
        _entitieControllersQ = EntityManager.CreateEntityQuery(typeof(ControlledEntity));
    }

    protected override void OnUpdate()
    {
        foreach (var input in SimInputs)
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
                if(uncontrolledEntity != Entity.Null)
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
        using (var controlledEntities = _entitieControllersQ.ToComponentDataArray<ControlledEntity>(Allocator.TempJob))
        {
            bool IsControlled(Entity entity)
            {
                for (int i = 0; i < controlledEntities.Length; i++)
                {
                    if (controlledEntities[i].Value == entity)
                    {
                        return true;
                    }
                }
                return false;
            }


            Entities.ForEach((Entity controllableEntity, ref ControllableTag controlledTag) =>
            {
                if (!IsControlled(controllableEntity))
                {
                    uncontrolledEntity = controllableEntity;
                    return;
                }
            });
        }

        return uncontrolledEntity;
    }
}
