
using Unity.Entities;

public struct OwnerPawn : IComponentData
{
    public Entity Value;

    public OwnerPawn(Entity Instigator)
    {
        Value = Instigator;
    }
}
