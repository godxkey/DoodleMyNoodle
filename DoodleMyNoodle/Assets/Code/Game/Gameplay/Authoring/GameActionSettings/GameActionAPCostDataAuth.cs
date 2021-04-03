using System;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[Serializable]
[GameActionSettingAuth(typeof(GameActionAPCostData))]
public class GameActionAPCostDataAuth : GameActionSettingAuthBase, IConvertGameObjectToEntity, IItemSettingDescription<GameActionAPCostData>
{
    public int ActionPointCost;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionAPCostData() { Value = ActionPointCost });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionAPCostData inputData)
    {
        if (inputData.Value == ActionPointCost)
        {
            return $"Stamina Cost : {inputData.Value}";
        }
        else
        {
            return $"Stamina Cost : {inputData.Value} ({ActionPointCost})";
        }
    }
}