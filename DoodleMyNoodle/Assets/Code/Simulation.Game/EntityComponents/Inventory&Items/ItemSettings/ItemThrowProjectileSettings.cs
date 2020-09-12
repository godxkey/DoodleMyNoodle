using Unity.Entities;

[GenerateAuthoringComponent]
public struct ItemThrowProjectileSettings : IComponentData
{
    public Entity ProjectilePrefab;
    public fix ThrowSpeed;
}
