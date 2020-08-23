using Unity.Entities;
using Unity.Mathematics;

public struct Active : IComponentData
{
    public bool Value;

    public static implicit operator bool(Active active) => active.Value;
    public static implicit operator Active(bool active) => new Active() { Value = active };
}