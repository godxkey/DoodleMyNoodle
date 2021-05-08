using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

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
}

public class ApplyExplosionSystem : SimSystemBase
{
    private EntityQuery _explosionEvents;
    private List<ExplosionEventData> _newExplosionEvents = new List<ExplosionEventData>();
    private NativeList<Entity> _entitiesToDamage;

    protected override void OnCreate()
    {
        base.OnCreate();

        _explosionEvents = GetEntityQuery(typeof(ExplosionEventData));
        _entitiesToDamage = new NativeList<Entity>(Allocator.Persistent);
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

        DynamicBuffer<ExplosionRequestData> explosionRequests = GetExplosionRequestBuffer();

        if (explosionRequests.Length > 0)
        {
            NativeList<DistanceHit> hits = new NativeList<DistanceHit>(Allocator.Temp);

            foreach (ExplosionRequestData request in explosionRequests)
            {
                if (CommonReads.Physics.OverlapCircle(Accessor, request.Position, request.Radius, hits))
                {
                    CommonWrites.RequestDamage(Accessor, request.Instigator, hits, request.Damage);
                }

                _newExplosionEvents.Add(new ExplosionEventData() { Position = request.Position, Radius = request.Radius });
            }

            explosionRequests.Clear();

            foreach (ExplosionEventData evnt in _newExplosionEvents)
            {
                EntityManager.CreateEventEntity(evnt);
            }
            _newExplosionEvents.Clear();
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestExplosion(ISimWorldReadWriteAccessor accessor, Entity instigator, fix2 position, fix radius, int damage)
    {
        ExplosionRequestData request = new ExplosionRequestData()
        {
            Damage = damage,
            Radius = radius,
            Instigator = instigator,
            Position = position
        };

        accessor.GetExistingSystem<ApplyExplosionSystem>().RequestExplosion(request);
    }
}