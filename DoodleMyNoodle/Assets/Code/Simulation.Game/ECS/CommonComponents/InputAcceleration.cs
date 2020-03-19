using System;
using Unity.Entities;
using Unity.Mathematics;

// temporary testing code
[Serializable]
[GenerateAuthoringComponent]
public struct InputAcceleration : IComponentData
{
    public float3 Value;
}
