using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PathPosition : IBufferElementData
{
    public fix3 Position;

    public static implicit operator fix3(PathPosition val) => val.Position;
    public static implicit operator PathPosition(fix3 val) => new PathPosition() { Position = val };
}
