﻿using Unity.Entities;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngineX;

public struct SystemRequestTransformTile : ISingletonBufferElementData
{
    public int2 Tile;
    public TileFlagComponent NewTileFlags;
    public SimAssetId? ForcedNewSimAssetId; // leave null to let system pick sim asset id itself
}

public partial class UpdateGridSystem : SimGameSystemBase
{
    private BlobAssetReference<Collider> _fullTileCollider;
    private BlobAssetReference<Collider> _cornerTileCollider;
    private EntityArchetype _tileColliderArchetype;

    protected override void OnCreate()
    {
        base.OnCreate();

        _tileColliderArchetype = EntityManager.CreateArchetype(
            typeof(TileColliderTag),
            typeof(FixTranslation),
            typeof(PhysicsColliderBlob));

        // Create tile collider
        BoxGeometry boxGeometry = new BoxGeometry()
        {
            Size = float2(1, 1)
        };
        PolygonGeometry triangleGeometry = new PolygonGeometry()
        {
            Vertices = new NativeArray<float2>(new float2[] {
                new float2(-.5f, -.5f),
                new float2(-.5f, .5f),
                new float2(.5f, -0.5f),
            }, Allocator.Temp)
        };
        CollisionFilter filter = CollisionFilter.FromLayer(SimulationGameConstants.Physics.LAYER_TERRAIN);
        PhysicsMaterial material = PhysicsMaterial.Default;

        _fullTileCollider = Collider.Create(boxGeometry, filter, material);
        _cornerTileCollider = Collider.Create(triangleGeometry);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _fullTileCollider.Dispose();
        _cornerTileCollider.Dispose();
    }

    protected override void OnUpdate()
    {
        HandleTransformRequests();
        UpdateColliders();
    }

    private void HandleTransformRequests()
    {
        var requests = GetSingletonBuffer<SystemRequestTransformTile>();

        if (requests.Length <= 0)
            return;

        var tiles = new NativeArray<Entity>(requests.Length, Allocator.TempJob);
        var tileWorld = CommonReads.GetTileWorld(Accessor);
        DefaultTilesInfo defaultTileInfos = GetSingleton<DefaultTilesInfo>();

        var tileFlags = GetComponentDataFromEntity<TileFlagComponent>();
        var simAssetIds = GetComponentDataFromEntity<SimAssetId>();

        Job.WithCode(() =>
        {
            for (int i = 0; i < requests.Length; i++)
            {
                tiles[i] = tileWorld.GetEntity(requests[i].Tile);
            }

            for (int i = 0; i < requests.Length; i++)
            {
                var tile = tiles[i];

                if (!tileFlags.HasComponent(tile))
                    continue;

                var request = requests[i];
                var oldTileFlags = tileFlags[tile];

                // if flag is the same, skip
                if (oldTileFlags == request.NewTileFlags)
                    continue;

                // set new asset id, if needed
                if (request.ForcedNewSimAssetId.HasValue)
                {
                    if (simAssetIds[tile] != request.ForcedNewSimAssetId.Value)
                    {
                        simAssetIds[tile] = request.ForcedNewSimAssetId.Value;
                    }
                }
                else if (request.NewTileFlags.IsLadder)
                {
                    if (!oldTileFlags.IsLadder)
                    {
                        simAssetIds[tile] = defaultTileInfos.DefaultLadderTile;
                    }
                }
                else if (request.NewTileFlags.IsTerrain)
                {
                    if (!oldTileFlags.IsTerrain)
                    {
                        simAssetIds[tile] = defaultTileInfos.DefaultTerrainTile;
                    }
                }
                else if (request.NewTileFlags.IsEmpty)
                {
                    if (!oldTileFlags.IsEmpty)
                    {
                        simAssetIds[tile] = SimAssetId.Invalid;
                    }
                }

                tileFlags[tile] = request.NewTileFlags;
            }

            requests.Clear();
        }).Run();

        tiles.Dispose();
    }

    private void UpdateColliders()
    {
        Entities
            .WithChangeFilter<TileFlagComponent>()
            .ForEach((ref TileColliderReference colliderReference, in TileId tileId, in TileFlagComponent tileFlags) =>
            {
                bool shouldHaveCollider = tileFlags.IsTerrain;

                bool hasCollider = EntityManager.HasComponent<PhysicsColliderBlob>(colliderReference.ColliderEntity);
                if (shouldHaveCollider != hasCollider)
                {
                    if (shouldHaveCollider)
                    {
                        Entity colliderEntity = CreateTileColliderEntity(tileId, tileFlags);
                        colliderReference.ColliderEntity = colliderEntity;
                    }
                    else
                    {
                        EntityManager.DestroyEntity(colliderReference.ColliderEntity);
                        colliderReference.ColliderEntity = Entity.Null;
                    }
                }
            })
            .WithoutBurst()
            .WithStructuralChanges()
            .Run();
    }

    private Entity CreateTileColliderEntity(TileId tileId, TileFlagComponent tileFlag)
    {
        Entity tileCollider = EntityManager.CreateEntity(_tileColliderArchetype);

#if UNITY_EDITOR
        EntityManager.SetName(tileCollider, $"Tile_Collider ({tileId.X}, {tileId.Y})");
#endif
        EntityManager.SetComponentData(tileCollider, new FixTranslation() { Value = Helpers.GetTileCenter(tileId) });
        EntityManager.SetComponentData(tileCollider, new PhysicsColliderBlob() { Collider = tileFlag.IsShapeCornerAny ? _cornerTileCollider : _fullTileCollider });

        return tileCollider;
    }
}

internal static partial class CommonWrites
{
    public static void RequestTransformTile(ISimGameWorldReadWriteAccessor accessor, int2 tile, TileFlagComponent newTileFlags)
    {
        SystemRequestTransformTile request = new SystemRequestTransformTile()
        {
            Tile = tile,
            NewTileFlags = newTileFlags,
        };

        accessor.GetSingletonBuffer<SystemRequestTransformTile>().Add(request);
    }
}