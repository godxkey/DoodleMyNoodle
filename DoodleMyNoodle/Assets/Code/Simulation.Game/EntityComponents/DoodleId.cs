using System;
using Unity.Entities;
using Unity.Mathematics;

public struct DoodleId : IComponentData
{
    // fbessette: for memory, we might want to store guids elsewhere in the future (they are 128bit each)
    public Guid Guid;
}

public struct DoodleStartDirection : IComponentData
{
    public bool IsLookingRight;
}