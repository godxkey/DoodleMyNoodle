using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using static fixMath;
using Unity.MathematicsX;
using Unity.Mathematics;
using UnityEngine.Profiling;

public class CreateLevelGridSystem : SimComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
        RequireSingletonForUpdate<StartingTileAddonData>();
    }

    protected override void OnUpdate()
    {
        // Creating singleton with a buffer of all tile entities (container of tiles)
        CreateTileReferenceList();

        // Setup All Grids Info
        Entity gridInfoEntity = GetSingletonEntity<GridInfo>();

        DynamicBuffer<StartingTileAddonData> tileAddonsBuffer = EntityManager.GetBufferReadOnly<StartingTileAddonData>(gridInfoEntity);
        NativeArray<StartingTileAddonData> tileAddons = tileAddonsBuffer.ToNativeArray(Allocator.Temp);

        intRect gridRect = GetSingleton<GridInfo>().GridRect;

        // Spawn Addons
        NativeArray<Entity> tileAddonInstances = new NativeArray<Entity>(tileAddons.Length, Allocator.Temp);
        for (int i = 0; i < tileAddons.Length; i++)
        {
            tileAddonInstances[i] = EntityManager.Instantiate(tileAddons[i].Prefab);
            EntityManager.SetComponentData(tileAddonInstances[i], new FixTranslation() { Value = fix3(tileAddons[i].Position, 0) });
        }

        // Create All tiles
        // middle row and same amount on each side
        int halfGridSize = (gridRect.width - 1) / 2;

        for (int h = -halfGridSize; h <= halfGridSize; h++)
        {
            for (int l = -halfGridSize; l <= halfGridSize; l++)
            {
                CreateTileEntity(new fix2() { x = l, y = h }, tileAddonInstances);
            }
        }

        EntityManager.RemoveComponent<StartingTileAddonData>(gridInfoEntity);
    }

    private void CreateTileReferenceList()
    {
        EntityManager.AddBuffer<GridTileReference>(GetSingletonEntity<GridInfo>());
    }

    private void CreateTileEntity(fix2 tilePosition, NativeArray<Entity> tileAddonInstances)
    {
        // Create Tile
        Entity newTileEntity = EntityManager.CreateEntity(typeof(FixTranslation), typeof(TileTag), typeof(TileAddonReference));
        EntityManager.SetComponentData(newTileEntity, new FixTranslation() { Value = fix3(tilePosition, 0) });

#if UNITY_EDITOR
        EntityManager.SetName(newTileEntity, $"Tile {tilePosition.x}, {tilePosition.y}");
#endif

        // Add it to the list of Tiles
        DynamicBuffer<GridTileReference> gridTilesBuffer = EntityManager.GetBuffer<GridTileReference>(GetSingletonEntity<GridInfo>());
        gridTilesBuffer.Add(new GridTileReference() { Tile = newTileEntity });

        // Add addons reference on the tile for those with the same position
        foreach (Entity addonInstance in tileAddonInstances)
        {
            if (EntityManager.GetComponentData<FixTranslation>(addonInstance).Value == tilePosition)
            {
                CommonWrites.AddTileAddon(Accessor, addonInstance, newTileEntity);
            }
        }
    }
}

public partial class CommonReads
{
    public static Entity GetTileEntity(ISimWorldReadAccessor accessor, int2 gridPosition)
    {
        GridInfo gridInfo = accessor.GetSingleton<GridInfo>();
        intRect gridRect = gridInfo.GridRect;

        var allTiles = accessor.GetBufferReadOnly<GridTileReference>(accessor.GetSingletonEntity<GridInfo>());

        int index = (gridPosition.x - gridRect.xMin) + ((gridPosition.y - gridRect.yMin) * gridRect.width);

        return index < 0 || index >= allTiles.Length ? Entity.Null : allTiles[index].Tile;
    }

    public static Entity GetFirstTileAddonWithComponent<T>(ISimWorldReadAccessor accessor, Entity tile) where T : IComponentData
    {
        foreach (TileAddonReference addon in accessor.GetBufferReadOnly<TileAddonReference>(tile))
        {
            if (accessor.HasComponent<T>(addon.Value))
            {
                return addon.Value;
            }
        }

        return Entity.Null;
    }
}

internal partial class CommonWrites
{
    public static void AddTileAddon(ISimWorldReadWriteAccessor accessor, Entity addonEntity, Entity tile)
    {
        DynamicBuffer<TileAddonReference> addons = accessor.GetBuffer<TileAddonReference>(tile);
        addons.Add(new TileAddonReference() { Value = addonEntity });
    }

    public static bool RemoveTileAddon(ISimWorldReadWriteAccessor accessor, Entity addonEntity, Entity tile)
    {
        DynamicBuffer<TileAddonReference> addons = accessor.GetBuffer<TileAddonReference>(tile);
        for (int i = 0; i < addons.Length; i++)
        {
            if (addons[i].Value == addonEntity)
            {
                addons.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}
