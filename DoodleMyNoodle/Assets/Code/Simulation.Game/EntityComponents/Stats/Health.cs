using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumInt<Health>))]
[assembly: RegisterGenericComponentType(typeof(MinimumInt<Health>))]

public struct Health : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }

    public static implicit operator int(Health val) => val.Value;
    public static implicit operator Health(int val) => new Health() { Value = val };
}