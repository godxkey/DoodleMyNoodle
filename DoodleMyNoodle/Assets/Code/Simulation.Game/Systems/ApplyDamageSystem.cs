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
    public Entity ActionOnHealthChanged;
    public Entity ActionOnExtremeReached;
    public uint EffectGroupID;
    public bool IsAutoAttack;
}

public struct DamageProcessor : IComponentData
{
    public GameFunctionId FunctionId;
}

public struct GameFunctionDamageProcessorArg
{
    public ISimGameWorldReadWriteAccessor Accessor;
    public Entity EffectEntity;
    public HealthChangeRequestData RequestData;
    public fix RemainingDamage;
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
            ProcessHealthChange(healthChangeData);
        }
    }

    private void ProcessHealthChange(HealthChangeRequestData request)
    {
        NativeList<(DamageProcessor dmgProcessor, Entity entity)> dmgProcessors = new NativeList<(DamageProcessor dmgProcessor, Entity entity)>(Allocator.Temp);
        Entity target = request.Target;
        Entity lastPhysicalInstigator = request.LastPhysicalInstigator;
        Entity firstPhyisicalInstigator = request.FirstPhysicalInstigator;
        fix amount = request.Amount;
        uint effectGroupID = request.EffectGroupID;
        Entity actionOnHealthChanged = request.ActionOnHealthChanged;
        Entity actionOnExtremeReached = request.ActionOnExtremeReached;
        bool isAutoAttack = request.IsAutoAttack;

        fix shieldDelta = 0;
        fix hpDelta = 0;
        fix remainingDelta = amount;
        fix totalAmountUncapped = amount;

        if (EntityManager.TryGetComponent(request.Target, out DamageProcessor dmgProcessor))
        {
            dmgProcessors.Add((dmgProcessor, request.Target));
        }

        // find who should be really targetted if there is a HealthProxy component. This is notably used by players since they all share the same health pool
        while (HasComponent<HealthProxy>(target))
        {
            var newTarget = GetComponent<HealthProxy>(target);
            if (newTarget == Entity.Null)
                break;

            target = newTarget;

            if (EntityManager.TryGetComponent(target, out DamageProcessor processor))
            {
                dmgProcessors.Add((processor, request.Target));
            }
        }

        // prevents too many damage instance from the same source/group
        if (effectGroupID != 0)
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
        if (remainingDelta < 0 && EntityManager.TryGetComponent(target, out InvincibleUntilTime invincibleUntilTime) && invincibleUntilTime.Time > Time.ElapsedTime)
        {
            remainingDelta = max(remainingDelta, 0);
        }

        // Damage multipliers for both the instigator that started the attack and the entity applying the attack (ex: pawn shooting arrow)
        if (lastPhysicalInstigator != Entity.Null && remainingDelta < 0 && EntityManager.TryGetComponent(lastPhysicalInstigator, out DamageMultiplier lastDamageMultiplier))
        {
            remainingDelta *= lastDamageMultiplier.Value;
            totalAmountUncapped *= lastDamageMultiplier.Value;
        }

        if (firstPhyisicalInstigator != Entity.Null && remainingDelta < 0 && EntityManager.TryGetComponent(firstPhyisicalInstigator, out DamageMultiplier firstDamageMultiplier))
        {
            remainingDelta *= firstDamageMultiplier.Value;
            totalAmountUncapped *= firstDamageMultiplier.Value;
        }

        // target might have an effect that increase it's damage taken
        if (remainingDelta < 0 && EntityManager.TryGetComponent(target, out DamageReceivedMultiplier damageReceivedMultiplier))
        {
            remainingDelta *= damageReceivedMultiplier;
            totalAmountUncapped *= damageReceivedMultiplier;
        }

        // execute damage processors (if any)
        if (dmgProcessors.Length > 0 && remainingDelta < 0)
        {
            GameFunctionDamageProcessorArg arg = new GameFunctionDamageProcessorArg()
            {
                Accessor = Accessor,
                RequestData = request,
                RemainingDamage = -remainingDelta
            };

            for (int i = 0; i < dmgProcessors.Length; i++)
            {
                if (arg.RemainingDamage <= 0)
                    break;

                arg.EffectEntity = dmgProcessors[i].entity;
                GameFunctions.Execute(dmgProcessors[i].dmgProcessor.FunctionId, ref arg);
            }

            remainingDelta = -arg.RemainingDamage;
        }

        // Shield
        if (remainingDelta != 0 && EntityManager.TryGetComponent(target, out Shield previousShield))
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
        if (remainingDelta != 0 && EntityManager.TryGetComponent(target, out Health previousHP))
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
            if (newHP.Value == 0 && EntityManager.TryGetComponent(target, out LifePoints lifePoints) && lifePoints.Value > 0)
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
                EntityManager.TryGetComponent(target, out InvincibleUntilTime invincible);
                invincible.Time = max(invincible.Time, Time.ElapsedTime + (fix)0.5);
                EntityManager.AddComponentData(target, invincible);
            }

            // Game Action to trigger on damaged done
            if (hpDelta > 0)
            {
                CommonWrites.RequestExecuteGameAction(Accessor, lastPhysicalInstigator, actionOnHealthChanged, target);

                // Extreme (if we dealt damage, it's an death game action / if we healed, it's a full hp reached game action)
                if (newHP.Value == 0 || newHP.Value == GetComponent<MaximumFix<Health>>(target).Value)
                {
                    CommonWrites.RequestExecuteGameAction(Accessor, lastPhysicalInstigator, actionOnExtremeReached, target);
                }
            }
        }

        // Trigger Signal on Damage
        if (hpDelta < 0 && shieldDelta < 0 && HasComponent<TriggerSignalOnDamage>(target) && EntityManager.TryGetComponent(target, out SignalEmissionType emissionType))
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
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, NativeArray<DistanceHit> hits, fix amount, Entity actionOnHealthChanged, Entity actionOnExtremeReached, uint effectGroupID = 0)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        Entity firstPhysicalInstigator = Entity.Null;
        if (accessor.TryGetComponent(LastPhysicalInstigator, out FirstInstigator firstInstigatorComponent))
        {
            firstPhysicalInstigator = firstInstigatorComponent.Value;
        }

        for (int i = 0; i < hits.Length; i++)
        {
            var request = new HealthChangeRequestData()
            {
                LastPhysicalInstigator = LastPhysicalInstigator,
                FirstPhysicalInstigator = firstPhysicalInstigator,
                Amount = -amount,
                Target = hits[i].Entity,
                ActionOnHealthChanged = actionOnHealthChanged,
                ActionOnExtremeReached = actionOnExtremeReached,
                EffectGroupID = effectGroupID
            };

            sys.RequestHealthChange(request);
        }
    }
    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, NativeArray<Entity> targets, fix amount, Entity actionOnHealthChanged, Entity actionOnExtremeReached, uint effectGroupID = 0)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        Entity firstPhysicalInstigator = Entity.Null;
        if (accessor.TryGetComponent(LastPhysicalInstigator, out FirstInstigator firstInstigatorComponent))
        {
            firstPhysicalInstigator = firstInstigatorComponent.Value;
        }

        for (int i = 0; i < targets.Length; i++)
        {
            var request = new HealthChangeRequestData()
            {
                LastPhysicalInstigator = LastPhysicalInstigator,
                FirstPhysicalInstigator = firstPhysicalInstigator,
                Amount = -amount,
                Target = targets[i],
                ActionOnHealthChanged = actionOnHealthChanged,
                ActionOnExtremeReached = actionOnExtremeReached,
                EffectGroupID = effectGroupID
            };

            sys.RequestHealthChange(request);
        }
    }

    public static void RequestDamage(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, Entity target, fix amount, Entity actionOnHealthChanged, Entity actionOnExtremeReached, uint effectGroupID = 0)
    {
        Entity firstPhysicalInstigator = Entity.Null;
        if (accessor.TryGetComponent(LastPhysicalInstigator, out FirstInstigator firstInstigatorComponent))
        {
            firstPhysicalInstigator = firstInstigatorComponent.Value;
        }

        var request = new HealthChangeRequestData()
        {
            LastPhysicalInstigator = LastPhysicalInstigator,
            FirstPhysicalInstigator = firstPhysicalInstigator,
            Amount = -amount,
            Target = target,
            ActionOnHealthChanged = actionOnHealthChanged,
            ActionOnExtremeReached = actionOnExtremeReached,
            EffectGroupID = effectGroupID
        };

        accessor.GetExistingSystem<ApplyDamageSystem>().RequestHealthChange(request);
    }

    public static void RequestHealthChange(ISimGameWorldReadWriteAccessor accessor, HealthChangeRequestData healthChangeRequest)
    {
        accessor.GetExistingSystem<ApplyDamageSystem>().RequestHealthChange(healthChangeRequest);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, NativeArray<Entity> targets, fix amount, Entity actionOnHealthChanged, Entity actionOnExtremeReached, uint effectGroupID = 0)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, LastPhysicalInstigator, targets, -amount, actionOnHealthChanged, actionOnExtremeReached, effectGroupID);
    }

    public static void RequestHeal(ISimGameWorldReadWriteAccessor accessor, Entity LastPhysicalInstigator, Entity target, fix amount, Entity actionOnHealthChanged, Entity actionOnExtremeReached, uint effectGroupID = 0)
    {
        // for now a heal request is a negative damage request
        RequestDamage(accessor, LastPhysicalInstigator, target, -amount, actionOnHealthChanged, actionOnExtremeReached, effectGroupID);
    }
}