using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System;

public struct ReboundOnOverlapWithTileSetting : IComponentData
{
    public int ReboundMax;
}

public struct ReboundOnOverlapWithTileState : IComponentData
{
    public TimeValue LastCollisionTime;
    public fix2 PreviousTile;
    public int ReboundCount;
}