using Unity.Entities;

public struct HealthIncreaseMultiplier : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }
}