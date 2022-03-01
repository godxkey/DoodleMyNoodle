using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumFix<Health>))]
[assembly: RegisterGenericComponentType(typeof(MinimumFix<Health>))]

public struct Health : IComponentData, IStatFix
{
    public fix Value;

    fix IStatFix.Value { get => Value; set => Value = value; }

    public static implicit operator fix(Health val) => val.Value;
    public static implicit operator Health(fix val) => new Health() { Value = val };
}

public struct HealthProxy : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(HealthProxy val) => val.Value;
    public static implicit operator HealthProxy(Entity val) => new HealthProxy() { Value = val };
}

public struct HealthRechargeRate : IComponentData
{
    public fix Value;

    public static implicit operator fix(HealthRechargeRate val) => val.Value;
    public static implicit operator HealthRechargeRate(fix val) => new HealthRechargeRate() { Value = val };
}

public struct HealthRechargeCooldown : IComponentData
{
    public fix Value;

    public static implicit operator fix(HealthRechargeCooldown val) => val.Value;
    public static implicit operator HealthRechargeCooldown(fix val) => new HealthRechargeCooldown() { Value = val };
}

public struct HealthLastHitTime : IComponentData
{
    public fix Value;

    public static implicit operator fix(HealthLastHitTime val) => val.Value;
    public static implicit operator HealthLastHitTime(fix val) => new HealthLastHitTime() { Value = val };
}