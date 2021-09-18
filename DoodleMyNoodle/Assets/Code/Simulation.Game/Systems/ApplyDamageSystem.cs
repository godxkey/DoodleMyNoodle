using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using Unity.Collections;

public struct DamageEventData : IComponentData
{
    public Entity EntityDamaged;
    public int DamageApplied;
    public fix2 Position;
}

public struct HealEventData : IComponentData
{
    public Entity EntityHealed;
    public int HealApplied;
    public fix2 Position;
}

public struct DamageRequestSingletonTag : IComponentData
{
}

public struct DamageRequestData : IBufferElementData
{
    public Entity InstigatorPawn;
    public int Amount;
    public Entity TargetPawn;
}

[AlwaysUpdateSystem]
public class ApplyDamageSystem : SimSystemBase
{
    private EntityQuery _damageEvents;
    private EntityQuery _healingEvents;

    private List<DamageEventData> _newDamageEvents = new List<DamageEventData>();
    private List<HealEventData> _newHealEvents = new List<HealEventData>();

    protected override void OnCreate()
    {
        base.OnCreate();

        _damageEvents = GetEntityQuery(typeof(DamageEventData));
        _healingEvents = GetEntityQuery(typeof(HealEventData));
    }

    public void RequestDamage(DamageRequestData damageRequestData)
    {
        GetDamageRequestBuffer().Add(damageRequestData);
    }

    private DynamicBuffer<DamageRequestData> GetDamageRequestBuffer()
    {
        if (!HasSingleton<DamageRequestSingletonTag>())
        {
            EntityManager.CreateEntity(typeof(DamageRequestSingletonTag), typeof(DamageRequestData));
        }

        return GetBuffer<DamageRequestData>(GetSingletonEntity<DamageRequestSingletonTag>());
    }

    protected override void OnUpdate()
    {
        // Clear Damage Applied Events
        EntityManager.DestroyEntity(_damageEvents);
        EntityManager.DestroyEntity(_healingEvents);

        DynamicBuffer<DamageRequestData> damageRequests = GetDamageRequestBuffer();

        foreach (DamageRequestData damageData in damageRequests)
        {
            if (damageData.Amount > 0)
            {
                ProcessDamage(damageData.InstigatorPawn, damageData.TargetPawn, damageData.Amount, _newDamageEvents);
            }
            else if (damageData.Amount < 0)
            {
                ProcessHeal(damageData.InstigatorPawn, damageData.TargetPawn, abs(damageData.Amount), _newHealEvents);
            }
        }

        damageRequests.Clear();

        // We save entities damaged and do this here because we need to clear the buffer and it's a simulation changed
        foreach (DamageEventData damageAppliedEventData in _newDamageEvents)
        {
            EntityManager.CreateEventEntity(damageAppliedEventData);
        }

        foreach (HealEventData healAppliedEventData in _newHealEvents)
        {
            EntityManager.CreateEventEntity(healAppliedEventData);
        }

        _newDamageEvents.Clear();
        _newHealEvents.Clear();
    }

    private void ProcessDamage(Entity instigator, Entity target, int amount, List<DamageEventData> outDamageEvents)
    {
        int remainingDamage = amount;
        bool damageHasBeenApplied = false;

        // Invincible
        if (HasComponent<Invincible>(target))
        {
            remainingDamage = 0;
        }

        // Armor
        if (remainingDamage > 0 && TryGetComponent(target, out Armor armor))
        {
            CommonWrites.ModifyStatInt<Armor>(Accessor, target, -remainingDamage);
            remainingDamage -= armor.Value;
            damageHasBeenApplied = true;
        }

        // Health
        if (remainingDamage > 0 && TryGetComponent(target, out Health health))
        {
            CommonWrites.ModifyStatInt<Health>(Accessor, target, -remainingDamage);
            remainingDamage -= health.Value;
            damageHasBeenApplied = true;
        }

        // Trigger Signal on Damage
        if (HasComponent<TriggerSignalOnDamage>(target) && TryGetComponent(target, out SignalEmissionType emissionType))
        {
            if (emissionType.Value == ESignalEmissionType.OnClick || emissionType.Value == ESignalEmissionType.ToggleOnClick)
            {
                World.GetOrCreateSystem<SetSignalSystem>().EmitterClickRequests.Add(target);
            }
        }

        // Handle damaged entity for feedbacks
        if (damageHasBeenApplied)
        {
            outDamageEvents.Add(new DamageEventData()
            {
                EntityDamaged = target,
                DamageApplied = amount,
                Position = GetComponent<FixTranslation>(target)
            });
        }
    }

    private void ProcessHeal(Entity instigator, Entity target, int amount, List<HealEventData> outHealEvents)
    {
        if (HasComponent<Health>(target))
        {
            CommonWrites.ModifyStatInt<Health>(Accessor, target, amount);

            outHealEvents.Add(new HealEventData()
            {
                EntityHealed = target,
                HealApplied = amount,
                Position = GetComponent<FixTranslation>(target)
            });
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestDamage(ISimWorldReadWriteAccessor accessor, Entity instigatorPawn, NativeArray<DistanceHit> hits, int damage)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < hits.Length; i++)
        {
            var request = new DamageRequestData() { Amount = damage, InstigatorPawn = instigatorPawn, TargetPawn = hits[i].Entity };
            sys.RequestDamage(request);
        }
    }
    public static void RequestDamage(ISimWorldReadWriteAccessor accessor, Entity instigatorPawn, NativeArray<Entity> targetPawns, int amount)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < targetPawns.Length; i++)
        {
            var request = new DamageRequestData() { Amount = amount, InstigatorPawn = instigatorPawn, TargetPawn = targetPawns[i] };
            sys.RequestDamage(request);
        }
    }

    public static void RequestDamage(ISimWorldReadWriteAccessor accessor, Entity instigatorPawn, Entity targetPawn, int amount)
    {
        var request = new DamageRequestData() { Amount = amount, InstigatorPawn = instigatorPawn, TargetPawn = targetPawn };
        accessor.GetExistingSystem<ApplyDamageSystem>().RequestDamage(request);
    }

    public static void RequestHeal(ISimWorldReadWriteAccessor accessor, Entity instigatorPawn, NativeArray<Entity> targetPawns, int amount)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, instigatorPawn, targetPawns, -amount);
    }

    public static void RequestHeal(ISimWorldReadWriteAccessor accessor, Entity instigatorPawn, Entity targetPawn, int amount)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, instigatorPawn, targetPawn, -amount);
    }
}