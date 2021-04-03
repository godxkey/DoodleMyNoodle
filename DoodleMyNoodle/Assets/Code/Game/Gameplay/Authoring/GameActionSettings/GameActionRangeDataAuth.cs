using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GameActionSettingAuth(typeof(GameActionRangeData))]
public class GameActionRangeDataAuth : GameActionSettingAuthBase, IItemSettingDescription<GameActionRangeData>
{
    public int Range;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionRangeData() { Value = Range });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionRangeData inputData)
    {
        if (inputData.Value == Range)
        {
            return $"Range : {inputData.Value}";
        }
        else
        {
            return $"Range : {inputData.Value} ({Range})";
        }
    }
}