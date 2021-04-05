using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GameActionSettingAuth(typeof(GameActionRangeData))]
public class GameActionRangeDataAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int Range = 1;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionRangeData() { Value = Range });
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