using Unity.Entities;

public struct DelayedExplosionShield : IComponentData
{
    public fix TimeDuration;
    public fix Radius;
    public fix ShieldDuration;
}