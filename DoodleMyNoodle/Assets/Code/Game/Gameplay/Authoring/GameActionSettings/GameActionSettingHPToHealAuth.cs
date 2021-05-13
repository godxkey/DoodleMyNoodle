using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingHPToHeal))]
[MovedFrom(false, sourceClassName: "GameActionHPToHealDataAuth")]
public class GameActionSettingHPToHealAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int HealthToGive;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingHPToHeal() { Value = HealthToGive });
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