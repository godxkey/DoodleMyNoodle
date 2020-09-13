using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Collections;
using Unity.Entities;
using System;
using UnityEngine;

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

    public static Entity FindFirstTileActorWithComponent<T1, T2>(ISimWorldReadAccessor accessor, int2 tile)
    {
        return FindFirstTileActorWithComponent<T1, T2>(accessor, GetTileWorld(accessor), tile);
    }

    public static Entity FindFirstTileActorWithComponent<T>(ISimWorldReadAccessor accessor, int2 tile)
    {
        return FindFirstTileActorWithComponent<T>(accessor, GetTileWorld(accessor), tile);
    }

    public static Entity FindFirstTileActorWithComponent<T1, T2>(ISimWorldReadAccessor accessor, in TileWorld tileWorld, int2 tile)
    {
        Entity entity = tileWorld.GetEntity(tile);
        if (entity != Entity.Null)
        {
            foreach (var actor in accessor.GetBufferReadOnly<TileActorReference>(entity))
            {
                if (accessor.HasComponent<T1>(actor) && accessor.HasComponent<T2>(actor))
                {
                    return actor;
                }
            }
        }

        return Entity.Null;
    }

    public static Entity FindFirstTileActorWithComponent<T>(ISimWorldReadAccessor accessor, in TileWorld tileWorld, int2 tile)
    {
        Entity entity = tileWorld.GetEntity(tile);
        if (entity != Entity.Null)
        {
            foreach (var actor in accessor.GetBufferReadOnly<TileActorReference>(entity))
            {
                if (accessor.HasComponent<T>(actor))
                {
                    return actor;
                }
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
    public static int2 GetTile(in FixTranslation translation) => floorToInt(translation.Value).xy;
    public static int2 GetTile(in fix3 worldPosition) => floorToInt(worldPosition).xy;
    public static int2 GetTile(in Vector3 worldPosition) => int2(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    public static int2 GetTile(in Vector2 worldPosition) => int2(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    
    public static fix3 GetTileCenter(in FixTranslation translation) => GetTileCenter(GetTile(translation));
    public static fix3 GetTileCenter(in fix3 worldPosition) => GetTileCenter(GetTile(worldPosition));
    public static fix3 GetTileCenter(in int2 tile) => fix3(tile.x + fix.Half, tile.y + fix.Half, 0);
}