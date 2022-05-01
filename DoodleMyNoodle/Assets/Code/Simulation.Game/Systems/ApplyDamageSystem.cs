using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using Unity.Collections;
using UnityEngineX;
using System.Diagnostics;

public struct HealthDeltaEventData
{
    /// <summary>
    /// When HealthProxy is used, this is the entity that receives a health change.
    /// </summary>
    public Entity FinalVictim;
    /// <summary>
    /// When HealthProxy is used, this is the original target.
    /// </summary>
    public Entity OriginalVictim;
    public fix2 VictimPosition;

    /// <summary>
    /// Vector from the victim center to the instigation point
    /// </summary>
    public fix2 ImpactVector;
    public InstigatorSet InstigatorSet;
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
    public bool IsHeal => TotalUncappedDelta > 0;
    public bool IsDamage => !IsHeal;
    public bool IsAutoAttack;
}

public struct HealthChangeRequestData : ISingletonBufferElementData
{
    public fix Amount;
    public Entity Target;
    public InstigatorSet InstigatorSet;
    public Entity ActionOnHealthChanged;
    public Entity ActionOnExtremeReached;
    public uint EffectGroupID;
    public bool IsAutoAttack;

    public fix2 Target2InstigationVector; // used for impact point display
}

public struct DamageRequestSettings
{
    public fix DamageAmount;
    public InstigatorSet InstigatorSet;
    public Entity ActionOnHealthChanged;
    public Entity ActionOnExtremeReached;
    public uint EffectGroupID;
    public bool IsAutoAttack;

    public static explicit operator HealRequestSettings(DamageRequestSettings settings)
    {
        return new HealRequestSettings()
        {
            HealAmount = -settings.DamageAmount,
            InstigatorSet = settings.InstigatorSet,
            ActionOnHealthChanged = settings.ActionOnHealthChanged,
            ActionOnExtremeReached = settings.ActionOnExtremeReached,
            EffectGroupID = settings.EffectGroupID,
            IsAutoAttack = settings.IsAutoAttack,
        };
    }
    public static explicit operator DamageRequestSettings(HealRequestSettings settings)
    {
        return new DamageRequestSettings()
        {
            DamageAmount = -settings.HealAmount,
            InstigatorSet = settings.InstigatorSet,
            ActionOnHealthChanged = settings.ActionOnHealthChanged,
            ActionOnExtremeReached = settings.ActionOnExtremeReached,
            EffectGroupID = settings.EffectGroupID,
            IsAutoAttack = settings.IsAutoAttack,
        };
    }
}

public struct HealRequestSettings
{
    public fix HealAmount;
    public InstigatorSet InstigatorSet;
    public Entity ActionOnHealthChanged;
    public Entity ActionOnExtremeReached;
    public uint EffectGroupID;
    public bool IsAutoAttack;
}

public struct DamageReceivedProcessor : IComponentData
{
    public GameFunctionId FunctionId;
}

public struct GameFunctionDamageReceivedProcessorArg
{
    public ISimGameWorldReadWriteAccessor Accessor;
    public Entity EffectEntity;
    public HealthChangeRequestData RequestData;
    public fix RemainingDamage;
}

public struct DamageDealtProcessor : IComponentData
{
    public GameFunctionId FunctionId;
}

public struct GameFunctionDamageDealtProcessorArg
{
    public ISimGameWorldReadWriteAccessor Accessor;
    public Entity EffectEntity;
    public HealthChangeRequestData RequestData;
    public fix RemainingDamage;
}

public struct OnDeathProcessor : IComponentData
{
    public GameFunctionId FunctionId;
}

public struct GameFunctionOnDeathProcessorArg
{
    public ISimGameWorldReadWriteAccessor Accessor;
    public Entity EffectEntity;
    public HealthChangeRequestData RequestData;
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
        NativeList<(DamageReceivedProcessor dmgProcessor, Entity effectEntity)> dmgReceivedProcessors = new NativeList<(DamageReceivedProcessor, Entity)>(Allocator.Temp);
        NativeList<(DamageDealtProcessor dmgProcessor, Entity effectEntity)> dmgDealtProcessors = new NativeList<(DamageDealtProcessor, Entity)>(Allocator.Temp);

        Entity target = request.Target;
        Entity lastPhysicalInstigator = request.InstigatorSet.LastPhysicalInstigator;
        Entity firstPhyisicalInstigator = request.InstigatorSet.FirstPhysicalInstigator;
        fix amount = request.Amount;
        uint effectGroupID = request.EffectGroupID;
        Entity actionOnHealthChanged = request.ActionOnHealthChanged;
        Entity actionOnExtremeReached = request.ActionOnExtremeReached;
        bool isAutoAttack = request.IsAutoAttack;

        fix shieldDelta = 0;
        fix hpDelta = 0;
        fix remainingDelta = amount;
        fix totalAmountUncapped = amount;

        // collect
        CollectEffectComponents(target, dmgReceivedProcessors);
        CollectEffectComponents(firstPhyisicalInstigator, dmgDealtProcessors);

