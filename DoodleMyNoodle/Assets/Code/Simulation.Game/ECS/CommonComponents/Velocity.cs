using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct Velocity : IComponentData
{
    public float3 Value;
}
