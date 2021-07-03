using Unity.Entities;

public struct MoveSpeed : IComponentData
{
    public fix Value;

    public static implicit operator fix(MoveSpeed val) => val.Value;
    public static implicit operator MoveSpeed(fix val) => new MoveSpeed() { Value = val };
}
