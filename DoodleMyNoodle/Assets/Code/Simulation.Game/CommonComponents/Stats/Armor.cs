using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumInt<Armor>))]
[assembly: RegisterGenericComponentType(typeof(MinimumInt<Armor>))]

public struct Armor : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}