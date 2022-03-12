using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumFix<ActionPoints>))]
[assembly: RegisterGenericComponentType(typeof(MinimumFix<ActionPoints>))]

public struct ActionPoints : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }

    public static implicit operator fix(ActionPoints val) => val.Value;
    public static implicit operator ActionPoints(fix val) => new ActionPoints() { Value = val };
}

public struct ActionPointsRechargeRate : IComponentData
{
    public fix Value;

    public static implicit operator fix(ActionPointsRechargeRate val) => val.Value;
    public static implicit operator ActionPointsRechargeRate(fix val) => new ActionPointsRechargeRate() { Value = val };
}

public struct ActionPointsRechargeCooldown : IComponentData
{
    public fix Value;
    public fix LastTime;

    public static implicit operator fix(ActionPointsRechargeCooldown val) => val.Value;
    public static implicit operator ActionPointsRechargeCooldown(fix val) => new ActionPointsRechargeCooldown() { Value = val };
}