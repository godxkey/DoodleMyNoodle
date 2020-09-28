using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct DamageAppliedEventData : IComponentData
{
    public Entity EntityDamaged;
    public int DamageApplied;
}

public struct HealingAppliedEventData : IComponentData
{
    public Entity EntityHealed;
    public int HealApplied;
}

public struct DamageToApplySingletonTag : IComponentData
{
}

public struct DamageToApplyData : IBufferElementData
{
    public Entity InstigatorPawn;
    public int Amount;
    public Entity TargetPawn;
}

[AlwaysUpdateSystem]
public class ApplyDamageSystem : SimComponentSystem
{
    private EntityQuery _damageEventsEntityQuery;
    private EntityQuery _healingEventsEntityQuery;

    private List<DamageAppliedEventData> _damagedEventData = new List<DamageAppliedEventData>();
    private List<HealingAppliedEventData> _healedEntities = new List<HealingAppliedEventData>();

    protected override void OnCreate()
    {
        base.OnCreate();

        _damageEventsEntityQuery = EntityManager.CreateEntityQuery(typeof(DamageAppliedEventData));
        _healingEventsEntityQuery = EntityManager.CreateEntityQuery(typeof(HealingAppliedEventData));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _damageEventsEntityQuery.Dispose();
        _healingEventsEntityQuery.Dispose();
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
        EntityManager.DestroyEntity(_damageEventsEntityQuery);
        EntityManager.DestroyEntity(_healingEventsEntityQuery);

        DynamicBuffer<DamageToApplyData> DamageToApplyBuffer = GetDamageToApplySingletonBuffer(Accessor);

        _damagedEventData.Clear();
        _healedEntities.Clear();

        foreach (DamageToApplyData damageData in DamageToApplyBuffer)
        {
            if (damageData.Amount > 0)
            {
                ProcessDamage(damageData.InstigatorPawn, damageData.TargetPawn, damageData.Amount, _damagedEventData);
            }
            else if (damageData.Amount < 0)
            {
                ProcessHeal(damageData.InstigatorPawn, damageData.TargetPawn, Math.Abs(damageData.Amount), _healedEntities);
            }
        }

        DamageToApplyBuffer.Clear();

        // We save entities damaged and do this here because we need to clear the buffer and it's a simulation changed
        foreach (DamageAppliedEventData damageAppliedEventData in _damagedEventData)
        {
            EntityManager.CreateEventEntity(damageAppliedEventData);
        }

        foreach (HealingAppliedEventData healAppliedEventData in _healedEntities)
        {
            EntityManager.CreateEventEntity(healAppliedEventData);
        }
    }

    private void ProcessDamage(Entity instigator, Entity target, int amount, List<DamageAppliedEventData> damagedEntities)
    {
        int remainingDamage = amount;
        bool damageHasBeenApplied = false;

        // Invincible
        if (EntityManager.HasComponent<Invincible>(target))
        {
            remainingDamage = 0;
        }

        // Armor
        if (remainingDamage > 0 && EntityManager.TryGetComponentData(target, out Armor armor))
        {
            CommonWrites.ModifyStatInt<Armor>(Accessor, target, -remainingDamage);
            remainingDamage -= armor.Value;
            damageHasBeenApplied = true;
        }

        // Health
        if (remainingDamage > 0 && EntityManager.TryGetComponentData(target, out Health health))
        {
            CommonWrites.ModifyStatInt<Health>(Accessor, target, -remainingDamage);
            remainingDamage -= health.Value;
            damageHasBeenApplied = true;
        }

        // Handle damaged entity for feedbacks
        if (damageHasBeenApplied)
        {
            damagedEntities.Add(new DamageAppliedEventData() { EntityDamaged = target, DamageApplied = amount } );
        }
    }

    private void ProcessHeal(Entity instigator, Entity target, int amount, List<HealingAppliedEventData> healedEntities)
    {
        if (EntityManager.HasComponent<Health>(target))
        {
            CommonWrites.ModifyStatInt<Health>(Accessor, target, amount);
            healedEntities.Add(new HealingAppliedEventData() { EntityHealed = target, HealApplied = amount });
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestDamageOnTarget(ISimWorldReadWriteAccessor accessor, Entity instigatorPawn, Entity targetPawn, int amount)
    {
        DynamicBuffer<DamageToApplyData> damageDataBuffer = ApplyDamageSystem.GetDamageToApplySingletonBuffer(accessor);

        damageDataBuffer.Add(new DamageToApplyData() { Amount = amount, InstigatorPawn = instigatorPawn, TargetPawn = targetPawn });
    }

    public static void RequestHealOnTarget(ISimWorldReadWriteAccessor accessor, Entity instigatorPawn, Entity targetPawn, int amount)
    {
        // for now a heal request is a negative damage request
        RequestDamageOnTarget(accessor, instigatorPawn, targetPawn, -amount);
    }
}