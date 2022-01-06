using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using System;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class DestroyOnOverlapWithTileSystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnUpdate()
    {
        NativeList<Entity> toDestroy = new NativeList<Entity>(Allocator.Temp);
        var transformTileRequests = GetSingletonBuffer<SystemRequestTransformTile>();
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);

        Entities.ForEach((Entity entity, in DestroyOnOverlapWithTileTag destroyOnOverlapWithTile, in FixTranslation position) =>
        {
            int2 tile = Helpers.GetTile(position);
            TileFlagComponent tileFlags = tileWorld.GetFlags(tile);

            if (tileFlags.IsTerrain && destroyOnOverlapWithTile.DestroySelfOnTerrain)
            {
                toDestroy.Add(entity);
            }

            if (destroyOnOverlapWithTile.DestroyTile)
            {
                transformTileRequests.Add(new SystemRequestTransformTile()
                {
                    NewTileFlags = TileFlagComponent.Empty,
                    Tile = tile
                });
            }
        }).Run();

        EntityManager.DestroyEntity(toDestroy);
        toDestroy.Clear();
    }
}