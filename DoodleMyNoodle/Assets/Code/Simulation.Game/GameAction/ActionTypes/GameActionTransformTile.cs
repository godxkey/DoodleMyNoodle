using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Mathematics;

public class GameActionTransformTile : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[] { typeof(GameActionSettingTransformTile) };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
                   new GameActionParameterPosition.Description() { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            var settings = accessor.GetComponent<GameActionSettingTransformTile>(context.Item);

            var transformTileRequests = accessor.GetSingletonBuffer<SystemRequestTransformTile>();

            NativeList<int2> tiles = new NativeList<int2>(Allocator.Temp);
            TilePhysics.GetAllTilesWithin(paramPosition.Position, settings.Radius, tiles);

            for (int i = 0; i < tiles.Length; i++)
            {
                transformTileRequests.Add(new SystemRequestTransformTile()
                {
                    NewTileFlags = settings.NewTileFlags,
                    Tile = tiles[i]
                });
            }

            return true;
        }

        return false;
    }
}