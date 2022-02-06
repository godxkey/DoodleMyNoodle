using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumFix<Shield>))]
[assembly: RegisterGenericComponentType(typeof(MaximumFix<Shield>))]

public struct Shield : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }

    public static implicit operator fix(Shield val) => val.Value;
    public static implicit operator Shield(fix val) => new Shield() { Value = val };
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