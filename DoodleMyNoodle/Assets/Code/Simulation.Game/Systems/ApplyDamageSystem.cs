using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using Unity.Collections;

public struct HealthDeltaEventData
{
    public Entity AffectedEntity;
    /// <summary>
    /// Value will be negative when the entity is damaged
    /// </summary>
    public fix TotalUncappedDelta;
    /// <summary>
    /// Value will be negative when the entity is damaged
    /// </summary>
    public fix TotalCappedDelta => ShieldDelta + HPDelta;
    /// <summary>
    /// Value will be negative when the entity is damaged
    /// </summary>
    public fix ShieldDelta;
    /// <summary>
    /// Value will be negative when the entity is damaged
    /// </summary>
    public fix HPDelta;
    public fix2 Position;
    public bool IsHeal => TotalUncappedDelta > 0;
    public bool IsDamage => !IsHeal;
}

public struct DamageRequestSingletonTag : IComponentData
{
}

public struct HealthChangeRequestData : IBufferElementData
{
    public fix Amount;
    public Entity Target;
    public uint EffectGroupID;
}

[AlwaysUpdateSystem]
public class ApplyDamageSystem : SimGameSystemBase
{
    public void RequestHealthChange(HealthChangeRequestData damageRequestData)
    {
        GetDamageRequestBuffer().Add(damageRequestData);
    }

    private DynamicBuffer<HealthChangeRequestData> GetDamageRequestBuffer()
    {
        if (!HasSingleton<DamageRequestSingletonTag>())
        {
            EntityManager.CreateEntity(typeof(DamageRequestSingletonTag), typeof(HealthChangeRequestData));
        }

        return GetBuffer<HealthChangeRequestData>(GetSingletonEntity<DamageRequestSingletonTag>());
    }

    protected override void OnUpdate()
    {
        DynamicBuffer<HealthChangeRequestData> damageRequests = GetDamageRequestBuffer();

        foreach (HealthChangeRequestData healthChangeData in damageRequests)
        {
            ProcessHealthChange(healthChangeData.Target, healthChangeData.Amount, healthChangeData.EffectGroupID);
        }

        damageRequests.Clear();
    }

    private void ProcessHealthChange(Entity target, fix amount, uint effectGroupID)
    {
        fix shieldDelta = 0;
        fix hpDelta = 0;
        fix remainingDelta = amount;

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
            remainingDelta = min(remainingDelta, 0);
        }

        if (remainingDelta != 0 && TryGetComponent(target, out Shield previousShield))
        {
            Shield newShield = clamp(previousShield + remainingDelta, 0, GetComponent<MaximumFix<Shield>>(target).Value);
            shieldDelta = newShield.Value - previousShield.Value;
            SetComponent(target, newShield);
            remainingDelta -= shieldDelta;
        }

        // Health
        if (remainingDelta != 0 && TryGetComponent(target, out Health previousHP))
        {
            Health newHP = clamp(previousHP + remainingDelta, 0, GetComponent<MaximumFix<Health>>(target).Value);
            hpDelta = newHP.Value - previousHP.Value;
            SetComponent(target, newHP);
            remainingDelta -= hpDelta;
        }

        // Trigger Signal on Damage
        if (hpDelta < 0 && shieldDelta < 0 && HasComponent<TriggerSignalOnDamage>(target) && TryGetComponent(target, out SignalEmissionType emissionType))
        {
            if (emissionType.Value == ESignalEmissionType.OnClick || emissionType.Value == ESignalEmissionType.ToggleOnClick)
            {
                World.GetOrCreateSystem<SetSignalSystem>().EmitterClickRequests.Add(target);
            }
        }

        // Handle damaged entity for feedbacks
        if (hpDelta != 0 || shieldDelta != 0)
        {
            PresentationEvents.HealthDeltaEvents.Push(new HealthDeltaEventData()
            {
                AffectedEntity = target,
                TotalUncappedDelta = amount,
                HPDelta = hpDelta,
                ShieldDelta = shieldDelta,
                Position = GetComponent<FixTranslation>(target)
            });
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, NativeArray<DistanceHit> hits, fix amount, uint effectGroupID = uint.MaxValue)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < hits.Length; i++)
        {
            var request = new HealthChangeRequestData() { Amount = -amount, Target = hits[i].Entity, EffectGroupID = effectGroupID };
            sys.RequestHealthChange(request);
        }
    }
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, NativeArray<Entity> targets, fix amount, uint effectGroupID = uint.MaxValue)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < targets.Length; i++)
        {
            var request = new HealthChangeRequestData() { Amount = -amount, Target = targets[i], EffectGroupID = effectGroupID };
            sys.RequestHealthChange(request);
        }
    }

    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity target, fix amount, uint effectGroupID = uint.MaxValue)
    {
        var request = new HealthChangeRequestData() { Amount = -amount, Target = target, EffectGroupID = effectGroupID };
        accessor.GetExistingSystem<ApplyDamageSystem>().RequestHealthChange(request);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, NativeArray<Entity> targets, fix amount, uint effectGroupID = uint.MaxValue)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, targets, -amount, effectGroupID);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, Entity target, fix amount, uint effectGroupID = uint.MaxValue)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, target, -amount, effectGroupID);
    }
}