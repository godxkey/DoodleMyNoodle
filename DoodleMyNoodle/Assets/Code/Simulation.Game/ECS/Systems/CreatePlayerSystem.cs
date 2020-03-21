using Unity.Entities;
using Unity.Collections;

public class CreatePlayerSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        foreach (var input in SimWorld.TickInputs)
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

        Entities.ForEach((Entity controllableEntity, ref ControllableTag controlledTag) =>
        {
            if (!IsEntityControlled(controllableEntity))
            {
                uncontrolledEntity = controllableEntity;
                return;
            }
        });

        return uncontrolledEntity;
    }

    private bool IsEntityControlled(Entity entity)
    {
        bool result = false;
        Entities.ForEach((ref ControlledEntity x) =>
        {
            if (entity == x.Value)
            {
                result = true;
                return;
            }
        });

        return result;
    }
}
