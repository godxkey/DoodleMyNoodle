using Unity.Entities;
using Unity.Mathematics;

public struct DelayedExplosion : IComponentData
{
    public bool UseTime;
    public int TurnDuration;
    public fix TimeDuration;

    public int Range;
    public int Damage;
}