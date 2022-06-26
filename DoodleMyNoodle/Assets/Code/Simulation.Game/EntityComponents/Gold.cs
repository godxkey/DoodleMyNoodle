using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct Gold : IComponentData
{
    public fix Value;

    public static implicit operator fix(Gold val) => val.Value;
    public static implicit operator Gold(fix val) => new Gold() { Value = val };
}
