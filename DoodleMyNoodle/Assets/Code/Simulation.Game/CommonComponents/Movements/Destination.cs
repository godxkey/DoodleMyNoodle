using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Destination : IComponentData
{
    public fix3 Value;
}
