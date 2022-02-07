using Unity.Entities;
using Unity.Mathematics;

public struct DelayedExplosion : IComponentData
{
    public fix TimeDuration;

    public fix Radius;
    public int Damage;
    public bool DestroyTiles;
}