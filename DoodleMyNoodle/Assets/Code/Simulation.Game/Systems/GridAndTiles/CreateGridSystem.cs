using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using static fixMath;
using Unity.MathematicsX;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using UnityEngineX;
using CCC.Fix2D;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class CreateGridSystem : SimGameSystemBase
{
    public EntityArchetype TileArchetype { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
        RequireSingletonForUpdate<StartingTileActorElement>();

        TileArchetype = EntityManager.CreateArchetype(
            typeof(TileTag),
            typeof(TileColliderReference),
            typeof(TileFlagComponent),
            typeof(TileId),
            typeof(SimAssetId));
    }

    protected override void OnUpdate()
    {
        var gridInfo = GetSingleton<GridInfo>();

        Entity gridInfoEntity = GetSingletonEntity<GridInfo>();
        NativeArray<StartingTileActorElement> startingTileActors = EntityManager.GetBuffer<StartingTileActorElement>(gridInfoEntity, isReadOnly: true).ToNativeArray(Allocator.Temp);
        NativeArray<StartingTileElement> startingTiles = EntityManager.GetBuffer<StartingTileElement>(gridInfoEntity, isReadOnly: true).ToNativeArray(Allocator.Temp);

        // Spawn Actors
        for (int i = 0; i < startingTileActors.Length; i++)
        {
            if (!gridInfo.Contains(startingTileActors[i].Position))
            {
                Log.Warning($"Tile actor at position {startingTileActors[i].Position} is outside of the grid. It will not be spawned.");
                continue;
            }

            Entity tileActor = EntityManager.Instantiate(startingTileActors[i].Prefab);
            SetComponent<FixTranslation>(tileActor, Helpers.GetTileCenter(startingTileActors[i].Position));
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
        Entity newTileEntity = EntityManager.CreateEntity(TileArchetype);

        SetComponent(newTileEntity, new TileFlagComponent() { Value = tileData.TileFlags });
        SetComponent(newTileEntity, new TileId(tileData.Position));
        SetComponent(newTileEntity, tileData.AssetId);

#if UNITY_EDITOR
        EntityManager.SetName(newTileEntity, $"Tile_Entity {tileData.Position.x}, {tileData.Position.y}");
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
            TileFlags = TileFlagComponent.Empty,
            Position = position
        };
    }
}