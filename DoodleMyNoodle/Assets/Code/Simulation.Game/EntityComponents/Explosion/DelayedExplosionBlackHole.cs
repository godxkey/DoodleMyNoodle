using Unity.Entities;

public struct DelayedExplosionBlackHole : IComponentData
{
    public bool UseTime;
    public int TurnDuration;
    public fix TimeDuration;

    public fix Radius;
    public bool CustomForce;
    public fix Force;
}