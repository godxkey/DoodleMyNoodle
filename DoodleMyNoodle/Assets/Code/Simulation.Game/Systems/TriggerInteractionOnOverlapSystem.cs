using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class TriggerInteractionOnOverlapSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref TileActorOverlapBeginEventData overlapEvent) =>
        {
            OnEntityOverlap(overlapEvent.TileActorA, overlapEvent.TileActorB);
            OnEntityOverlap(overlapEvent.TileActorB, overlapEvent.TileActorA);
        });
    }

    private void OnEntityOverlap(Entity a, Entity b)
    {
        if (EntityManager.HasComponent<Controllable>(a) &&
            EntityManager.HasComponent<InteractOnOverlapTag>(b) &&
            EntityManager.HasComponent<Interactable>(b))
        {
            CommonWrites.Interact(Accessor, b, a);
        }
    }
}

internal partial class CommonWrites
{
    public static void Interact(ISimWorldReadWriteAccessor accessor, Entity interactableEntity, Entity Instigator)
    {
        if (accessor.HasComponent<NoInteractTimer>(interactableEntity))
        {
            NoInteractTimer interactableTimer = accessor.GetComponentData<NoInteractTimer>(interactableEntity);
            fix endTime = accessor.Time.ElapsedTime + interactableTimer.Duration;
            accessor.SetComponentData(interactableEntity, new NoInteractTimer() { Duration = interactableTimer.Duration, EndTime = endTime, CanCountdown = true });
        }

        accessor.SetOrAddComponentData(interactableEntity, new Interacted() { Value = true, Instigator = Instigator });
    }
}