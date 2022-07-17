using CCC.Fix2D;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;

public struct SingletonEventElementExplosion : ISingletonBufferElementData
{
    public fix2 Position;
    public fix Radius;
}

public struct SystemRequestExplosion : ISingletonBufferElementData
{
    public Entity LastInstigator;
    public fix2 Position;
    public fix Radius;
    public fix Damage;
    public bool DestroyTiles;
}

public partial class ApplyExplosionSystem : SimGameSystemBase
{
    private const int IMPULSE_MAX = 4;
    private const int IMPULSE_MIN = 2;

    private List<SingletonEventElementExplosion> _newExplosionEvents = new List<SingletonEventElementExplosion>();
    private NativeList<Entity> _entitiesToDamage;
    private ApplyImpulseSystem _applyImpulseSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _entitiesToDamage = new NativeList<Entity>(Allocator.Persistent);
        _applyImpulseSystem = World.GetOrCreateSystem<ApplyImpulseSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _entitiesToDamage.Dispose();
    }

    protected override void OnUpdate()
    {
        // Clear Damage Applied Events
        GetSingletonBuffer<SingletonEventElementExplosion>().Clear();

        DynamicBuffer<SystemRequestExplosion> explosionRequestBuffer = GetSingletonBuffer<SystemRequestExplosion>();

        if (explosionRequestBuffer.Length > 0)
        {
            NativeArray<SystemRequestExplosion> requests = explosionRequestBuffer.ToNativeArray(Allocator.Temp);
            explosionRequestBuffer.Clear();
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

            foreach (SystemRequestExplosion request in requests)
            {
                if (request.DestroyTiles)
                {
                    DestroyTiles(request.Position, request.Radius);
                }

                if (CommonReads.Physics.OverlapCircle(Accessor, request.Position, request.Radius, hits))
                {
                    DamageRequestSettings damageRequest = new DamageRequestSettings()
                    {
                        DamageAmount = request.Damage,
                        InstigatorSet = CommonReads.GetInstigatorSetFromLastPhysicalInstigator(Accessor, request.LastInstigator),
                        IsAutoAttack = false,
                    };

                    CommonWrites.RequestDamage(Accessor, damageRequest, hits);

                    if (request.Damage > 0)
                    {
                        var impulseRequests = GetSingletonBuffer<SystemRequestImpulseRadial>();

                        // request impulses for every hit
                        foreach (DistanceHit hit in hits)
                        {
                            if (!HasComponent<PhysicsVelocity>(hit.Entity))
                            {
                                continue;
                            }
                            impulseRequests.Add(new SystemRequestImpulseRadial()
                            {
                                StrengthMin = IMPULSE_MIN,
                                StrengthMax = IMPULSE_MAX,
                                IgnoreMass = false,
                                Position = request.Position,
                                Radius = request.Radius,
                                Target = hit.Entity
                            });
                        }
                    }
                }

                _newExplosionEvents.Add(new SingletonEventElementExplosion() { Position = request.Position, Radius = request.Radius });
            }

            var explosionEvents = GetSingletonBuffer<SingletonEventElementExplosion>();
            foreach (SingletonEventElementExplosion evnt in _newExplosionEvents)
            {
                explosionEvents.Add(evnt);
            }
            _newExplosionEvents.Clear();
        }
    }

    private void DestroyTiles(fix2 position, fix radius)
    {
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);
        NativeList<int2> tiles = new NativeList<int2>(Allocator.Temp);

        var transformTileRequests = GetSingletonBuffer<SystemRequestTransformTile>();

        Job.WithCode(() =>
        {
            TilePhysics.GetAllTilesWithin(position, radius, tiles);

            for (int i = 0; i < tiles.Length; i++)
            {
                Entity tileEntity = tileWorld.GetEntity(tiles[i]);
                TileFlagComponent tileFlags = tileWorld.GetFlags(tileEntity);

                if (!tileFlags.IsOutOfGrid && tileFlags.IsDestructible)
                {
                    // remove ladder and terrain flags
                    tileFlags.Value &= ~(TileFlags.Ladder | TileFlags.Terrain);
                    transformTileRequests.Add(new SystemRequestTransformTile()
                    {
                        Tile = tiles[i],
                        NewTileFlags = tileFlags
                    });
                }
            }
        }).Run();
    }
}

internal static partial class CommonWrites
{
    public static void RequestExplosion(ISimGameWorldReadWriteAccessor accessor, Entity instigator, fix2 position, fix radius, fix damage, bool destroyTiles)
    {
        SystemRequestExplosion request = new SystemRequestExplosion()
        {
            Damage = damage,
            Radius = radius,
            LastInstigator = instigator,
            Position = position,
            DestroyTiles = destroyTiles
        };

        accessor.GetSingletonBuffer<SystemRequestExplosion>().Add(request);
    }
}