using Unity.Entities;

[GenerateAuthoringComponent]
public struct GameActionThrowProjectileSettings : IComponentData
{
    public Entity ProjectilePrefab;
    public fix ThrowSpeed;
}
