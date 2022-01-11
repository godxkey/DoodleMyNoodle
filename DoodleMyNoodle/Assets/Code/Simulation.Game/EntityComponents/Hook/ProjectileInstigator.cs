using Unity.Entities;

public struct ProjectileInstigator : IComponentData
{
    public Entity Value;

    public ProjectileInstigator(Entity Instigator)
    {
        Value = Instigator;
    }
}