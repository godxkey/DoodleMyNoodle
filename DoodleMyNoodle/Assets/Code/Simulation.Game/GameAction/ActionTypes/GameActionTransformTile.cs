using static fixMath;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
using CCC.Fix2D;

public class GameActionTransformTile : GameAction<GameActionTransformTile.Settings>
{
    public struct Settings : IComponentData
    {
        public bool SetNewSimAssetId;
        public SimAssetId NewSimAssetId;
        public TileFlags NewTileFlags;
        public fix Radius;
        public fix Range;
        public bool CanChangeBedrockTile;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
                   new GameActionParameterPosition.Description() { MaxRangeFromInstigator = settings.Range });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            var transformTileRequests = accessor.GetSingletonBuffer<SystemRequestTransformTile>();

            // clamp input position into range
            paramPosition.Position = Helpers.ClampPositionInsideRange(paramPosition.Position, accessor.GetComponent<FixTranslation>(context.InstigatorPawn), settings.Range);

            NativeList<int2> tiles = new NativeList<int2>(Allocator.Temp);
            TilePhysics.GetAllTilesWithin(paramPosition.Position, settings.Radius, tiles);

            for (int i = 0; i < tiles.Length; i++)
            {
                if (!settings.CanChangeBedrockTile && accessor.GetComponent<TileFlagComponent>(CommonReads.GetTileEntity(accessor, tiles[i])).Value == TileFlagComponent.Bedrock)
                {
                    continue;
                }

                transformTileRequests.Add(new SystemRequestTransformTile()
                {
                    ForcedNewSimAssetId = settings.SetNewSimAssetId ? (SimAssetId?)settings.NewSimAssetId : (SimAssetId?)null,
                    NewTileFlags = settings.NewTileFlags,
                    Tile = tiles[i]
                });
            }

            return true;
        }

        return false;
    }
}