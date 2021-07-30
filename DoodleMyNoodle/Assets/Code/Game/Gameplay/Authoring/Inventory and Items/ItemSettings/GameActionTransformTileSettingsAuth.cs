using Unity.Entities;
using System;

[Serializable]
[GameActionSettingAuth(typeof(GameActionTransformTile.Settings))]
public class GameActionTransformTileSettingsAuth : GameActionSettingAuthBase
{
    public TileAuth NewTile;
    public fix Radius = (fix)0.5;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        SimAsset newTileSimAsset = NewTile?.GetComponent<SimAsset>();

        dstManager.AddComponentData(entity, new GameActionTransformTile.Settings()
        {
            NewTileFlags = NewTile != null ? NewTile.GetTileFlags() : TileFlagComponent.Empty,
            SetNewSimAssetId = newTileSimAsset != null,
            NewSimAssetId = newTileSimAsset != null ? newTileSimAsset.GetSimAssetId() : default,
            Radius = Radius
        });
    }
}
