using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Velocity : IComponentData
{
    public fix2 Value;

    public static implicit operator fix2(Velocity velocity) => velocity.Value;
    public static implicit operator Velocity(fix2 velocity) => new Velocity() { Value = velocity };
}