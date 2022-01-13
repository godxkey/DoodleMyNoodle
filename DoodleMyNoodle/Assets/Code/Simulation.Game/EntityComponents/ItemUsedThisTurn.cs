using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct ItemUsedThisTurn : IComponentData
{
    public int Value;

    public static implicit operator int(ItemUsedThisTurn val) => val.Value;
    public static implicit operator ItemUsedThisTurn(int val) => new ItemUsedThisTurn() { Value = val };
}

public struct MaxItemUsesPerTurn : IComponentData
{
    public int Value;

    public static implicit operator int(MaxItemUsesPerTurn val) => val.Value;
    public static implicit operator MaxItemUsesPerTurn(int val) => new MaxItemUsesPerTurn() { Value = val };
}