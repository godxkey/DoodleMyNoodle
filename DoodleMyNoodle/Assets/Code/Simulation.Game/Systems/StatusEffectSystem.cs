using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public struct AddStatusEffectRequest : ISingletonBufferElementData
{
    public Entity Target;
    public StatusEffectType Type;
    public int StackAmount;
    public Entity Instigator;
}

public struct RemoveStatusEffectRequest : ISingletonBufferElementData
{
    public Entity Target;
    public StatusEffectType Type;
    public int StackAmount;
    public Entity Instigator;
}

[AlwaysUpdateSystem]
public class StatusEffectSystem : SimGameSystemBase
{
    List<Entity> EntityStatsToUpdate = new List<Entity>();

    protected override void OnUpdate()
    {
        UpdateDurations();

        var addStatusEffectRequests = GetSingletonBuffer<AddStatusEffectRequest>();
        ProcessAddRequests(addStatusEffectRequests);
        addStatusEffectRequests.Clear();

        var removeStatusEffectRequests = GetSingletonBuffer<RemoveStatusEffectRequest>();
        ProcessRemoveRequests(removeStatusEffectRequests);
        removeStatusEffectRequests.Clear();

        UpdateStats();
    }

    private void UpdateStats()
    {
        foreach (Entity entity in EntityStatsToUpdate)
        {
            DynamicBuffer<StatusEffect> statusEffects = GetBuffer<StatusEffect>(entity);

            // MAXIMUM HEALTH STAT
            if (EntityManager.TryGetComponent(entity, out BaseMaxHealth baseMaxHealth))
            {
                fix newMaxHealth = baseMaxHealth.Value;

                foreach (var statusEffect in statusEffects)
                {
                    if (statusEffect.Type == StatusEffectType.Tanky)
                    {
                        newMaxHealth *= (fix)1.75;
                    }
                }

                SetComponent(entity, new MaximumFix<Health>() { Value = newMaxHealth });
            }

            // SPEED STAT
            if (EntityManager.TryGetComponent(entity, out BaseMoveSpeed baseMoveSpeed))
            {
                fix newMoveSpeed = baseMoveSpeed.Value;

                foreach (var statusEffect in statusEffects)
                {
                    if (statusEffect.Type == StatusEffectType.Fast)
                    {
                        newMoveSpeed *= (fix)2;
                    }

                    if (statusEffect.Type == StatusEffectType.Slow)
                    {
                        newMoveSpeed *= (fix)0.5;
                    }
                }

                SetComponent(entity, new MoveSpeed() { Value = newMoveSpeed });
            }

            // DAMAGE
            if (EntityManager.TryGetComponent(entity, out BaseDamageMultiplier baseDamageMultiplier))
            {
                fix newDamageMultiplier = baseDamageMultiplier.Value;

                foreach (var statusEffect in statusEffects)
                {
                    if (statusEffect.Type == StatusEffectType.Brutal)
                    {
                        newDamageMultiplier *= (fix)2;
                    }

                    if (statusEffect.Type == StatusEffectType.BonusDamage)
                    {
                        newDamageMultiplier *= (fix)1.5;
                    }
                }
                
                SetComponent(entity, new DamageMultiplier() { Value = newDamageMultiplier });
            }

            // ATTACK SPEED
            if (EntityManager.TryGetComponent(entity, out BaseAttackSpeed baseAttackSpeed))
            {
                fix newAttackSpeed = baseAttackSpeed.Value;

                foreach (var statusEffect in statusEffects)
                {
                    if (statusEffect.Type == StatusEffectType.AttackSpeedBoost)
                    {
                        newAttackSpeed *= (fix)1.5;
                    }
                }

                SetComponent(entity, new AttackSpeed() { Value = newAttackSpeed });
            }
        }

        EntityStatsToUpdate.Clear();
    }

