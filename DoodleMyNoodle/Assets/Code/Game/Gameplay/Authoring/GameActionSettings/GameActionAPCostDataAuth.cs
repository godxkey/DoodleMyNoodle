using System;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[Serializable]
[GameActionSettingAuth(typeof(GameActionAPCostData))]
public class GameActionAPCostDataAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int ActionPointCost = 1;

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
        return $"Actions : {ActionPointCost}";
    }
}