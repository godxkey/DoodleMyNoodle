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

public struct TileColliderTag : IComponentData { }
public struct TileTag : IComponentData { }

[UpdateInGroup(typeof(InitializationSystemGroup))]
public class CreateLevelGridSystem : SimComponentSystem
{
    private BlobAssetReference<Collider> _sharedTileCollider;

    public EntityArchetype TileArchetype { get; private set; }
    public EntityArchetype TileColliderArchetype { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
        RequireSingletonForUpdate<StartingTileActorElement>();

        TileArchetype = EntityManager.CreateArchetype(
            typeof(TileTag),
            typeof(TileActorReference),
            typeof(TileFlagComponent),
            typeof(TileId),
            typeof(SimAssetId));

        TileColliderArchetype = EntityManager.CreateArchetype(
            typeof(TileColliderTag),
            typeof(FixTranslation),
            typeof(PhysicsColliderBlob));

        // Create tile collider
        BoxGeometry boxGeometry = new BoxGeometry()
        {
            Size = float2(1, 1)
        };
        CollisionFilter filter = CollisionFilter.FromLayer(SimulationGameConstants.TERRAIN_PHYSICS_LAYER);
        PhysicsMaterial material = PhysicsMaterial.Default;

        _sharedTileCollider = Collider.Create(boxGeometry, filter, material);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _sharedTileCollider.Dispose();
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
        Entity newTileEntity = EntityManager.CreateEntity(TileArchetype);

        EntityManager.SetComponentData(newTileEntity, new TileFlagComponent() { Value = tileData.TileFlags });
        EntityManager.SetComponentData(newTileEntity, new TileId(tileData.Position));
        EntityManager.SetComponentData(newTileEntity, tileData.AssetId);

#if UNITY_EDITOR
        EntityManager.SetName(newTileEntity, $"Tile {tileData.Position.x}, {tileData.Position.y}");
#endif

        // Create tile collider
        if (tileData.TileFlags.IsTerrain)
        {
            Entity tileCollider = EntityManager.CreateEntity(TileColliderArchetype);

#if UNITY_EDITOR
            EntityManager.SetName(tileCollider, $"Tile Collider {tileData.Position.x}, {tileData.Position.y}");
#endif
            EntityManager.SetComponentData(tileCollider, new FixTranslation() { Value = Helpers.GetTileCenter(tileData.Position) });
            EntityManager.SetComponentData(tileCollider, new PhysicsColliderBlob() { Collider = _sharedTileCollider });
        }

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