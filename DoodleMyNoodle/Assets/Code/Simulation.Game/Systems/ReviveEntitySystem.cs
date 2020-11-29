using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct ReviveEntityEventData : IComponentData
{
    public Entity Entity;
}

public class ReviveEntitySystem : SimComponentSystem
{
    EntityQuery _eventsGroup;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsGroup = EntityManager.CreateEntityQuery(typeof(ReviveEntityEventData));
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

        Entities.ForEach((Entity entity, ref Health health, ref DeadTag dead, ref FixTranslation translation) =>
        {
            if (health.Value > 0)
            {
                // Create Event
                EntityManager.CreateEventEntity(new ReviveEntityEventData() { Entity = entity });

                PostUpdateCommands.RemoveComponent<DeadTag>(entity);
            }
        });
    }
}