
using Unity.Entities;

public struct SpellInstigator : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(SpellInstigator val) => val.Value;
    public static implicit operator SpellInstigator(Entity val) => new SpellInstigator() { Value = val };
}
