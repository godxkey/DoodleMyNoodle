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