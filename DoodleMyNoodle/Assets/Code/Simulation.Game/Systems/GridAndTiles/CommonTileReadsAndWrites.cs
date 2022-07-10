using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using UnityEngine;
using CCC.Fix2D;

public partial class CommonReads
{
    public static fix3 GetFloorPlaneNormal(ISimWorldReadAccessor accessor) 
    {
        return fix3(0, 0, -1);
    }

    public static Entity GetTileEntity(ISimWorldReadAccessor accessor, int2 gridPosition)
    {
        GridInfo gridRect = accessor.GetSingleton<GridInfo>();

        if (!gridRect.Contains(gridPosition))
        {
            return Entity.Null;
        }

        int2 offset = gridPosition - gridRect.TileMin;
        int index = offset.x + (offset.y * gridRect.Width);

        var allTiles = accessor.GetBufferReadOnly<GridTileReference>(accessor.GetSingletonEntity<GridInfo>());
        return allTiles[index].Tile;
    }

    public static Entity GetTileEntity(int2 gridPosition, in GridInfo gridRect, DynamicBuffer<GridTileReference> tileReferences)
    {
        if (!gridRect.Contains(gridPosition))
        {
            return Entity.Null;
        }

        int2 offset = gridPosition - gridRect.TileMin;
        int index = offset.x + (offset.y * gridRect.Width);

        return tileReferences[index].Tile;
    }
}

public partial struct Helpers
{
    public static int2 GetTile(FixTranslation translation) => floorToInt(translation.Value).xy;
    public static int2 GetTile(fix2 worldPosition) => floorToInt(worldPosition).xy;
    public static int2 GetTile(Vector3 worldPosition) => int2(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    public static int2 GetTile(Vector2 worldPosition) => int2(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    public static int2 GetTile(TileId tileId) => int2(tileId.X, tileId.Y);
    
    public static TileId GetTileId(FixTranslation translation) => new TileId(floorToInt(translation.Value).xy);
    public static TileId GetTileId(fix2 worldPosition) => new TileId(floorToInt(worldPosition).xy);
    public static TileId GetTileId(Vector3 worldPosition) => new TileId(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    public static TileId GetTileId(Vector2 worldPosition) => new TileId(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));

    public static fix2 GetTileCenter(FixTranslation translation) => GetTileCenter(GetTile(translation));
    public static fix2 GetTileCenter(fix2 worldPosition) => GetTileCenter(GetTile(worldPosition));
    public static fix2 GetTileCenter(int2 tile) => fix2(tile.x + fix.Half, tile.y + fix.Half);
    public static fix2 GetTileCenter(TileId tileId) => fix2(tileId.X + fix.Half, tileId.Y + fix.Half);

    public static fix2 GetTileBottom(FixTranslation translation) => GetTileBottom(GetTile(translation));
    public static fix2 GetTileBottom(fix2 worldPosition) => GetTileBottom(GetTile(worldPosition));
    public static fix2 GetTileBottom(int2 tile) => fix2(tile.x + fix.Half, tile.y);
    public static fix2 GetTileBottom(TileId tileId) => fix2(tileId.X + fix.Half, tileId.Y);
}