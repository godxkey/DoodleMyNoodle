using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PathPosition : IBufferElementData
{
    public fix2 Position;

    public static implicit operator fix2(PathPosition val) => val.Position;
    public static implicit operator PathPosition(fix2 val) => new PathPosition() { Position = val };
}
