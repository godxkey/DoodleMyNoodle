using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GameActionSettingAuthAttribute(typeof(GameActionSettingTransformTile))]
public class GameActionSettingTransformTileAuth : GameActionSettingAuthBase
{
    public TileFlags NewTileFlags;
    public fix Radius = (fix)0.5;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingTransformTile() { NewTileFlags = NewTileFlags, Radius = Radius });
    }

    //public Color GetColor()
    //{
    //    return Color.white;
    //}

    //public string GetDescription()
    //{
    //    return $"Description d'item";
    //}
}