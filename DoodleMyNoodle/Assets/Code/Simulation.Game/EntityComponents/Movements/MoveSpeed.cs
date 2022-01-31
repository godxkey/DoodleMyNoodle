using Unity.Entities;

public struct MoveSpeed : IComponentData
{
    public fix Value;

    public static implicit operator fix(MoveSpeed val) => val.Value;
    public static implicit operator MoveSpeed(fix val) => new MoveSpeed() { Value = val };
}

public struct CanMove : IComponentData
{
    public bool Value;

    public static implicit operator bool(CanMove val) => val.Value;
    public static implicit operator CanMove(bool val) => new CanMove() { Value = val };
}