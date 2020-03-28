using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumInt<Health>))]
[assembly: RegisterGenericComponentType(typeof(MinimumInt<Health>))]

public struct Health : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}