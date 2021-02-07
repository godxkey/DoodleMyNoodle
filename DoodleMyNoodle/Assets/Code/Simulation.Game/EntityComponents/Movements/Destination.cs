using Unity.Entities;
using Unity.Mathematics;

[GenerateAuthoringComponent]
public struct Destination : IComponentData
{
    public fix2 Value;

    public static implicit operator fix2(Destination val) => val.Value;
    public static implicit operator Destination(fix2 val) => new Destination() { Value = val };
}
