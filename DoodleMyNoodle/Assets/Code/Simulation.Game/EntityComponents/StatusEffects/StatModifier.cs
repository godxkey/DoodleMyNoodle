using System.Collections.Generic;
using Unity.Entities;

// TODO : refaire ce système en étant agnostique du design.
// Les StatModifier type devrait être genre "attack speed, health, damage, resistance, etc." et non "armored, poisoned"

public enum StatModifierType
{
    Slow,
    Fast,
    Tanky,
    Brutal,
    Armored,
    AttackSpeedBoost,
    Poison,
    BonusDamage,
    Stunned,
}

public enum StatModifierBlendmode
{
    Additive,
    Multiplier
}

public struct StatModifierSetting
{
    public StatModifierType Type;
    public fix Value;
    public StatModifierBlendmode Blendmode;
}

static public class StatModifierSettings
{
    static public Dictionary<StatModifierType,StatModifierSetting> Settings = new Dictionary<StatModifierType, StatModifierSetting>()
    {
        {
            StatModifierType.Slow,
            new StatModifierSetting()
            {
                Type = StatModifierType.Slow,
                Value = (fix)0.5,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.Fast,
            new StatModifierSetting()
            {
                Type = StatModifierType.Fast,
                Value = (fix)2,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.Tanky,
            new StatModifierSetting()
            {
                Type = StatModifierType.Tanky,
                Value = (fix)1.75,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.Brutal,
            new StatModifierSetting()
            {
                Type = StatModifierType.Brutal,
                Value = (fix)2,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.Armored,
            new StatModifierSetting()
            {
                Type = StatModifierType.Armored,
                Value = (fix)0.75,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.AttackSpeedBoost,
            new StatModifierSetting()
            {
                Type = StatModifierType.AttackSpeedBoost,
                Value = (fix)1.5,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.Poison,
            new StatModifierSetting()
            {
                Type = StatModifierType.Poison,
                Value = (fix)2,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.BonusDamage,
            new StatModifierSetting()
            {
                Type = StatModifierType.BonusDamage,
                Value = (fix)1.5,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },

        {
            StatModifierType.Stunned,
            new StatModifierSetting()
            {
                Type = StatModifierType.Stunned,
                Value = (fix)0,
                Blendmode = StatModifierBlendmode.Multiplier
            }
        },
    };
}

public struct StatModifier : IBufferElementData
{
    public StatModifierType Type;
    public fix Value;
    public StatModifierBlendmode Blendmode;
    public int Stacks;
    public Entity Instigator;
}

public struct StartingStatModifier : IBufferElementData
{
    public StatModifierType Type;
    public int StackAmount;
}