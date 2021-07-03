using Unity.Entities;

public struct MoveInput : IComponentData
{
    public fix2 Value;

    public static implicit operator fix2(MoveInput val) => val.Value;
    public static implicit operator MoveInput(fix2 val) => new MoveInput() { Value = val };
}
