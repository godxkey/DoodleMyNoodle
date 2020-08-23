using Unity.Entities;
using Unity.Mathematics;

public struct ItemStackableData : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}