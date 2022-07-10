using Unity.Entities;

public struct BaseHealthMax : IComponentData
{
    public fix Value;

    public static implicit operator fix(BaseHealthMax val) => val.Value;
    public static implicit operator BaseHealthMax(fix val) => new BaseHealthMax() { Value = val };
}

public struct Health : IComponentData
{
    public fix Value;

    public static implicit operator fix(Health val) => val.Value;
    public static implicit operator Health(fix val) => new Health() { Value = val };
}

public struct HealthMax : IComponentData
{
    public fix Value;

    public static implicit operator fix(HealthMax val) => val.Value;
    public static implicit operator HealthMax(fix val) => new HealthMax() { Value = val };
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