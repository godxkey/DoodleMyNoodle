using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public struct AddStatModifierRequest : ISingletonBufferElementData
{
    public Entity Target;
    public StatModifierType Type;
    public int StackAmount;
    public Entity Instigator;
}

public struct RemoveStatModifierRequest : ISingletonBufferElementData
{
    public Entity Target;
    public StatModifierType Type;
    public int StackAmount;
    public Entity Instigator;
}

[AlwaysUpdateSystem]
public class StatModifierSystem : SimGameSystemBase
{
    List<Entity> _entityStatsToUpdate = new List<Entity>();

    protected override void OnUpdate()
    {
        var addStatusEffectRequests = GetSingletonBuffer<AddStatModifierRequest>();
        ProcessAddRequests(addStatusEffectRequests);
        addStatusEffectRequests.Clear();

        var removeStatusEffectRequests = GetSingletonBuffer<RemoveStatModifierRequest>();
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
            if (EntityManager.TryGetComponent(entity, out BaseMaxHealth baseMaxHealth))
            {
                fix newMaxHealth = baseMaxHealth.Value;

                foreach (var statModifier in statModifiers)
                {
                    if (statModifier.Type == StatModifierType.Tanky)
                    {
                        newMaxHealth *= statModifier.Value;
                    }
                }

                SetComponent(entity, new MaximumFix<Health>() { Value = newMaxHealth });
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

    private void ProcessRemoveRequests(DynamicBuffer<RemoveStatModifierRequest> removeStatusEffectRequests)
    {
        foreach (RemoveStatModifierRequest removeRequest in removeStatusEffectRequests)
        {
            if (!EntityManager.TryGetBuffer(removeRequest.Target, out DynamicBuffer<StatModifier> statModifiers))
                continue;

            _entityStatsToUpdate.Add(removeRequest.Target);

            int index = -1;
            for (int i = 0; i < statModifiers.Length; i++)
            {
                StatModifier currentStatusEffect = statModifiers[i];
                if (currentStatusEffect.Type == removeRequest.Type && currentStatusEffect.Instigator == removeRequest.Instigator)
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

    private void ProcessAddRequests(DynamicBuffer<AddStatModifierRequest> addStatusEffectRequests)
    {
        foreach (AddStatModifierRequest addRequest in addStatusEffectRequests)
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
                    Instigator = addRequest.Instigator
                });
            }
        }
    }
}

internal static partial class CommonWrites
{
    static public void AddStatusEffect(ISimGameWorldReadWriteAccessor accessor, AddStatModifierRequest request)
    {
        accessor.GetSingletonBuffer<AddStatModifierRequest>().Add(request);
    }

    static public void RemoveStatusEffect(ISimGameWorldReadWriteAccessor accessor, RemoveStatModifierRequest request)
    {
        accessor.GetSingletonBuffer<RemoveStatModifierRequest>().Add(request);
    }
}