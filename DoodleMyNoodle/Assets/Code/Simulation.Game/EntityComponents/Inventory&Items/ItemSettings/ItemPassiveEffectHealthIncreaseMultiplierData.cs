using Unity.Entities;

public struct ItemPassiveEffectHealthIncreaseMultiplierData : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}