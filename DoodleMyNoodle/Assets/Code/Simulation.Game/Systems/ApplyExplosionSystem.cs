using CCC.Fix2D;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;

public struct ExplosionEventData : IComponentData
{
    public fix2 Position;
    public fix Radius;
}

public struct ExplosionRequestsSingletonTag : IComponentData
{
}

public struct ExplosionRequestData : IBufferElementData
{
    public Entity Instigator;
    public fix2 Position;
    public fix Radius;
    public int Damage;
    public bool DestroyTiles;
}

[UpdateBefore(typeof(ApplyImpulseSystem))]
public class ApplyExplosionSystem : SimSystemBase
{
    private const int IMPULSE_MAX = 4;
    private const int IMPULSE_MIN = 2;

    private EntityQuery _explosionEvents;
    private List<ExplosionEventData> _newExplosionEvents = new List<ExplosionEventData>();
    private NativeList<Entity> _entitiesToDamage;
    private ApplyImpulseSystem _applyImpulseSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _explosionEvents = GetEntityQuery(typeof(ExplosionEventData));
        _entitiesToDamage = new NativeList<Entity>(Allocator.Persistent);
        _applyImpulseSystem = World.GetOrCreateSystem<ApplyImpulseSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _entitiesToDamage.Dispose();
    }

    public void RequestExplosion(ExplosionRequestData request)
    {
        var buffer = GetExplosionRequestBuffer();
        buffer.Add(request);
    }

    private DynamicBuffer<ExplosionRequestData> GetExplosionRequestBuffer()
    {
        if (!HasSingleton<ExplosionRequestsSingletonTag>())
        {
            EntityManager.CreateEntity(typeof(ExplosionRequestsSingletonTag), typeof(ExplosionRequestData));
        }

        return GetBuffer<ExplosionRequestData>(GetSingletonEntity<ExplosionRequestsSingletonTag>());
    }

    protected override void OnUpdate()
    {
        // Clear Damage Applied Events
        EntityManager.DestroyEntity(_explosionEvents);

        DynamicBuffer<ExplosionRequestData> explosionRequestBuffer = GetExplosionRequestBuffer();

        if (explosionRequestBuffer.Length > 0)
        {
            NativeArray<ExplosionRequestData> requests = explosionRequestBuffer.ToNativeArray(Allocator.Temp);
            explosionRequestBuffer.Clear();
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

            foreach (ExplosionRequestData request in requests)
            {
                if (request.DestroyTiles)
                {
                    DestroyTiles(request.Position, request.Radius);
                }

                if (CommonReads.Physics.OverlapCircle(Accessor, request.Position, request.Radius, hits))
                {
                    CommonWrites.RequestDamage(Accessor, request.Instigator, hits, request.Damage);

                    // request impulses for every hit
                    foreach (DistanceHit hit in hits)
                    {
                        if (!HasComponent<PhysicsVelocity>(hit.Entity))
                        {
                            continue;
                        }

                        _applyImpulseSystem.RequestImpulseRadial(new RadialImpulseRequestData()
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

                _newExplosionEvents.Add(new ExplosionEventData() { Position = request.Position, Radius = request.Radius });
            }

            foreach (ExplosionEventData evnt in _newExplosionEvents)
            {
                EntityManager.CreateEventEntity(evnt);
            }
            _newExplosionEvents.Clear();
        }
    }

    private void DestroyTiles(fix2 position, fix radius)
    {
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);
        NativeList<int2> tiles = new NativeList<int2>(Allocator.Temp);

        var transformTileRequests = GetSystemRequests<SystemRequestTransformTile>();

        Job.WithCode(() =>
        {
            TilePhysics.GetAllTilesWithin(position, radius, tiles);

            for (int i = 0; i < tiles.Length; i++)
            {
                Entity tileEntity = tileWorld.GetEntity(tiles[i]);
                TileFlagComponent tileFlags = tileWorld.GetFlags(tileEntity);

                if (!tileFlags.IsOutOfGrid)
                {
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
    public static void RequestExplosion(ISimWorldReadWriteAccessor accessor, Entity instigator, fix2 position, fix radius, int damage, bool destroyTiles)
    {
        ExplosionRequestData request = new ExplosionRequestData()
        {
            Damage = damage,
            Radius = radius,
            Instigator = instigator,
            Position = position,
            DestroyTiles = destroyTiles
        };

        accessor.GetExistingSystem<ApplyExplosionSystem>().RequestExplosion(request);
    }
}