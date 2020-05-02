using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

public class CreateLevelGrid : SimComponentSystem
{
    // todo change this
    public const int GRID_MAX_SIZE = 11; // needs to be impair

    protected override void OnUpdate() 
    {
        if(!Accessor.HasSingleton<GridTag>())
        {
            // Creating singleton with a buffer of all tile entities
            CreateTileReferenceList();

            // middle row and same amount on each side
            int halfGridSize = (GRID_MAX_SIZE - 1) / 2;

            for (int l = -halfGridSize; l <= halfGridSize; l++)
            {
                for (int h = -halfGridSize; h <= halfGridSize; h++)
                {
                    CreateTileEntity(new fix2() { x = l, y = h });
                }
            }
        }
    }

    private void CreateTileReferenceList()
    {
        Accessor.SetOrCreateSingleton(new GridTag());
        EntityManager.AddBuffer<GridTileReference>(Accessor.GetSingletonEntity<GridTag>());
    }

    private void CreateTileEntity(fix2 tilePosition)
    {
        Entity newPlayerEntity = EntityManager.CreateEntity(typeof(FixTranslation),typeof(TileTag));
        EntityManager.AddBuffer<EntitiesOnTile>(newPlayerEntity);

        EntityManager.SetComponentData(newPlayerEntity, new FixTranslation() { Value = new fix3(tilePosition.x, tilePosition.y,0) });

        DynamicBuffer<GridTileReference> gridTilesBuffer = EntityManager.GetBuffer<GridTileReference>(Accessor.GetSingletonEntity<GridTag>());

        gridTilesBuffer.Add(new GridTileReference() { Tile = newPlayerEntity });
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
