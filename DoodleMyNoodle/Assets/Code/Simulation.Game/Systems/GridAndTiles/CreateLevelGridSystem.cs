using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using static fixMath;
using Unity.MathematicsX;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngineX;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CreateLevelGridSystem : SimComponentSystem
{
    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
        RequireSingletonForUpdate<StartingTileActorElement>();
    }

    protected override void OnUpdate()
    {
        var gridInfo = GetSingleton<GridInfo>();

        Entity gridInfoEntity = GetSingletonEntity<GridInfo>();
        NativeArray<StartingTileActorElement> startingTileActors = EntityManager.GetBufferReadOnly<StartingTileActorElement>(gridInfoEntity).ToNativeArray(Allocator.Temp);
        NativeArray<StartingTileElement> startingTiles = EntityManager.GetBufferReadOnly<StartingTileElement>(gridInfoEntity).ToNativeArray(Allocator.Temp);

        // Spawn Actors
        for (int i = 0; i < startingTileActors.Length; i++)
        {
            if (!gridInfo.Contains(startingTileActors[i].Position))
            {
                Log.Warning($"Tile actor at position {startingTileActors[i].Position} is outside of the grid. It will not be spawned.");
                continue;
            }

            Entity tileActor = EntityManager.Instantiate(startingTileActors[i].Prefab);
            EntityManager.SetComponentData<FixTranslation>(tileActor, Helpers.GetTileCenter(startingTileActors[i].Position));
        }

        // Creating singleton with a buffer of all tile entities (container of tiles)

        // middle row and same amount on each side

        NativeList<GridTileReference> tiles = new NativeList<GridTileReference>(Allocator.Temp);

        for (int y = gridInfo.TileMin.y; y <= gridInfo.TileMax.y; y++)
        {
            for (int x = gridInfo.TileMin.x; x <= gridInfo.TileMax.x; x++)
            {
                int2 pos = int2(x, y);
                StartingTileElement tileData = FindStartingTileData(startingTiles, pos);
                GridTileReference gridTileReference = CreateTileEntity(tileData);
                tiles.Add(gridTileReference);
            }
        }

        EntityManager.RemoveComponent<StartingTileActorElement>(gridInfoEntity);
        EntityManager.AddBuffer<GridTileReference>(gridInfoEntity).AddRange(tiles);
    }

    private Entity CreateTileEntity(StartingTileElement tileData)
    {
        // Create Tile
        Entity newTileEntity = EntityManager.CreateEntity(
            typeof(TileTag),
            typeof(TileActorReference),
            typeof(TileFlagComponent),
            typeof(TileId),
            typeof(SimAssetId));

        EntityManager.SetComponentData(newTileEntity, new TileFlagComponent() { Value = tileData.TileFlags });
        EntityManager.SetComponentData(newTileEntity, new TileId(tileData.Position));
        EntityManager.SetComponentData(newTileEntity, tileData.AssetId);

#if UNITY_EDITOR
        EntityManager.SetName(newTileEntity, $"Tile {tileData.Position.x}, {tileData.Position.y}");
#endif

        return newTileEntity;
    }

    private StartingTileElement FindStartingTileData(NativeArray<StartingTileElement> tiles, int2 position)
    {
        foreach (StartingTileElement tile in tiles)
        {
            if (tile.Position.Equals(position))
            {
                return tile;
            }
        }
        
        return new StartingTileElement()
        {
            AssetId = SimAssetId.Invalid,
            TileFlags = TileFlags.Empty,
            Position = position
        };
    }
}