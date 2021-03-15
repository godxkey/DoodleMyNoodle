using System;
using Unity.Entities;
using UnityEngine;
using static fixMath;
using System.Collections.Generic;
using CCC.Fix2D;

public struct EntityDeathEventData : IComponentData
{
    public Entity Entity;
}

public class DestroyDeadEntitiesSystem : SimSystemBase
{
    private EntityQuery _eventsGroup;
    private List<Entity> _toDestroy = new List<Entity>();

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsGroup = EntityManager.CreateEntityQuery(typeof(EntityDeathEventData));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _eventsGroup.Dispose();
    }

    protected override void OnUpdate()
    {
        // destroy events
        EntityManager.DestroyEntity(_eventsGroup);

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Health health, ref FixTranslation translation) =>
        {
            if (health.Value <= 0)
            {
                // Create Event
                EntityManager.CreateEventEntity(new EntityDeathEventData() { Entity = entity });

                // Only Destroy entities that have been tagged for it
                if (EntityManager.TryGetComponentData(entity, out DestroyOnDeath destroyOnDeath))
                {
                    _toDestroy.Add(entity);
                }
                else
                {
                    // if not destroyed, play a death animation
                    CommonWrites.SetEntityAnimation(Accessor, entity, new KeyValuePair<string, object>("Type", (int)CommonReads.AnimationTypes.Death));

                    EntityManager.AddComponentData(entity, new DeadTag());
                }
            }
        }).Run();

        foreach (var entity in _toDestroy)
        {
            EntityManager.DestroyEntity(entity);
        }
        _toDestroy.Clear();
    }
}