    private void UpdateDurations()
    {
        fix deltaTime = Time.DeltaTime;
        var removeStatusEffectRequests = GetSingletonBuffer<RemoveStatusEffectRequest>();

        Entities.ForEach((Entity entity, DynamicBuffer<StatusEffect> statusEffects) =>
        {
            for (int i = 0; i < statusEffects.Length; i++)
            {
                StatusEffect statusEffect = statusEffects[i];
                StatusEffectSetting setting = StatusEffectSettings.Settings[statusEffect.Type];

                if (!setting.UseDuration)
                    continue;

                statusEffect.RemainingTime -= deltaTime;

                if (statusEffect.RemainingTime < 0)
                {
                    removeStatusEffectRequests.Add(new RemoveStatusEffectRequest()
                    {
                        Type = statusEffect.Type,
                        StackAmount = setting.RemoveAllStacksOnDurationEnd ? statusEffect.Stacks : 1,
                        Target = entity,
                        Instigator = statusEffect.Instigator
                    });

                    statusEffect.RemainingTime += setting.Duration;
                }

                statusEffects[i] = statusEffect;
            }
        }).WithoutBurst().Run();
    }

    private void ProcessRemoveRequests(DynamicBuffer<RemoveStatusEffectRequest> removeStatusEffectRequests)
    {
        foreach (RemoveStatusEffectRequest removeRequest in removeStatusEffectRequests)
        {
            StatusEffectSetting setting = StatusEffectSettings.Settings[removeRequest.Type];

            if (!EntityManager.TryGetBuffer(removeRequest.Target, out DynamicBuffer<StatusEffect> statusEffects))
                continue;

            EntityStatsToUpdate.Add(removeRequest.Target);

            int index = -1;
            for (int i = 0; i < statusEffects.Length; i++)
            {
                StatusEffect currentStatusEffect = statusEffects[i];
                if (currentStatusEffect.Type == removeRequest.Type && currentStatusEffect.Instigator == removeRequest.Instigator)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                continue;

            StatusEffect statusEffect = statusEffects[index];
            if (setting.Stackable)
            {
                statusEffect.Stacks = max(0, statusEffect.Stacks - max(1, removeRequest.StackAmount));
            }
            else
            {
                statusEffect.Stacks = 0;
            }

            statusEffects[index] = statusEffect;

            if (statusEffect.Stacks == 0)
            {
                statusEffects.RemoveAt(index);
            }
        }
    }

    private void ProcessAddRequests(DynamicBuffer<AddStatusEffectRequest> addStatusEffectRequests)
    {
        foreach (AddStatusEffectRequest addRequest in addStatusEffectRequests)
        {
            StatusEffectSetting setting = StatusEffectSettings.Settings[addRequest.Type];

            if (!EntityManager.TryGetBuffer(addRequest.Target, out DynamicBuffer<StatusEffect> statusEffects))
                continue;

            EntityStatsToUpdate.Add(addRequest.Target);

            bool addNew = true;

            if (setting.Unique)
            {
                int index = -1;
                for (int i = 0; i < statusEffects.Length; i++)
                {
                    if (statusEffects[i].Type == addRequest.Type)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    StatusEffect statusEffect = statusEffects[index];

                    if (setting.RefreshExistingUniqueInstanceDuration)
                    {
                        statusEffect.RemainingTime = setting.Duration;
                    }

                    if (setting.Stackable)
                    {
                        statusEffect.Stacks += max(1, addRequest.StackAmount);
                    }

                    statusEffects[index] = statusEffect;

                    addNew = false;
                }
            }

            if (addNew)
            {
                statusEffects.Add(new StatusEffect()
                {
                    Type = setting.Type,
                    RemainingTime = setting.UseDuration ? setting.Duration : fix.MinValue,
                    Stacks = setting.Stackable ? max(1, addRequest.StackAmount) : 1,
                    Instigator = addRequest.Instigator
                });
            }
        }
    }
}

internal static partial class CommonWrites
{
    static public void AddStatusEffect(ISimGameWorldReadWriteAccessor accessor, AddStatusEffectRequest request)
    {
        accessor.GetSingletonBuffer<AddStatusEffectRequest>().Add(request);
    }

    static public void RemoveStatusEffect(ISimGameWorldReadWriteAccessor accessor, RemoveStatusEffectRequest request)
    {
        accessor.GetSingletonBuffer<RemoveStatusEffectRequest>().Add(request);
    }
}