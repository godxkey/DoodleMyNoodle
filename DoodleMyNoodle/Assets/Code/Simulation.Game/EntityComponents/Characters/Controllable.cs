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
}

/// <summary>
/// Indicates a controllable entity's default controller
/// </summary>
[Serializable]
public struct DefaultControllerPrefab : IComponentData
{
    public Entity Value;
}

/// <summary>
/// An entity with this tag will have its default controller instantiated
/// </summary>
[Serializable]
public struct InstantiateAndUseDefaultControllerTag : IComponentData
{
}