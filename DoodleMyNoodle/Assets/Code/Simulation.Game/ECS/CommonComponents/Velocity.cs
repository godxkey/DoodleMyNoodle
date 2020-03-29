using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
[GenerateAuthoringComponent]
public struct Velocity : IComponentData
{
    public FixVector3 Value;
}
