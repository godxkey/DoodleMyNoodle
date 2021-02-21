using CCC.Fix2D;
using Unity.Entities;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class ResetDestinationOnCollideSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // if entities collide when moving along path, reset their destination
        Entities.ForEach((ref TileCollisionEventData collisionEvent) =>
        {
            if (EntityManager.HasComponent<PathPosition>(collisionEvent.Entity))
            {
                fix2 entityPos = EntityManager.GetComponentData<FixTranslation>(collisionEvent.Entity);
                fix2 newDestination = Helpers.GetTileCenter(entityPos);
                EntityManager.SetOrAddComponentData(collisionEvent.Entity, new Destination() { Value = newDestination });
            }
        });
    }
}