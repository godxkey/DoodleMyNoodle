using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DamageOnContactSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref TileCollisionEventData collisionEvent) =>
        {
            int2 tile = collisionEvent.Tile;
            Entity entity = collisionEvent.Entity;

            // find all entities we have collided with
            NativeList<Entity> collidedEntities = new NativeList<Entity>(Allocator.Temp);
            Entities.WithAny<Health, DamageOnContact>()
                .ForEach((Entity otherEntity, ref FixTranslation pos) =>
                {
                    if (entity != otherEntity && Helpers.GetTile(pos).Equals(tile))
                    {
                        collidedEntities.Add(otherEntity);
                    }
                });

            foreach (Entity otherEntity in collidedEntities)
            {
                DebugService.Log($"Collision between {EntityManager.GetName(entity)} and {EntityManager.GetName(otherEntity)}");
                ProcessEntityPair(entity, otherEntity);
                ProcessEntityPair(otherEntity, entity);
            }
        });
    }

    private void ProcessEntityPair(Entity instigator, Entity target)
    {
        if (EntityManager.TryGetComponentData(instigator, out DamageOnContact damageOnContact) &&
            EntityManager.HasComponent<Health>(target))
        {
            CommonWrites.ModifyStatInt<Health>(Accessor, target, -damageOnContact.Value);
        }
    }
}
