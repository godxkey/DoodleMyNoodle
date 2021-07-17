using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Indicates that an entity is controllable (like a pawn)
/// </summary>
[Serializable]
public struct Controllable : IComponentData
{
    public Entity CurrentController;

    public static implicit operator Entity(Controllable val) => val.CurrentController;
    public static implicit operator Controllable(Entity val) => new Controllable() { CurrentController = val };
}

/// <summary>
/// Indicates a controllable entity's default controller
/// </summary>
[Serializable]
public struct DefaultControllerPrefab : IComponentData
{
    public Entity Value;
}