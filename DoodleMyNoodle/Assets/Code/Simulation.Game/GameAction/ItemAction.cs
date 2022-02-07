using Unity.Entities;

public struct ItemAction : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(ItemAction val) => val.Value;
    public static implicit operator ItemAction(Entity val) => new ItemAction() { Value = val };
}