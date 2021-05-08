using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingHPCost))]
[MovedFrom(false, sourceClassName: "GameActionHPCostDataAuth")]
public class GameActionSettingHPCostAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int HealthCost;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingHPCost() { Value = HealthCost });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"Vie à perdre : {HealthCost}";
    }
}