        // find who should be really targetted if there is a HealthProxy component. This is notably used by players since they all share the same health pool
        while (HasComponent<HealthProxy>(target))
        {
            var newTarget = GetComponent<HealthProxy>(target);
            if (newTarget == Entity.Null)
                break;

            target = newTarget;

            CollectEffectComponents(target, dmgReceivedProcessors);
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

        // execute damage dealt processors (if any)
        if (dmgDealtProcessors.Length > 0 && remainingDelta < 0)
        {
            GameFunctionDamageDealtProcessorArg arg = new GameFunctionDamageDealtProcessorArg()
            {
                Accessor = Accessor,
                RequestData = request,
                RemainingDamage = -remainingDelta
            };

            for (int i = 0; i < dmgDealtProcessors.Length; i++)
            {
                if (arg.RemainingDamage <= 0)
                    break;

                arg.EffectEntity = dmgDealtProcessors[i].effectEntity;
                GameFunctions.Execute(dmgDealtProcessors[i].dmgProcessor.FunctionId, ref arg);
            }

            remainingDelta = -arg.RemainingDamage;
        }

        // execute damage received processors (if any)
        if (dmgReceivedProcessors.Length > 0 && remainingDelta < 0)
        {
            GameFunctionDamageReceivedProcessorArg arg = new GameFunctionDamageReceivedProcessorArg()
            {
                Accessor = Accessor,
                RequestData = request,
                RemainingDamage = -remainingDelta
            };

            for (int i = 0; i < dmgReceivedProcessors.Length; i++)
            {
                if (arg.RemainingDamage <= 0)
                    break;

                arg.EffectEntity = dmgReceivedProcessors[i].effectEntity;
                GameFunctions.Execute(dmgReceivedProcessors[i].dmgProcessor.FunctionId, ref arg);
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
            fix maxHP = GetComponent<MaximumFix<Health>>(target).Value;
            Health newHP = clamp(previousHP + remainingDelta, 0, maxHP);
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
                    SetComponent<Health>(target, maxHP);
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
            if (hpDelta != 0)
            {
                if (actionOnHealthChanged != Entity.Null)
                    CommonWrites.RequestExecuteGameAction(Accessor, lastPhysicalInstigator, actionOnHealthChanged, target);

                // Extreme (if we dealt damage, it's an death game action / if we healed, it's a full hp reached game action)
                if ((newHP.Value == 0 || newHP.Value == maxHP) && actionOnExtremeReached != Entity.Null)
                    CommonWrites.RequestExecuteGameAction(Accessor, lastPhysicalInstigator, actionOnExtremeReached, target);
            }

            // Process Death
            if (newHP.Value == 0 && !HasComponent<DeadTag>(target))
            {
                if (HasComponent<PhysicsGravity>(target))
                    SetComponent(target, new PhysicsGravity() { Scale = 1 });
                EntityManager.AddComponentData(target, new DeadTag());

                NativeList<(OnDeathProcessor dmgProcessor, Entity effectEntity)> onDeathProcessors = new NativeList<(OnDeathProcessor, Entity)>(Allocator.Temp);
                CollectEffectComponents(target, onDeathProcessors);

                GameFunctionOnDeathProcessorArg arg = new GameFunctionOnDeathProcessorArg()
                {
                    Accessor = Accessor,
                    RequestData = request
                };
                foreach (var onDeathProcessor in onDeathProcessors)
                {
                    arg.EffectEntity = onDeathProcessor.effectEntity;
                    GameFunctions.Execute(onDeathProcessor.dmgProcessor.FunctionId, ref arg);
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
            EntityManager.TryGetComponent(target, out FixTranslation targetPosition);
            EntityManager.TryGetComponent(lastPhysicalInstigator, out FixTranslation instigatorPosition);

            PresentationEvents.HealthDeltaEvents.Push(new HealthDeltaEventData()
            {
                FinalVictim = target,
                OriginalVictim = request.Target,
                VictimPosition = targetPosition.Value,
                ImpactVector = request.Target2InstigationVector,
                InstigatorSet = new InstigatorSet()
                {
                    FirstPhysicalInstigator = firstPhyisicalInstigator,
                    LastPhysicalInstigator = lastPhysicalInstigator,
                },
                TotalUncappedDelta = totalAmountUncapped,
                HPDelta = hpDelta,
                ShieldDelta = shieldDelta,
                IsAutoAttack = isAutoAttack
            });
        }
    }

    private void CollectEffectComponents<T>(Entity entity, NativeList<(T, Entity effectEntity)> processors) where T : struct, IComponentData
    {
        if (EntityManager.TryGetBuffer(entity, out DynamicBuffer<GameEffectBufferElement> effects))
        {
            foreach (var effect in effects)
            {
                if (HasComponent<T>(effect.EffectEntity))
                {
                    processors.Add((GetComponent<T>(effect.EffectEntity), effect.EffectEntity));
                }
            }
        }
    }

    //[Conditional("SAFETY")]
    //private void ValidateRequest(ref HealthChangeRequestData request)
    //{
    //    if (!EntityManager.HasComponent<FixTranslation>(request.LastPhysicalInstigator))
    //    {
    //        Log.Error($"Last physical instigator does not have a FixTranslation component:{EntityManager.GetNameSafe(request.LastPhysicalInstigator)}");
    //    }

    //    if (!EntityManager.HasComponent<FixTranslation>(request.FirstPhysicalInstigator))
    //    {
    //        Log.Error($"First physical instigator does not have a FixTranslation component:{EntityManager.GetNameSafe(request.FirstPhysicalInstigator)}");
    //    }
    //}

    public void RequestHealthChange(HealthChangeRequestData damageRequestData)
    {
        GetDamageRequestBuffer().Add(damageRequestData);
    }

    private DynamicBuffer<HealthChangeRequestData> GetDamageRequestBuffer()
    {
        return GetSingletonBuffer<HealthChangeRequestData>();
    }
}

internal static partial class CommonWrites
{
    public static void RequestDamage(
        ISimGameWorldReadWriteAccessor accessor,
        DamageRequestSettings args,
        NativeArray<DistanceHit> hits)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < hits.Length; i++)
        {
            var request = new HealthChangeRequestData()
            {
                IsAutoAttack = args.IsAutoAttack,
                InstigatorSet = args.InstigatorSet,
                Target2InstigationVector = (fix2)(-hits[i].Direction),
                Amount = -args.DamageAmount,
                Target = hits[i].Entity,
                ActionOnHealthChanged = args.ActionOnHealthChanged,
                ActionOnExtremeReached = args.ActionOnExtremeReached,
                EffectGroupID = args.EffectGroupID,
            };

            sys.RequestHealthChange(request);
        }
    }

    public static void RequestDamage(
        ISimGameWorldReadWriteAccessor accessor,
        DamageRequestSettings args,
        NativeArray<Entity> targets)
    {
        var sys = accessor.GetExistingSystem<ApplyDamageSystem>();

        for (int i = 0; i < targets.Length; i++)
        {
            fix2 target2Instigator = default;
            if (accessor.TryGetComponent(args.InstigatorSet.LastPhysicalInstigator, out FixTranslation instigatorPos)
                && accessor.TryGetComponent(targets[i], out FixTranslation targetPos))
            {
                target2Instigator = instigatorPos.Value - targetPos.Value;
            }

            var request = new HealthChangeRequestData()
            {
                IsAutoAttack = args.IsAutoAttack,
                InstigatorSet = args.InstigatorSet,
                Target2InstigationVector = target2Instigator,
                Amount = -args.DamageAmount,
                Target = targets[i],
                ActionOnHealthChanged = args.ActionOnHealthChanged,
                ActionOnExtremeReached = args.ActionOnExtremeReached,
                EffectGroupID = args.EffectGroupID,
            };

            sys.RequestHealthChange(request);
        }
    }

    public static void RequestDamage(
        ISimGameWorldReadWriteAccessor accessor,
        DamageRequestSettings args,
        Entity target)
    {
        fix2 target2Instigator = default;
        if (accessor.TryGetComponent(args.InstigatorSet.LastPhysicalInstigator, out FixTranslation instigatorPos)
            && accessor.TryGetComponent(target, out FixTranslation targetPos))
        {
            target2Instigator = instigatorPos.Value - targetPos.Value;
        }

        var request = new HealthChangeRequestData()
        {
            IsAutoAttack = args.IsAutoAttack,
            InstigatorSet = args.InstigatorSet,
            Target2InstigationVector = target2Instigator,
            Amount = -args.DamageAmount,
            Target = target,
            ActionOnHealthChanged = args.ActionOnHealthChanged,
            ActionOnExtremeReached = args.ActionOnExtremeReached,
            EffectGroupID = args.EffectGroupID,
        };

        accessor.GetExistingSystem<ApplyDamageSystem>().RequestHealthChange(request);
    }

    public static void RequestHeal(
        ISimGameWorldReadWriteAccessor accessor,
        HealRequestSettings args,
        NativeArray<DistanceHit> hits)
    {
        RequestDamage(accessor, (DamageRequestSettings)args, hits);
    }

    public static void RequestHeal(
        ISimGameWorldReadWriteAccessor accessor,
        HealRequestSettings args,
        NativeArray<Entity> targets)
    {
        RequestDamage(accessor, (DamageRequestSettings)args, targets);
    }

    public static void RequestHeal(
        ISimGameWorldReadWriteAccessor accessor,
        HealRequestSettings args,
        Entity target)
    {
        RequestDamage(accessor, (DamageRequestSettings)args, target);
    }

    public static void RequestHealthChange(ISimGameWorldReadWriteAccessor accessor, HealthChangeRequestData healthChangeRequest)
    {
        accessor.GetExistingSystem<ApplyDamageSystem>().RequestHealthChange(healthChangeRequest);
    }
}