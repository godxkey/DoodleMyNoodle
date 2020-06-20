using System;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct PathPosition : IBufferElementData
{
    public fix3 Position;
}
