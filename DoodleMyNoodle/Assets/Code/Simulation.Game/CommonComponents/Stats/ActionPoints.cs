using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(MaximumInt<ActionPoints>))]
[assembly: RegisterGenericComponentType(typeof(MinimumInt<ActionPoints>))]

public struct ActionPoints : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}