using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public partial class UpdateItemCooldownSystem : SimGameSystemBase
{
    EntityQuery _entitiesWithCooldown;

    protected override void OnCreate()
    {
        base.OnCreate();

        var queryDescription = new EntityQueryDesc()
        {
            Any = new ComponentType[] { typeof(ItemCooldownTimeCounter) }
        };
        _entitiesWithCooldown = GetEntityQuery(queryDescription);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var deltaTime = Time.DeltaTime;

        if (HasSingleton<NoCooldownSingletonTag>())
        {
            EntityManager.RemoveComponent(_entitiesWithCooldown, new ComponentTypes(typeof(ItemCooldownTimeCounter)));
        }

        Entities
            .ForEach((Entity item, ref ItemCooldownTimeCounter cooldownCounter) =>
            {
                cooldownCounter.Value -= deltaTime;
                if (cooldownCounter.Value <= 0)
                {
                    ecb.RemoveComponent<ItemCooldownTimeCounter>(item);
                }
            }).Run();

        ecb.Playback(EntityManager);
    }
}
