using Unity.Entities;

public struct HealthIncreaseMultiplier : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}