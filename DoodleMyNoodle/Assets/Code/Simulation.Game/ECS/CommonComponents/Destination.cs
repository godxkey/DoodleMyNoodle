using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Destination : IComponentData
{
    public FixVector3 Value;
}
