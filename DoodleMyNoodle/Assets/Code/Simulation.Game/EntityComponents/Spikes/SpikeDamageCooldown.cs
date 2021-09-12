using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;

public struct SpikeDamageCooldown : IComponentData
{
    public fix ElapsedTimeWhenCreated;
    public fix Cooldown;
    public Entity EntityLinkedTo;
}