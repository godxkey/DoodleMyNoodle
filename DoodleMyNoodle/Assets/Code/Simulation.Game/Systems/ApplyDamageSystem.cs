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
    public int Amount;
    public Entity Target;
    public uint EffectGroupID;
}

[AlwaysUpdateSystem]
public class ApplyDamageSystem : SimGameSystemBase
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
                ProcessDamage(damageData.Target, damageData.Amount, damageData.EffectGroupID, _newDamageEvents);
            }
            else if (damageData.Amount < 0)
            {
                ProcessHeal(damageData.Target, abs(damageData.Amount), _newHealEvents);
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

    private void ProcessDamage(Entity target, int amount, uint effectGroupID, List<DamageEventData> outDamageEvents)
    {
        int remainingDamage = amount;
        bool damageHasBeenApplied = false;

        // prevents too many damage instance from the same source/group
        if (effectGroupID != uint.MaxValue)
        {
            DynamicBuffer<EffectGroupBufferSingleton> effectGroupBufferSingleton = GetSingletonBuffer<EffectGroupBufferSingleton>();
            for (int i = 0; i < effectGroupBufferSingleton.Length; i++)
            {
                if (effectGroupBufferSingleton[i].ID == effectGroupID && effectGroupBufferSingleton[i].Entity == target)
                {
                    // can't take damage when we already took damage from a same id effect Group
                    return;
                }
            }

            // hard coded cooldown for effect group 0.1 for now
            effectGroupBufferSingleton.Add(new EffectGroupBufferSingleton() { ID = effectGroupID, Entity = target, TimeStamp = Time.ElapsedTime, Delay = SimulationGameConstants.SameEffectGroupDamageCooldown });
        }

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

    private void ProcessHeal(Entity target, int amount, List<HealEventData> outHealEvents)
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
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, NativeArray<DistanceHit> hits, int damage, uint effectGroupID = uint.MaxValue)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < hits.Length; i++)
        {
            var request = new DamageRequestData() { Amount = damage, Target = hits[i].Entity, EffectGroupID = effectGroupID };
            sys.RequestDamage(request);
        }
    }
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, NativeArray<Entity> targets, int amount, uint effectGroupID = uint.MaxValue)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < targets.Length; i++)
        {
            var request = new DamageRequestData() { Amount = amount, Target = targets[i], EffectGroupID = effectGroupID };
            sys.RequestDamage(request);
        }
    }

    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity target, int amount, uint effectGroupID = uint.MaxValue)
    {
        var request = new DamageRequestData() { Amount = amount, Target = target, EffectGroupID = effectGroupID };
        accessor.GetExistingSystem<ApplyDamageSystem>().RequestDamage(request);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, NativeArray<Entity> targets, int amount, uint effectGroupID = uint.MaxValue)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, targets, -amount, effectGroupID);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, Entity target, int amount, uint effectGroupID = uint.MaxValue)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, target, -amount, effectGroupID);
    }
}