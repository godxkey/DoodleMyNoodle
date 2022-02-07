using Unity.Entities;

public struct DelayedExplosionBlackHole : IComponentData
{
    public fix TimeDuration;

    public fix Radius;
    public bool CustomForce;
    public fix Force;
}