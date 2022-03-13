﻿using Unity.Entities;

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

public struct StopMoveFromTargetDistance : IComponentData
{
    public fix Value;

    public static implicit operator fix(StopMoveFromTargetDistance val) => val.Value;
    public static implicit operator StopMoveFromTargetDistance(fix val) => new StopMoveFromTargetDistance() { Value = val };
}