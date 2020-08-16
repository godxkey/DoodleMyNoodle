using System;
using Unity.Entities;
using UnityEngine;
using static fixMath;

public struct EntityDeathEventData : IComponentData
{
    public Entity Entity;
}

public class DestroyDeadEntitiesSystem : SimComponentSystem
{
    EntityQuery _eventsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsEntityQuery = EntityManager.CreateEntityQuery(typeof(EntityDeathEventData));
    }

    protected override void OnUpdate()
    {
        // destroy events
        EntityManager.DestroyEntity(_eventsEntityQuery);

        Entities.ForEach((Entity entity, ref Health health, ref FixTranslation translation) =>
        {
            if (health.Value <= 0)
            {
                // Create Event
                EntityManager.CreateEventEntity(new EntityDeathEventData() { Entity = entity });

                PostUpdateCommands.DestroyEntity(entity);
            }
        });
    }
}
