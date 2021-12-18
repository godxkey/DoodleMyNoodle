using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct MoveCost : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }

    public static implicit operator fix(MoveCost val) => val.Value;
    public static implicit operator MoveCost(fix val) => new MoveCost() { Value = val };
}