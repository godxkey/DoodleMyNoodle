using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public struct ExplosionOnTileEventData : IComponentData
{
    public int2 ExplodedTile;
}

public struct ExplosionToApplySingletonTag : IComponentData
{
}

public struct ExplosionToApplyData : IBufferElementData
{
    public Entity Instigator;
    public int2 TilePos;
    public int Damage;
}

public class ApplyExplosionSystem : SimComponentSystem
{
    EntityQuery _eventsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsEntityQuery = EntityManager.CreateEntityQuery(typeof(ExplosionOnTileEventData));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _eventsEntityQuery.Dispose();
    }

    public static DynamicBuffer<ExplosionToApplyData> GetExplosionToApplySingletonBuffer(ISimWorldReadWriteAccessor accessor)
    {
        if (!accessor.HasSingleton<ExplosionToApplySingletonTag>())
        {
            accessor.CreateEntity(typeof(ExplosionToApplySingletonTag), typeof(ExplosionToApplyData));
        }

        return accessor.GetBuffer<ExplosionToApplyData>(accessor.GetSingletonEntity<ExplosionToApplySingletonTag>());
    }

    protected override void OnUpdate()
    {
        // Clear Damage Applied Events
        EntityManager.DestroyEntity(_eventsEntityQuery);

        DynamicBuffer<ExplosionToApplyData> ExplosionToApplyBuffer = GetExplosionToApplySingletonBuffer(Accessor);

        List<int2> ExplodedLocations = new List<int2>();

        foreach (ExplosionToApplyData explosionData in ExplosionToApplyBuffer)
        {
            NativeList<Entity> entityToDamage = new NativeList<Entity>(Allocator.Temp);
            Entity currentTile = CommonReads.GetTileEntity(Accessor, explosionData.TilePos);

            if (currentTile != Entity.Null)
            {
                CommonReads.FindTileActorsWithComponent<Health>(Accessor, currentTile, entityToDamage);

                foreach (Entity entity in entityToDamage)
                {
                    CommonWrites.RequestDamageOnTarget(Accessor, explosionData.Instigator, entity, explosionData.Damage);
                }

                ExplodedLocations.Add(explosionData.TilePos);
            }

        }

        ExplosionToApplyBuffer.Clear();

        foreach (int2 pos in ExplodedLocations)
        {
            EntityManager.CreateEventEntity(new ExplosionOnTileEventData() { ExplodedTile = pos });
        }
    }
}

internal static partial class CommonWrites
{
    public static void RequestExplosionOnTiles(ISimWorldReadWriteAccessor accessor, Entity instigator, int2 tilePos, int range, int damage)
    {
        DynamicBuffer<ExplosionToApplyData> explosionDataBuffer = ApplyExplosionSystem.GetExplosionToApplySingletonBuffer(accessor);

        explosionDataBuffer.Add(new ExplosionToApplyData() { Instigator = instigator, Damage = damage, TilePos = tilePos });

        // explode other tiles in range
        if (range > 0)
        {

        }
    }
}