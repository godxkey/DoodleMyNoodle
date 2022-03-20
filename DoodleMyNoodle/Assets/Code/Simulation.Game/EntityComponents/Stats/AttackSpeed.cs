using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct BaseAttackSpeed : IComponentData
{
    public fix Value;

    public static implicit operator fix(BaseAttackSpeed val) => val.Value;
    public static implicit operator BaseAttackSpeed(fix val) => new BaseAttackSpeed() { Value = val };
}

public struct AttackSpeed : IComponentData
{
    public fix Value;

    public static implicit operator fix(AttackSpeed val) => val.Value;
    public static implicit operator AttackSpeed(fix val) => new AttackSpeed() { Value = val };
}