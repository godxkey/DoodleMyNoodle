using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct DamageAppliedEventData : IComponentData
{
    public Entity EntityDamaged;
}

public struct DamageToApplySingletonTag : IComponentData
{
}

public struct DamageToApplyData : IBufferElementData
{
    public Entity Instigator;
    public int Amount;
    public Entity Target;
}

[AlwaysUpdateSystem]
public class ApplyDamageSystem : SimComponentSystem
{
    EntityQuery _eventsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsEntityQuery = EntityManager.CreateEntityQuery(typeof(DamageAppliedEventData));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _eventsEntityQuery.Dispose();
    }

    public static DynamicBuffer<DamageToApplyData> GetDamageToApplySingletonBuffer(ISimWorldReadWriteAccessor accessor)
    {
        if (!accessor.HasSingleton<DamageToApplySingletonTag>())
        {
            accessor.CreateEntity(typeof(DamageToApplySingletonTag), typeof(DamageToApplyData));
        }

        return accessor.GetBuffer<DamageToApplyData>(accessor.GetSingletonEntity<DamageToApplySingletonTag>());
    }

    protected override void OnUpdate()
    {
        // Clear Damage Applied Events
        EntityManager.DestroyEntity(_eventsEntityQuery);

        DynamicBuffer<DamageToApplyData> DamageToApplyBuffer = GetDamageToApplySingletonBuffer(Accessor);

        List<Entity> damagedEntity = new List<Entity>();

        foreach (DamageToApplyData damageData in DamageToApplyBuffer)
        {
            int remainingDamage = damageData.Amount;
            Entity target = damageData.Target;
            bool damageHasBeenApplied = false;

            // Invincible
            if (Accessor.HasComponent<Invincible>(target))
            {
                remainingDamage = 0;
            }

            // Armor
            if (remainingDamage > 0 && Accessor.TryGetComponentData(target, out Armor armor))
            {
                CommonWrites.ModifyStatInt<Armor>(Accessor, target, -remainingDamage);
                remainingDamage -= armor.Value;
                damageHasBeenApplied = true;
            }

            // Health
            if (remainingDamage > 0 && Accessor.TryGetComponentData(target, out Health health))
            {
                CommonWrites.ModifyStatInt<Health>(Accessor, target, -remainingDamage);
                remainingDamage -= health.Value;
                damageHasBeenApplied = true;
            }

            // Handle damaged entity for feedbacks
            if (damageHasBeenApplied)
            {
                damagedEntity.Add(target);
            }
        }

        DamageToApplyBuffer.Clear();

        // We save entities damaged and do this here because we need to clear the buffer and it's a simulation changed
        foreach (Entity entity in damagedEntity)
        {
            EntityManager.CreateEventEntity(new DamageAppliedEventData() { EntityDamaged = entity });
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestDamageOnTarget(ISimWorldReadWriteAccessor accessor, Entity instigator, Entity target, int amount)
    {
        DynamicBuffer<DamageToApplyData> damageDataBuffer = ApplyDamageSystem.GetDamageToApplySingletonBuffer(accessor);

        damageDataBuffer.Add(new DamageToApplyData() { Amount = amount, Instigator = instigator, Target = target });
    }
}