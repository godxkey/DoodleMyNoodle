using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public struct SystemRequestAddStatModifier : ISingletonBufferElementData
{
    public Entity Target;
    public StatModifierType Type;
    public int StackAmount;
    public Entity Instigator;
}

public struct SystemRequestRemoveStatModifier : ISingletonBufferElementData
{
    public Entity Target;
    public StatModifierType Type;
    public int StackAmount;
    public Entity Instigator;
}

[AlwaysUpdateSystem]
public partial class StatModifierSystem : SimGameSystemBase
{
    List<Entity> _entityStatsToUpdate = new List<Entity>();

    protected override void OnUpdate()
    {
        var addStatusEffectRequests = GetSingletonBuffer<SystemRequestAddStatModifier>();
        ProcessAddRequests(addStatusEffectRequests);
        addStatusEffectRequests.Clear();

        var removeStatusEffectRequests = GetSingletonBuffer<SystemRequestRemoveStatModifier>();
        ProcessRemoveRequests(removeStatusEffectRequests);
        removeStatusEffectRequests.Clear();

        UpdateStats();
    }

    private void UpdateStats()
    {
        foreach (Entity entity in _entityStatsToUpdate)
        {
            DynamicBuffer<StatModifier> statModifiers = GetBuffer<StatModifier>(entity);

            // MAXIMUM HEALTH STAT
            if (EntityManager.TryGetComponent(entity, out BaseHealthMax baseMaxHealth))
            {
                fix newMaxHealth = baseMaxHealth.Value;

                foreach (var statModifier in statModifiers)
                {
                    if (statModifier.Type == StatModifierType.Tanky)
                    {
                        newMaxHealth *= statModifier.Value;
                    }
                }

                SetComponent(entity, new HealthMax() { Value = newMaxHealth });
            }

            // SPEED STAT
            if (EntityManager.TryGetComponent(entity, out BaseMoveSpeed baseMoveSpeed))
            {
                fix newMoveSpeed = baseMoveSpeed.Value;

                foreach (var statModifier in statModifiers)
                {
                    if (statModifier.Type == StatModifierType.Fast || statModifier.Type == StatModifierType.Slow || statModifier.Type == StatModifierType.Stunned)
                    {
                        newMoveSpeed *= statModifier.Value;
                    }
                }

                SetComponent(entity, new MoveSpeed() { Value = newMoveSpeed });
            }

            // DAMAGE DONE
            if (EntityManager.TryGetComponent(entity, out BaseDamageMultiplier baseDamageMultiplier))
            {
                fix newDamageMultiplier = baseDamageMultiplier.Value;

                foreach (var statModifier in statModifiers)
                {
                    if (statModifier.Type == StatModifierType.Brutal || statModifier.Type == StatModifierType.BonusDamage)
                    {
                        newDamageMultiplier *= statModifier.Value;
                    }
                }

                SetComponent(entity, new DamageMultiplier() { Value = newDamageMultiplier });
            }

            // DAMAGE RECEIVED
            if (EntityManager.TryGetComponent(entity, out BaseDamageReceivedMultiplier baseDamageReceivedMultiplier))
            {
                fix newDamageReceivedMultiplier = baseDamageReceivedMultiplier.Value;

                foreach (var statModifier in statModifiers)
                {
                    if (statModifier.Type == StatModifierType.Armored || statModifier.Type == StatModifierType.Poison)
                    {
                        newDamageReceivedMultiplier *= statModifier.Value;
                    }
                }

                SetComponent(entity, new DamageReceivedMultiplier() { Value = newDamageReceivedMultiplier });
            }

            // ATTACK SPEED
            if (EntityManager.TryGetComponent(entity, out BaseAttackSpeed baseAttackSpeed))
            {
                fix newAttackSpeed = baseAttackSpeed.Value;

                foreach (var statModifier in statModifiers)
                {
                    if (statModifier.Type == StatModifierType.AttackSpeedBoost || statModifier.Type == StatModifierType.Stunned)
                    {
                        newAttackSpeed *= statModifier.Value;
                    }
                }

                SetComponent(entity, new AttackSpeed() { Value = newAttackSpeed });
            }
        }

        _entityStatsToUpdate.Clear();
    }

    private void ProcessRemoveRequests(DynamicBuffer<SystemRequestRemoveStatModifier> removeStatusEffectRequests)
    {
        foreach (SystemRequestRemoveStatModifier removeRequest in removeStatusEffectRequests)
        {
            if (!EntityManager.TryGetBuffer(removeRequest.Target, out DynamicBuffer<StatModifier> statModifiers))
                continue;

            _entityStatsToUpdate.Add(removeRequest.Target);

            int index = -1;
            for (int i = 0; i < statModifiers.Length; i++)
            {
                StatModifier currentStatusEffect = statModifiers[i];
                if (currentStatusEffect.Type == removeRequest.Type)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
                continue;

            StatModifier statModifier = statModifiers[index];
            statModifier.Stacks = max(0, statModifier.Stacks - max(1, removeRequest.StackAmount));

            statModifiers[index] = statModifier;

            if (statModifier.Stacks == 0)
            {
                statModifiers.RemoveAt(index);
            }
        }
    }

    private void ProcessAddRequests(DynamicBuffer<SystemRequestAddStatModifier> addStatusEffectRequests)
    {
        foreach (SystemRequestAddStatModifier addRequest in addStatusEffectRequests)
        {
            StatModifierSetting setting = StatModifierSettings.Settings[addRequest.Type];

            if (!EntityManager.TryGetBuffer(addRequest.Target, out DynamicBuffer<StatModifier> statModifiers))
                continue;

            _entityStatsToUpdate.Add(addRequest.Target);

            int index = -1;
            for (int i = 0; i < statModifiers.Length; i++)
            {
                if (statModifiers[i].Type == addRequest.Type)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                StatModifier statModifier = statModifiers[index];
                statModifier.Stacks += max(1, addRequest.StackAmount);
                statModifiers[index] = statModifier;
            }
            else
            {
                statModifiers.Add(new StatModifier()
                {
                    Type = setting.Type,
                    Value = setting.Value,
                    Blendmode = setting.Blendmode,
                    Stacks = addRequest.StackAmount,
                });
            }
        }
    }
}

internal static partial class CommonWrites
{
    static public void AddStatusEffect(ISimGameWorldReadWriteAccessor accessor, SystemRequestAddStatModifier request)
    {
        accessor.GetSingletonBuffer<SystemRequestAddStatModifier>().Add(request);
    }

    static public void RemoveStatusEffect(ISimGameWorldReadWriteAccessor accessor, SystemRequestRemoveStatModifier request)
    {
        accessor.GetSingletonBuffer<SystemRequestRemoveStatModifier>().Add(request);
    }
}