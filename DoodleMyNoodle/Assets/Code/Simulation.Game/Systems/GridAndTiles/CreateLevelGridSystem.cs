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
    public int GridVersion { get; private set; }

    private NativeArray<GridTileReference> _tileReferencesBuffer;
    public GridInfo GridInfo;

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<GridInfo>();
        RequireSingletonForUpdate<StartingTileActors>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (_tileReferencesBuffer.IsCreated)
        {
            _tileReferencesBuffer.Dispose();
        }
    }

    public TileWorld GetTileWorld()
    {
        return new TileWorld(
            GridInfo,
            _tileReferencesBuffer,
            GetComponentDataFromEntity<TileFlagComponent>());
    }

    protected override void OnUpdate()
    {
        GridVersion++;

        GridInfo = GetSingleton<GridInfo>();

        Entity gridInfoEntity = GetSingletonEntity<GridInfo>();
        NativeArray<StartingTileActors> startingTileActors = EntityManager.GetBufferReadOnly<StartingTileActors>(gridInfoEntity).ToNativeArray(Allocator.Temp);

        // Spawn Actors
        NativeArray<Entity> tileActors = new NativeArray<Entity>(startingTileActors.Length, Allocator.Temp);
        for (int i = 0; i < startingTileActors.Length; i++)
        {
            if (!GridInfo.Contains(startingTileActors[i].Position))
            {
                Log.Warning($"Tile actor at position {startingTileActors[i].Position} is outside of the grid. It will not be spawned.");
                continue;
            }

            tileActors[i] = EntityManager.Instantiate(startingTileActors[i].Prefab);
            EntityManager.SetComponentData(tileActors[i], new FixTranslation() { Value = Helpers.GetTileCenter(startingTileActors[i].Position) });
        }

        // Creating singleton with a buffer of all tile entities (container of tiles)

        // middle row and same amount on each side

        NativeList<GridTileReference> tiles = new NativeList<GridTileReference>(Allocator.Temp);

        for (int y = GridInfo.TileMin.y; y <= GridInfo.TileMax.y; y++)
        {
            for (int x = GridInfo.TileMin.x; x <= GridInfo.TileMax.x; x++)
            {
                int2 pos = int2(x, y);
                GridTileReference gridTileReference = CreateTileEntity(pos, FindTileFlags(startingTileActors, pos));
                tiles.Add(gridTileReference);
            }
        }

        EntityManager.RemoveComponent<StartingTileActors>(gridInfoEntity);
        EntityManager.AddBuffer<GridTileReference>(gridInfoEntity).AddRange(tiles);

        if (_tileReferencesBuffer.IsCreated)
        {
            _tileReferencesBuffer.Dispose();
        }

        _tileReferencesBuffer = EntityManager.GetBufferReadOnly<GridTileReference>(GetSingletonEntity<GridInfo>()).ToNativeArray(Allocator.Persistent);
    }

    private Entity CreateTileEntity(int2 tilePosition, TileFlags tileFlags)
    {
        // Create Tile
        Entity newTileEntity = EntityManager.CreateEntity(
            typeof(TileTag),
            typeof(TileActorReference),
            typeof(TileFlagComponent));

        EntityManager.SetComponentData(newTileEntity, new TileFlagComponent() { Value = tileFlags });

#if UNITY_EDITOR
        EntityManager.SetName(newTileEntity, $"Tile {tilePosition.x}, {tilePosition.y}");
#endif

        return newTileEntity;
    }

    private TileFlags FindTileFlags(NativeArray<StartingTileActors> tileActors, int2 position)
    {
        foreach (StartingTileActors actor in tileActors)
        {
            if (actor.Position.Equals(position))
            {
                if (EntityManager.HasComponent<TerrainTag>(actor.Prefab))
                {
                    return TileFlags.Terrain;
                }

                if (EntityManager.HasComponent<LadderTag>(actor.Prefab))
                {
                    return TileFlags.Ladder;
                }
            }
        }
        return TileFlags.Empty;
    }
}

public partial class CommonReads
{
    public static TileWorld GetTileWorld(ISimWorldReadAccessor accessor)
    {
        return accessor.GetExistingSystem<CreateLevelGridSystem>().GetTileWorld();
    }

    public static bool IsTileValid(ISimWorldReadAccessor accessor, int2 tile)
    {
        return accessor.GetExistingSystem<CreateLevelGridSystem>().GridInfo.Contains(tile);
    }
}