using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct ExplosionEventData : IComponentData
{
    public int2 ExplodedTile;
}

public struct ExplosionRequestsSingletonTag : IComponentData
{
}

public struct ExplosionRequestData : IBufferElementData
{
    public Entity Instigator;
    public int2 TilePos;
    public int Damage;
}

public class ApplyExplosionSystem : SimSystemBase
{
    EntityQuery _explosionEvents;

    private List<int2> _newExplodeLocations = new List<int2>();
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

    public void RequestExplosion(Entity instigator, int2 tilePos, int range, int damage)
    {
        var buffer = GetExplosionRequestBuffer();
        if (range > 1)
        {
            int2 tileMin = tilePos - int2(range - 1);
            int2 tileMax = tilePos + int2(range - 1);

            for (int x = tileMin.x; x <= tileMax.x; x++)
            {
                for (int y = tileMin.y; y <= tileMax.y; y++)
                {
                    buffer.Add(new ExplosionRequestData() { Instigator = instigator, Damage = damage, TilePos = int2(x, y) });
                }
            }
        }
        else
        {
            buffer.Add(new ExplosionRequestData() { Instigator = instigator, Damage = damage, TilePos = tilePos });
        }
    }

    public void RequestExplosion(ExplosionRequestData damageRequestData)
    {
        GetExplosionRequestBuffer().Add(damageRequestData);
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

        foreach (ExplosionRequestData request in explosionRequests)
        {
            Entity tile = CommonReads.GetTileEntity(Accessor, request.TilePos);

            if (tile != Entity.Null)
            {
                _entitiesToDamage.Clear();
                CommonReads.FindTileActorsWithComponent<Health>(Accessor, tile, _entitiesToDamage);

                foreach (Entity entity in _entitiesToDamage)
                {
                    CommonWrites.RequestDamageOnTarget(Accessor, request.Instigator, entity, request.Damage);
                }

                _newExplodeLocations.Add(request.TilePos);
            }

        }

        explosionRequests.Clear();

        foreach (int2 pos in _newExplodeLocations)
        {
            EntityManager.CreateEventEntity(new ExplosionEventData() { ExplodedTile = pos });
        }
        _newExplodeLocations.Clear();
    }
}

internal static partial class CommonWrites
{
    public static void RequestExplosionOnTiles(ISimWorldReadWriteAccessor accessor, Entity instigator, int2 tilePos, int range, int damage)
    {
        accessor.GetExistingSystem<ApplyExplosionSystem>().RequestExplosion(instigator, tilePos, range, damage);
    }
}