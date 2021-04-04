using System;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[Serializable]
[GameActionSettingAuth(typeof(GameActionAPCostData))]
public class GameActionAPCostDataAuth : GameActionSettingAuthBase, IConvertGameObjectToEntity, IItemSettingDescription
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

    public string GetDescription()
    {
        return $"AP Cost : {ActionPointCost}";
    }
}