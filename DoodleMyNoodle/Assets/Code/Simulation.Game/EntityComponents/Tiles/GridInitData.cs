using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Unity.MathematicsX;

public struct StartingTileActorElement : IBufferElementData
{
    public Entity Prefab;
    public int2 Position;
}

public struct StartingTileElement : IBufferElementData
{
    public SimAssetId AssetId;
    public TileFlagComponent TileFlags;
    public int2 Position;
}

public struct GridInfo : IComponentData
{
    public int2 TileMin;
    public int2 TileMax;

    public bool Contains(int2 tilePosition)
        => tilePosition.x >= TileMin.x &&
           tilePosition.x <= TileMax.x &&
           tilePosition.y >= TileMin.y &&
           tilePosition.y <= TileMax.y;

    public int2 Size => int2(Width, Height);
    public int Width => (TileMax.x - TileMin.x) + 1;
    public int Height => (TileMax.y - TileMin.y) + 1;
}
