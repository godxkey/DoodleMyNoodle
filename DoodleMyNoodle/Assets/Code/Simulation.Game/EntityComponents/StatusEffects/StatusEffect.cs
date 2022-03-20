using System.Collections.Generic;
using Unity.Entities;

public enum StatusEffectType
{
    Slow,
    Fast,
    Tanky,
    Brutal,
    Armored,
    AttackSpeedBoost,
    Poison,
    BonusDamage,
}

public struct StatusEffectSetting
{
    public StatusEffectType Type;
    public bool UseDuration;
    public fix Duration;
    /// <summary>
    /// On one UNIQUE instance, adding new status effects increases a count
    /// </summary>
    public bool Stackable;
    /// <summary>
    /// If False, only remove 1 stack
    /// </summary>
    public bool RemoveAllStacksOnDurationEnd;
    /// <summary>
    /// True : Only one entry possible
    /// False : Accumulate different instance in parallel (Stackable should be false)
    /// </summary>
    public bool Unique;
    public bool RefreshExistingUniqueInstanceDuration;
}

static public class StatusEffectSettings
{
    static public Dictionary<StatusEffectType,StatusEffectSetting> Settings = new Dictionary<StatusEffectType, StatusEffectSetting>()
    {
        {
            StatusEffectType.Slow,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.Slow,
                UseDuration = true,
                Duration = 2,
                RefreshExistingUniqueInstanceDuration = true,
                Stackable = false,
                Unique = true
            }
        },

        {
            StatusEffectType.Fast,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.Fast,
                UseDuration = false,
                Stackable = false,
                Unique = true
            }
        },

        {
            StatusEffectType.Tanky,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.Tanky,
                UseDuration = false,
                Stackable = false,
                Unique = true
            }
        },

        {
            StatusEffectType.Brutal,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.Brutal,
                UseDuration = false,
                Stackable = false,
                Unique = true
            }
        },

        {
            StatusEffectType.Armored,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.Armored,
                UseDuration = false,
                Stackable = false,
                Unique = true
            }
        },

        {
            StatusEffectType.AttackSpeedBoost,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.AttackSpeedBoost,
                UseDuration = true,
                Duration = 4,
                Stackable = false,
                Unique = false
            }
        },

        {
            StatusEffectType.Poison,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.Poison,
                UseDuration = true,
                Duration = 2,
                RefreshExistingUniqueInstanceDuration = true,
                Stackable = false,
                Unique = true
            }
        },

        {
            StatusEffectType.BonusDamage,
            new StatusEffectSetting()
            {
                Type = StatusEffectType.BonusDamage,
                UseDuration = true,
                Duration = 4,
                Stackable = false,
                Unique = false
            }
        },
    };
}

public struct StatusEffect : IBufferElementData
{
    public StatusEffectType Type;
    public fix RemainingTime;
    public int Stacks;
    public Entity Instigator;
}

public struct StartingStatusEffect : IBufferElementData
{
    public StatusEffectType Type;
    public int StackAmount;
}