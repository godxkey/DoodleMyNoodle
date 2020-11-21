using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct Velocity : IComponentData
{
    public fix3 Value;

    public static implicit operator fix3(Velocity velocity) => velocity.Value;
    public static implicit operator Velocity(fix3 velocity) => new Velocity() { Value = velocity };
}