using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumFix<MoveEnergy>))]
[assembly: RegisterGenericComponentType(typeof(MinimumFix<MoveEnergy>))]

public struct MoveEnergy : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }

    public static implicit operator fix(MoveEnergy val) => val.Value;
    public static implicit operator MoveEnergy(fix val) => new MoveEnergy() { Value = val };
}