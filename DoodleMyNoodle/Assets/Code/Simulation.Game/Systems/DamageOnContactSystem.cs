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

            NativeList<Entity> toDestroy = new NativeList<Entity>(Allocator.Temp);
            foreach (Entity otherEntity in collidedEntities)
            {
                ProcessEntityPair(entity, otherEntity, toDestroy);
                ProcessEntityPair(otherEntity, entity, toDestroy);
            }

            if (EntityManager.TryGetComponentData(entity, out DamageOnContact damageOnContact) && damageOnContact.DestroySelf)
            {
                toDestroy.AddUnique(entity);
            }

            foreach (var item in toDestroy)
            {
                PostUpdateCommands.DestroyEntity(item);
            }
        });
    }

    private void ProcessEntityPair(Entity instigator, Entity target, NativeList<Entity> toDestroy)
    {
        if (EntityManager.TryGetComponentData(instigator, out DamageOnContact damageOnContact))
        {
            if (EntityManager.HasComponent<Health>(target))
            {
                CommonWrites.RequestDamageOnTarget(Accessor, instigator, target, damageOnContact.Value);
            }

            if (damageOnContact.DestroySelf)
            {
                toDestroy.AddUnique(instigator);
            }
        }
    }
}
