using Unity.Entities;

public struct ActionPoints : IComponentData
{
    public fix Value;

    public static implicit operator fix(ActionPoints val) => val.Value;
    public static implicit operator ActionPoints(fix val) => new ActionPoints() { Value = val };
}

public struct ActionPointsMax : IComponentData
{
    public fix Value;

    public static implicit operator fix(ActionPointsMax val) => val.Value;
    public static implicit operator ActionPointsMax(fix val) => new ActionPointsMax() { Value = val };
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