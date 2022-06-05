using Unity.Entities;

public struct BaseMoveSpeed : IComponentData
{
    public fix Value;

    public static implicit operator fix(BaseMoveSpeed val) => val.Value;
    public static implicit operator BaseMoveSpeed(fix val) => new BaseMoveSpeed() { Value = val };
}

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

public struct DesiredRangeFromTarget : IComponentData
{
    public FixRange Value;

    public static implicit operator FixRange(DesiredRangeFromTarget val) => val.Value;
    public static implicit operator DesiredRangeFromTarget(FixRange val) => new DesiredRangeFromTarget() { Value = val };
}

public struct KeepWalkingAfterPeriodicAction : IComponentData { }
