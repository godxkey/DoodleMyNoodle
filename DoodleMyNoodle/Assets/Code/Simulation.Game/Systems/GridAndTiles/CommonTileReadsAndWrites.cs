using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Collections;
using Unity.Entities;
using System;
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

    //public static void FindBodies<T>(ISimWorldReadAccessor accessor, fix2 position, fix radius)
    //{
    //    var physicsWorld = accessor.GetExistingSystem<CCC.Fix2D.PhysicsWorldSystem>();

    //    CCC.Fix2D.Collider c;

    //    physicsWorld.PhysicsWorld.OverlapCollider
    //}
}

public partial class Helpers
{
    public static int2 GetTile(FixTranslation translation) => floorToInt(translation.Value).xy;
    public static int2 GetTile(fix2 worldPosition) => floorToInt(worldPosition).xy;
    public static int2 GetTile(Vector3 worldPosition) => int2(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    public static int2 GetTile(Vector2 worldPosition) => int2(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    
    public static TileId GetTileId(FixTranslation translation) => new TileId(floorToInt(translation.Value).xy);
    public static TileId GetTileId(fix2 worldPosition) => new TileId(floorToInt(worldPosition).xy);
    public static TileId GetTileId(Vector3 worldPosition) => new TileId(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));
    public static TileId GetTileId(Vector2 worldPosition) => new TileId(Mathf.FloorToInt(worldPosition.x), Mathf.FloorToInt(worldPosition.y));

    public static fix2 GetTileCenter(FixTranslation translation) => GetTileCenter(GetTile(translation));
    public static fix2 GetTileCenter(fix2 worldPosition) => GetTileCenter(GetTile(worldPosition));
    public static fix2 GetTileCenter(int2 tile) => fix2(tile.x + fix.Half, tile.y + fix.Half);
    public static fix2 GetTileCenter(TileId tileId) => fix2(tileId.X + fix.Half, tileId.Y + fix.Half);
}