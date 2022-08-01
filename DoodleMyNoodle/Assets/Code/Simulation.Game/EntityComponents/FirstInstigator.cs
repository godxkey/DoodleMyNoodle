
using Unity.Entities;

/// <summary>
/// The item, effect or actor that cause this entity to exist
/// </summary>
public struct FirstInstigator : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(FirstInstigator val) => val.Value;
    public static implicit operator FirstInstigator(Entity val) => new FirstInstigator() { Value = val };
}