using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using Unity.Mathematics;

public class GameActionTransformTile : GameAction<GameActionTransformTile.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public TileFlags NewTileFlags;
        public fix Radius = (fix)0.5;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings() { NewTileFlags = NewTileFlags, Radius = Radius });
        }
    }

    public struct Settings : IComponentData
    {
        public TileFlags NewTileFlags;
        public fix Radius;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
                   new GameActionParameterPosition.Description() { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
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