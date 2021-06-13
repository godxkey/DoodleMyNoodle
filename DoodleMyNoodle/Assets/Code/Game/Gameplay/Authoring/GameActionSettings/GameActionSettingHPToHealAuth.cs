using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingHPToHeal))]
[MovedFrom(false, sourceClassName: "GameActionHPToHealDataAuth")]
public class GameActionSettingHPToHealAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int MaxHealthToGive;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingHPToHeal() { Value = MaxHealthToGive });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"Vie à donner : {MaxHealthToGive}";
    }
}