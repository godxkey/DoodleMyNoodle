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
    public Entity LastPhysicalInstigator;
    public Entity FirstPhysicalInstigator;
    public uint EffectGroupID;
}

[AlwaysUpdateSystem]
[UpdateBefore(typeof(ExecuteGameActionSystem))]
public class ApplyDamageSystem : SimGameSystemBase
{
    private ExecuteGameActionSystem _gameActionSystem;
    private NativeList<HealthChangeRequestData> _processingHealthChanges;

    protected override void OnCreate()
    {
        base.OnCreate();

        _gameActionSystem = World.GetOrCreateSystem<ExecuteGameActionSystem>();
        _processingHealthChanges = new NativeList<HealthChangeRequestData>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _processingHealthChanges.Dispose();
    }

    protected override void OnUpdate()
    {
        DynamicBuffer<HealthChangeRequestData> damageRequests = GetDamageRequestBuffer();
        _processingHealthChanges.CopyFrom(damageRequests.AsNativeArray());
        damageRequests.Clear();

        foreach (HealthChangeRequestData healthChangeData in _processingHealthChanges)
        {
            ProcessHealthChange(healthChangeData.Target, healthChangeData.LastPhysicalInstigator, healthChangeData.FirstPhysicalInstigator, healthChangeData.Amount, healthChangeData.EffectGroupID);
        }
    }

    private void ProcessHealthChange(Entity target, Entity lastPhysicalInstigator, Entity firstPhyisicalInstigator, fix amount, uint effectGroupID)
    {
        fix shieldDelta = 0;
        fix hpDelta = 0;
        fix remainingDelta = amount;
        fix totalAmountUncapped = amount;

        // find who should be really targetted if there is a HealthProxy component. This is notably used by players since they all share the same health pool
        while (HasComponent<HealthProxy>(target))
        {
            var newTarget = GetComponent<HealthProxy>(target);
            if (newTarget == Entity.Null)
                break;

            target = newTarget;
        }

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

        // If delta is negative (damage), check for invincible
        if (remainingDelta < 0 && TryGetComponent(target, out InvincibleUntilTime invincibleUntilTime) && invincibleUntilTime.Time > Time.ElapsedTime)
        {
            remainingDelta = max(remainingDelta, 0);
        }

        // Damage multipliers for both the instigator that started the attack and the entity applying the attack (ex: pawn shooting arrow)
        if (lastPhysicalInstigator != Entity.Null && remainingDelta < 0 && TryGetComponent(lastPhysicalInstigator, out DamageMultiplier lastDamageMultiplier))
        {
            remainingDelta *= lastDamageMultiplier.Value;
            totalAmountUncapped *= lastDamageMultiplier.Value;
        }

        if (firstPhyisicalInstigator != Entity.Null && remainingDelta < 0 && TryGetComponent(firstPhyisicalInstigator, out DamageMultiplier firstDamageMultiplier))
        {
            remainingDelta *= firstDamageMultiplier.Value;
            totalAmountUncapped *= firstDamageMultiplier.Value;
        }

        // Shield
        if (remainingDelta != 0 && TryGetComponent(target, out Shield previousShield))
        {
            Shield newShield = clamp(previousShield + remainingDelta, 0, GetComponent<MaximumFix<Shield>>(target).Value);
            shieldDelta = newShield.Value - previousShield.Value;
            SetComponent(target, newShield);
            remainingDelta -= shieldDelta;

            if (HasComponent<ShieldLastHitTime>(target))
            {
                SetComponent<ShieldLastHitTime>(target, Time.ElapsedTime);
            }
        }

        // Health
        if (remainingDelta != 0 && TryGetComponent(target, out Health previousHP))
        {
            Health newHP = clamp(previousHP + remainingDelta, 0, GetComponent<MaximumFix<Health>>(target).Value);
            hpDelta = newHP.Value - previousHP.Value;
            SetComponent(target, newHP);
            remainingDelta -= hpDelta;

            if (HasComponent<HealthLastHitTime>(target))
            {
                SetComponent<HealthLastHitTime>(target, Time.ElapsedTime);
            }

            // If target is dead but has life points, regenerate all values and request special action
            if (newHP.Value == 0 && TryGetComponent(target, out LifePoints lifePoints) && lifePoints.Value > 0)
            {
                // recharge HP & shield
                if (HasComponent<Health>(target))
                    SetComponent<Health>(target, GetComponent<MaximumFix<Health>>(target).Value);
                if (HasComponent<Shield>(target))
                    SetComponent<Shield>(target, GetComponent<MaximumFix<Shield>>(target).Value);

                // decrement life points
                SetComponent<LifePoints>(target, lifePoints.Value - 1);

                if (EntityManager.TryGetBuffer(target, out DynamicBuffer<LifePointLostAction> lossActions))
                {
                    foreach (var item in lossActions)
                    {
                        _gameActionSystem.ActionRequestsManaged.Add(new GameActionRequestManaged()
                        {
                            ActionEntity = item.Value,
                            Instigator = target,
                        });
                    }
                }

                // set invincible for a minimum of 0.5 seconds
                TryGetComponent(target, out InvincibleUntilTime invincible);
                invincible.Time = max(invincible.Time, Time.ElapsedTime + (fix)0.5);
                EntityManager.AddComponentData(target, invincible);
            }
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
                TotalUncappedDelta = totalAmountUncapped,
                HPDelta = hpDelta,
                ShieldDelta = shieldDelta,
                Position = GetComponent<FixTranslation>(target)
            });
        }
    }

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
}

