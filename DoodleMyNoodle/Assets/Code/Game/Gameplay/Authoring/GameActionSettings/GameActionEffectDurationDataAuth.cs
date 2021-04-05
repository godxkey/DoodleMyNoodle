using System;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[Serializable]
[GameActionSettingAuth(typeof(GameActionEffectDurationData))]
public class GameActionEffectDurationDataAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int Duration;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionEffectDurationData() { Value = Duration });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"Durée : {Duration}";
    }
}