using Unity.Entities;
using Unity.Mathematics;

public struct DelayedExplosion : IComponentData
{
    public bool UseTime;
    public int TurnDuration;
    public fix TimeDuration;

    public fix Radius;
    public int Damage;
    public bool DestroyTiles;
}