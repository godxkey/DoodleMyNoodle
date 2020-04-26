using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

/* have a scene with a tilemap
 * use auth and ecs conversion to convert the tilemap in a real level
 * use this system to create all tile entities accordingly
 * things on tile are called tile addon
 */

public class CreateLevelGrid : SimComponentSystem
{
    const int GRID_SIZE = 11; // needs to be impair

    protected override void OnCreate()
    {
        base.OnCreate();

        // Creating singleton with a buffer of all tile entities
        CreateTileReferenceList();

        // middle row and same amount on each side
        int halfGridSize = (GRID_SIZE - 1) / 2;

        for (int l = -halfGridSize; l <= halfGridSize; l++)
        {
            for (int h = -halfGridSize; h <= halfGridSize; h++)
            {
                CreateTileEntity(new fix2() { x = l, y = h } );
            }
        }
    }

    protected override void OnUpdate() { }

    private void CreateTileReferenceList()
    {
        Entity singletonEntityWithTileList = EntityManager.CreateEntity(typeof(GridTileReference));
    }

    private void CreateTileEntity(fix2 tilePosition)
    {
        Entity newPlayerEntity = EntityManager.CreateEntity(typeof(FixTranslation),typeof(TileTag), typeof(EntitiesOnTile));

        EntityManager.SetComponentData(newPlayerEntity, new FixTranslation() { Value = new fix3(tilePosition.x, tilePosition.y,0) });

        DynamicBuffer<GridTileReference> gridTilesBuffer = EntityManager.GetBuffer<GridTileReference>(Accessor.GetSingletonEntity<GridTileReference>());

        gridTilesBuffer.Add(new GridTileReference() { Tile = newPlayerEntity });
    }
}

public partial class CommonReads
{
    public static Entity GetTile(fix2 gridPosition)
    {
        // TODO
        return Entity.Null;
    }

    public static List<Entity> GetEntitiesOnTile(fix2 gridPosition)
    {
        // TODO
        return null;
    }
}

internal partial class CommonWrites
{
    public static void AddEntityOnTile(Entity entity)
    {
        // TODO
    }

    public static void RemoveEntityOnTile(Entity entity)
    {
        // TODO
    }
}
