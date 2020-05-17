using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using static fixMath;
using Unity.MathematicsX;

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
        Entity gridInfoEntity = Accessor.GetSingletonEntity<GridInfo>();

        DynamicBuffer<StartingTileAddonData> tileAddonsBuffer = Accessor.GetBufferReadOnly<StartingTileAddonData>(gridInfoEntity);
        NativeArray<StartingTileAddonData> tileAddons = tileAddonsBuffer.ToNativeArray(Allocator.Temp);

        intRect gridRect = Accessor.GetSingleton<GridInfo>().GridRect;

        // Spawn Addons
        NativeArray<Entity> tileAddonInstances = new NativeArray<Entity>(tileAddons.Length, Allocator.Temp);
        for (int i = 0; i < tileAddons.Length; i++)
        {
            tileAddonInstances[i] = Accessor.Instantiate(tileAddons[i].Prefab);
            Accessor.SetComponentData(tileAddonInstances[i], new FixTranslation() { Value = fix3(tileAddons[i].Position, 0) });
        }

        // Create All tiles
        // middle row and same amount on each side
        int halfGridSize = (gridRect.width - 1) / 2;

        for (int l = -halfGridSize; l <= halfGridSize; l++)
        {
            for (int h = -halfGridSize; h <= halfGridSize; h++)
            {
                CreateTileEntity(new fix2() { x = l, y = h }, tileAddonInstances);
            }
        }

        Accessor.RemoveComponent<StartingTileAddonData>(gridInfoEntity);
    }

    private void CreateTileReferenceList()
    {
        EntityManager.AddBuffer<GridTileReference>(Accessor.GetSingletonEntity<GridInfo>());
    }

    private void CreateTileEntity(fix2 tilePosition, NativeArray<Entity> tileAddonInstances)
    {
        // Create Tile
        Entity newTileEntity = EntityManager.CreateEntity(typeof(FixTranslation), typeof(TileTag), typeof(EntityOnTile));
        EntityManager.SetComponentData(newTileEntity, new FixTranslation() { Value = fix3(tilePosition, 0) });

#if UNITY_EDITOR
        EntityManager.SetName(newTileEntity, $"Tile {tilePosition.x}, {tilePosition.y}");
#endif

        // Add it to the list of Tiles
        DynamicBuffer<GridTileReference> gridTilesBuffer = EntityManager.GetBuffer<GridTileReference>(Accessor.GetSingletonEntity<GridInfo>());
        gridTilesBuffer.Add(new GridTileReference() { Tile = newTileEntity });

        // Add addons reference on the tile for those with the same position
        foreach (Entity addonInstance in tileAddonInstances)
        {
            if (Accessor.GetComponentData<FixTranslation>(addonInstance).Value == tilePosition)
            {
                CommonWrites.AddEntityOnTile(Accessor, addonInstance, newTileEntity);
            }
        }
    }
}

public partial class CommonReads
{
    public static Entity GetTile(ISimWorldReadAccessor accessor, fix2 gridPosition)
    {
        accessor.TryGetBufferReadOnly(accessor.GetSingletonEntity<GridInfo>(), out DynamicBuffer<GridTileReference> gridTileReferences);

        for (int i = 0; i < gridTileReferences.Length; i++)
        {
            Entity tileEntity = gridTileReferences[i].Tile;
            FixTranslation position = accessor.GetComponentData<FixTranslation>(tileEntity);
            if (gridPosition == position.Value)
            {
                return tileEntity;
            }
        }

        return Entity.Null;
    }

    public static List<Entity> GetEntitiesOnTile(ISimWorldReadAccessor accessor, Entity tile)
    {
        accessor.TryGetBufferReadOnly(tile, out DynamicBuffer<EntityOnTile> entities);

        List<Entity> allEntitiesOnTile = new List<Entity>();
        for (int i = 0; i < entities.Length; i++)
        {
            allEntitiesOnTile.Add(entities[i].TileEntity);
        }

        return allEntitiesOnTile;
    }
}

internal partial class CommonWrites
{
    public static void AddEntityOnTile(ISimWorldReadWriteAccessor accessor, Entity entity, Entity tile)
    {
        DynamicBuffer<EntityOnTile> entities = accessor.GetBuffer<EntityOnTile>(tile);
        entities.Add(new EntityOnTile() { TileEntity = entity });
    }

    public static void RemoveEntityOnTile(ISimWorldReadWriteAccessor accessor, Entity entity, Entity tile)
    {
        DynamicBuffer<EntityOnTile> entities = accessor.GetBuffer<EntityOnTile>(tile);
        for (int i = 0; i < entities.Length; i++)
        {
            if (entities[i].TileEntity == entity)
            {
                entities.RemoveAt(i);
            }
        }
    }
}
