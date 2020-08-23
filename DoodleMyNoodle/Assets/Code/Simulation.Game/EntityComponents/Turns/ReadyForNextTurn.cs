using System;
using Unity.Entities;

[Serializable]
public struct ReadyForNextTurn : IComponentData
{
    public bool Value;

    public static implicit operator bool(ReadyForNextTurn val) => val.Value;
    public static implicit operator ReadyForNextTurn(bool val) => new ReadyForNextTurn() { Value = val };
}
