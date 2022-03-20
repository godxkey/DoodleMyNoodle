using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct BaseDamageMultiplier : IComponentData
{
    public fix Value;

    public static implicit operator fix(BaseDamageMultiplier val) => val.Value;
    public static implicit operator BaseDamageMultiplier(fix val) => new BaseDamageMultiplier() { Value = val };
}

public struct DamageMultiplier : IComponentData
{
    public fix Value;

    public static implicit operator fix(DamageMultiplier val) => val.Value;
    public static implicit operator DamageMultiplier(fix val) => new DamageMultiplier() { Value = val };
}

public struct BaseDamageReceivedMultiplier : IComponentData
{
    public fix Value;

    public static implicit operator fix(BaseDamageReceivedMultiplier val) => val.Value;
    public static implicit operator BaseDamageReceivedMultiplier(fix val) => new BaseDamageReceivedMultiplier() { Value = val };
}

public struct DamageReceivedMultiplier : IComponentData
{
    public fix Value;

    public static implicit operator fix(DamageReceivedMultiplier val) => val.Value;
    public static implicit operator DamageReceivedMultiplier(fix val) => new DamageReceivedMultiplier() { Value = val };
}