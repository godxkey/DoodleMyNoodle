using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingRange))]
[MovedFrom(false, sourceClassName: "GameActionRangeDataAuth")]
public class GameActionSettingRangeAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int Range = 1;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingRange() { Value = Range });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"Porté : {Range}";
    }
}