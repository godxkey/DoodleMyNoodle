using Unity.Entities;
using Unity.Transforms;

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
                else if (EntityManager.TryGetComponentData(instigator, out ExplodeOnContact explosionData))
                {
                    CommonWrites.RequestExplosionOnTiles(Accessor, instigator, collisionEvent.Tile, explosionData.Range, explosionData.Damage);
                    EntityManager.DestroyEntity(instigator);
                }
            });
    }
}