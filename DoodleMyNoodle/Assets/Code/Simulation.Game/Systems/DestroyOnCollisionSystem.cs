using Unity.Entities;

public class DestroyOnCollisionSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities
            .ForEach((ref TileCollisionEventData collisionEvent) =>
            {
                Entity instigator = collisionEvent.Entity;
                if (EntityManager.HasComponent<DestroyOnCollisionTag>(instigator))
                {
                    EntityManager.DestroyEntity(instigator);
                }
            });
    }
}