using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Destination : IComponentData
{
    public fix3 Value;

    public static implicit operator fix3(Destination val) => val.Value;
    public static implicit operator Destination(fix3 val) => new Destination() { Value = val };
}
