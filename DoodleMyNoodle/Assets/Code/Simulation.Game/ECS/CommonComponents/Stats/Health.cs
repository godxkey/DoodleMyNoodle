using Unity.Entities;

[assembly: RegisterGenericComponentType(typeof(Maximum<Health>))]
[assembly: RegisterGenericComponentType(typeof(Minimum<Health>))]

[GenerateAuthoringComponent]
public struct Health : IComponentData, IStatInt
{
    public int Value;

    int IStatInt.Value { get => Value; set => Value = value; }
}