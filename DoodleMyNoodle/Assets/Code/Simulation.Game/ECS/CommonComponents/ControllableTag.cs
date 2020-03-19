using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

/// <summary>
/// Indicates that an entity is controllable (like a pawn)
/// </summary>
[Serializable]
[GenerateAuthoringComponent]
public struct ControllableTag : IComponentData
{

}
