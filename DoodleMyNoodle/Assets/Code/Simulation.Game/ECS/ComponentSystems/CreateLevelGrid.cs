using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;

public class CreateLevelGrid : SimComponentSystem
{
    protected override void OnUpdate() 
    {
        if(!Accessor.HasSingleton<GridTag>() && Accessor.HasSingleton<GridInfoContainer>())
        {
            // Creating singleton with a buffer of all tile entities (container of tiles)
            CreateTileReferenceList();

            // Setup All Grids Info
            Entity GridInfoEntity = Accessor.GetSingletonEntity<GridInfoContainer>();

            Accessor.TryGetBufferReadOnly(GridInfoEntity, out DynamicBuffer<StartingTileAddonData> tileAddonsBuffer);
            NativeArray<StartingTileAddonData> tileAddons = tileAddonsBuffer.ToNativeArray(Allocator.Temp);

            int GridSize = Accessor.GetComponentData<GridInfoContainer>(GridInfoEntity).GridSize;

            // Spawn Addons
            NativeArray<Entity> tileAddonInstances = new NativeArray<Entity>(tileAddons.Length, Allocator.Temp);
            for (int i = 0; i < tileAddons.Length; i++)
            {
                tileAddonInstances[i] = Accessor.Instantiate(tileAddons[i].Prefab);
                Accessor.SetComponentData(tileAddonInstances[i], new FixTranslation() { Value = new fix3(tileAddons[i].Position.x, tileAddons[i].Position.y, 0) });
            }

            // Create All tiles
            // middle row and same amount on each side
            int halfGridSize = (GridSize - 1) / 2;

            for (int l = -halfGridSize; l <= halfGridSize; l++)
            {
                for (int h = -halfGridSize; h <= halfGridSize; h++)
                {
                    CreateTileEntity(new fix2() { x = l, y = h }, tileAddonInstances);
                }
            }

            // Clean up
            tileAddonInstances.Dispose();
            tileAddons.Dispose();
        }
    }

    private void CreateTileReferenceList()
    {
        Accessor.SetOrCreateSingleton(new GridTag());
        EntityManager.AddBuffer<GridTileReference>(Accessor.GetSingletonEntity<GridTag>());
    }

    private void CreateTileEntity(fix2 tilePosition, NativeArray<Entity> tileAddonInstances)
    {
        // Create Tile
        Entity newTileEntity = EntityManager.CreateEntity(typeof(FixTranslation),typeof(TileTag));
        EntityManager.AddBuffer<EntitiesOnTile>(newTileEntity);
        EntityManager.SetComponentData(newTileEntity, new FixTranslation() { Value = new fix3(tilePosition.x, tilePosition.y,0) });

        // Add it to the list of Tiles
        DynamicBuffer<GridTileReference> gridTilesBuffer = EntityManager.GetBuffer<GridTileReference>(Accessor.GetSingletonEntity<GridTag>());
        gridTilesBuffer.Add(new GridTileReference() { Tile = newTileEntity });

        // Add addons reference on the tile for those with the same position
        foreach (Entity addonInstance in tileAddonInstances)
        {
            if(Accessor.GetComponentData<FixTranslation>(addonInstance).Value == tilePosition)
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
        accessor.TryGetBufferReadOnly(accessor.GetSingletonEntity<GridTag>(), out DynamicBuffer<GridTileReference> gridTileReferences);

        for (int i = 0; i < gridTileReferences.Length; i++)
        {
            Entity tileEntity = gridTileReferences[i].Tile;
            FixTranslation position = accessor.GetComponentData<FixTranslation>(tileEntity);
            if(gridPosition == position.Value)
            {
                return tileEntity;
            }   
        }

        return Entity.Null;
    }

    public static List<Entity> GetEntitiesOnTile(ISimWorldReadAccessor accessor, Entity tile)
    {
        accessor.TryGetBufferReadOnly(tile, out DynamicBuffer<EntitiesOnTile> entities);

        List<Entity> allEntitiesOnTile = new List<Entity>();
        for (int i = 0; i < entities.Length; i++)
        {
            allEntitiesOnTile.Add(entities[i].EntityOnTile);
        }

        return allEntitiesOnTile;
    }
}

internal partial class CommonWrites
{
    public static void AddEntityOnTile(ISimWorldReadWriteAccessor accessor, Entity entity, Entity tile)
    {
        DynamicBuffer<EntitiesOnTile> entities = accessor.GetBuffer<EntitiesOnTile>(tile);
        entities.Add(new EntitiesOnTile() { EntityOnTile = entity });
    }

    public static void RemoveEntityOnTile(ISimWorldReadWriteAccessor accessor, Entity entity, Entity tile)
    {
        DynamicBuffer<EntitiesOnTile> entities = accessor.GetBuffer<EntitiesOnTile>(tile);
        for (int i = 0; i < entities.Length; i++)
        {
            if(entities[i].EntityOnTile == entity)
            {
                entities.RemoveAt(i);
            }
        }
    }
}
