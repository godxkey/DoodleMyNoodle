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
}
