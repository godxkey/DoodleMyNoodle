using Unity.Entities;

public struct DelayedExplosionShield : IComponentData
{
    public bool UseTime;
    public int TurnDuration;
    public fix TimeDuration;

    public fix Radius;
    public int RoundDuration;
}