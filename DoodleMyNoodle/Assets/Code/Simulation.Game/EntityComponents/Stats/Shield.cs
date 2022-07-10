using Unity.Entities;

public struct Shield : IComponentData
{
    public fix Value;

    public static implicit operator fix(Shield val) => val.Value;
    public static implicit operator Shield(fix val) => new Shield() { Value = val };
}

public struct ShieldMax : IComponentData
{
    public fix Value;

    public static implicit operator fix(ShieldMax val) => val.Value;
    public static implicit operator ShieldMax(fix val) => new ShieldMax() { Value = val };
}

public struct ShieldRechargeRate : IComponentData
{
    public fix Value;

    public static implicit operator fix(ShieldRechargeRate val) => val.Value;
    public static implicit operator ShieldRechargeRate(fix val) => new ShieldRechargeRate() { Value = val };
}

public struct ShieldRechargeCooldown : IComponentData
{
    public fix Value;

    public static implicit operator fix(ShieldRechargeCooldown val) => val.Value;
    public static implicit operator ShieldRechargeCooldown(fix val) => new ShieldRechargeCooldown() { Value = val };
}

public struct ShieldLastHitTime : IComponentData
{
    public fix Value;

    public static implicit operator fix(ShieldLastHitTime val) => val.Value;
    public static implicit operator ShieldLastHitTime(fix val) => new ShieldLastHitTime() { Value = val };
}