internal static partial class CommonWrites
{
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, NativeArray<DistanceHit> hits, fix amount, uint effectGroupID = uint.MaxValue)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        Entity firstPhysicalInstigator = Entity.Null;
        if (accessor.TryGetComponent(LastPhysicalInstigator, out FirstInstigator firstInstigatorComponent))
        {
            firstPhysicalInstigator = firstInstigatorComponent.Value;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            var request = new HealthChangeRequestData() { LastPhysicalInstigator = LastPhysicalInstigator, FirstPhysicalInstigator = firstPhysicalInstigator, Amount = -amount, Target = hits[i].Entity, EffectGroupID = effectGroupID };
            sys.RequestHealthChange(request);
        }
    }
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, NativeArray<Entity> targets, fix amount, uint effectGroupID = uint.MaxValue)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        Entity firstPhysicalInstigator = Entity.Null;
        if (accessor.TryGetComponent(LastPhysicalInstigator, out FirstInstigator firstInstigatorComponent))
        {
            firstPhysicalInstigator = firstInstigatorComponent.Value;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            var request = new HealthChangeRequestData() { LastPhysicalInstigator = LastPhysicalInstigator, FirstPhysicalInstigator = firstPhysicalInstigator, Amount = -amount, Target = targets[i], EffectGroupID = effectGroupID };
            sys.RequestHealthChange(request);
        }
    }

    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, Entity target, fix amount, uint effectGroupID = uint.MaxValue)
    {
        Entity firstPhysicalInstigator = Entity.Null;
        if (accessor.TryGetComponent(LastPhysicalInstigator, out FirstInstigator firstInstigatorComponent))
        {
            firstPhysicalInstigator = firstInstigatorComponent.Value;
        }

        var request = new HealthChangeRequestData() { LastPhysicalInstigator = LastPhysicalInstigator, FirstPhysicalInstigator = firstPhysicalInstigator, Amount = -amount, Target = target, EffectGroupID = effectGroupID };
        accessor.GetExistingSystem<ApplyDamageSystem>().RequestHealthChange(request);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, NativeArray<Entity> targets, fix amount, uint effectGroupID = uint.MaxValue)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, LastPhysicalInstigator, targets, -amount, effectGroupID);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, Entity target, fix amount, uint effectGroupID = uint.MaxValue)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, LastPhysicalInstigator, target, -amount, effectGroupID);
    }
}