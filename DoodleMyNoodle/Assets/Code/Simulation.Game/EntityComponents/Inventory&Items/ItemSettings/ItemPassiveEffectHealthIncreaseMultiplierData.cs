using Unity.Entities;

public struct ItemPassiveEffectHealthIncreaseMultiplierData : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }
}