
using Unity.Entities;

public struct FirstInstigator : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(FirstInstigator val) => val.Value;
    public static implicit operator FirstInstigator(Entity val) => new FirstInstigator() { Value = val };
}