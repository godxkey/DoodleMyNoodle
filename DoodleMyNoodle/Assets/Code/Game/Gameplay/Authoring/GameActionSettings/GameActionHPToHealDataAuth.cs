using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GameActionSettingAuth(typeof(GameActionHPToHealData))]
public class GameActionHPToHealDataAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int HealthToGive;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionHPToHealData() { Value = HealthToGive });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"Vie à donner : {HealthToGive}";
    }
}