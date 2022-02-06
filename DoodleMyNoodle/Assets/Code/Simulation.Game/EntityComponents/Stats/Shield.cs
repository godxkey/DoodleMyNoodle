using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumInt<Shield>))]
[assembly: RegisterGenericComponentType(typeof(MinimumInt<Shield>))]

public struct Shield : IComponentData, IStatInt
{
    public int Value;
    public int RechargeRate;
    public fix RechargeCooldown;

    int IStatInt.Value { get => Value; set => Value = value; }

    public static implicit operator int(Shield val) => val.Value;
    public static implicit operator Shield(int val) => new Shield() { Value = val };
}