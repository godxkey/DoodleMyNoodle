using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Collections;
using Unity.Entities;
using System;

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

    public static Entity FindFirstTileActorWithComponent<T>(ISimWorldReadAccessor accessor, Entity tile)
    {
        foreach (TileActorReference actor in accessor.GetBufferReadOnly<TileActorReference>(tile))
        {
            if (accessor.HasComponent<T>(actor))
            {
                return actor;
            }
        }

        return Entity.Null;
    }

    public static void FindTileActorsWithComponent<T>(ISimWorldReadAccessor accessor, Entity tile, NativeList<Entity> result)
    {
        foreach (TileActorReference actor in accessor.GetBufferReadOnly<TileActorReference>(tile))
        {
            if (accessor.HasComponent<T>(actor))
            {
                result.Add(actor);
            }
        }
    }

    public static void FindTileActors(ISimWorldReadAccessor accessor, Entity tile, NativeList<Entity> result, Predicate<Entity> predicate)
    {
        if (predicate is null)
        {
            throw new ArgumentNullException(nameof(predicate));
        }

        foreach (TileActorReference actor in accessor.GetBufferReadOnly<TileActorReference>(tile))
        {
            if (predicate(actor))
            {
                result.Add(actor);
            }
        }
    }

    public static Entity FindFirstTileActorWithComponent<T>(ISimWorldReadAccessor accessor, int2 tile)
    {
        var tileEntity = GetTileEntity(accessor, tile);
        if (tileEntity != Entity.Null)
        {
            return FindFirstTileActorWithComponent<T>(accessor, tileEntity);
        }
        else
        {
            return Entity.Null;
        }
    }

    public static void FindTileActorsWithComponent<T>(ISimWorldReadAccessor accessor, int2 tile, NativeList<Entity> result)
    {
        var tileEntity = GetTileEntity(accessor, tile);
        if (tileEntity != Entity.Null)
        {
            FindTileActorsWithComponent<T>(accessor, tileEntity, result);
        }
    }

    public static void FindTileActors(ISimWorldReadAccessor accessor, int2 tile, NativeList<Entity> result, Predicate<Entity> predicate)
    {
        var tileEntity = GetTileEntity(accessor, tile);
        if(tileEntity != Entity.Null)
        {
            FindTileActors(accessor, tileEntity, result, predicate);
        }
    }

    public static void FindTileActorsWithComponents<T>(ISimWorldReadAccessor accessor, int2 tile, NativeList<Entity> result)
        where T : struct, IComponentData
    {
        Entity tileEntity = GetTileEntity(accessor, tile);

        if(tileEntity == Entity.Null)
        {
            return;
        }

        foreach (var tileActor in accessor.GetBufferReadOnly<TileActorReference>(tileEntity))
        {
            if (accessor.HasComponent<T>(tileActor))
            {
                result.Add(tileActor);
            }
        }
    }
}

public partial class Helpers
{
    public static int2 GetTile(in FixTranslation translation) => roundToInt(translation.Value).xy;
    public static int2 GetTile(in fix3 worldPosition) => roundToInt(worldPosition).xy;
    public static fix3 GetTileCenter(in FixTranslation translation) => fix3(roundToInt(translation.Value).xy, 0);
    public static fix3 GetTileCenter(in fix3 worldPosition) => fix3(roundToInt(worldPosition).xy, 0);
    public static fix3 GetTileCenter(in int2 tile) => fix3(tile, 0);
}