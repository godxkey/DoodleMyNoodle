using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// ComponentData found on controllers of controllable entities (like PawnController)
/// </summary>
[Serializable]
public struct ControlledEntity : IComponentData
{
    public Entity Value;

    public static implicit operator Entity(ControlledEntity val) => val.Value;
    public static implicit operator ControlledEntity(Entity val) => new ControlledEntity() { Value = val };
}
