using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using System;

public class DestroyTileOnOverlapSystem : SimSystemBase
{
    protected override void OnCreate()
    {
        base.OnCreate();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnUpdate()
    {
        var transformTileRequests = GetSingletonBuffer<SystemRequestTransformTile>();
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);

        Entities.ForEach((Entity entity, in DestroyTileOnOverlapTag destroyOnOverlapWithTile, in FixTranslation position) =>
        {
            int2 tile = Helpers.GetTile(position);
            TileFlagComponent tileFlags = tileWorld.GetFlags(tile);

            if (tileFlags.IsDestructible && !tileFlags.IsEmpty)
            {
                transformTileRequests.Add(new SystemRequestTransformTile()
                {
                    NewTileFlags = TileFlagComponent.Empty,
                    Tile = tile
                });
            }
        }).Run();
    }
}