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
    public Entity InstigatorPawn;
    public int Amount;
    public Entity TargetPawn;
}

[AlwaysUpdateSystem]
public class ApplyDamageSystem : SimComponentSystem
{
    private EntityQuery _eventsEntityQuery;

    private List<Entity> _damagedEntities = new List<Entity>();
    private List<Entity> _healedEntities = new List<Entity>();

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

        _damagedEntities.Clear();
        _healedEntities.Clear();

        foreach (DamageToApplyData damageData in DamageToApplyBuffer)
        {
            if (damageData.Amount > 0)
            {
                ProcessDamage(damageData.InstigatorPawn, damageData.TargetPawn, damageData.Amount, _damagedEntities);
            }
            else if (damageData.Amount < 0)
            {
                ProcessHeal(damageData.InstigatorPawn, damageData.TargetPawn, -damageData.Amount, _healedEntities);
            }
        }

        DamageToApplyBuffer.Clear();

        // We save entities damaged and do this here because we need to clear the buffer and it's a simulation changed
        foreach (Entity entity in _damagedEntities)
        {
            EntityManager.CreateEventEntity(new DamageAppliedEventData() { EntityDamaged = entity });
        }
    }

    private void ProcessDamage(Entity instigator, Entity target, int amount, List<Entity> damagedEntities)
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
            damagedEntities.Add(target);
        }
    }

    private void ProcessHeal(Entity instigator, Entity target, int amount, List<Entity> healedEntities)
    {
        if (EntityManager.HasComponent<Health>(target))
        {
            CommonWrites.ModifyStatInt<Health>(Accessor, target, amount);
            healedEntities.Add(target);
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