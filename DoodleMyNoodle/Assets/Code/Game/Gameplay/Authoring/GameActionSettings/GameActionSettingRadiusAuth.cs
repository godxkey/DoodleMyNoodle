using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingRadius))]
[MovedFrom(false, sourceClassName: "GameActionExplosionRangeAuth")]
public class GameActionSettingRadiusAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public fix Radius;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingRadius() { Value = Radius });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"Porté d'explosion : {Radius}";
    }
}