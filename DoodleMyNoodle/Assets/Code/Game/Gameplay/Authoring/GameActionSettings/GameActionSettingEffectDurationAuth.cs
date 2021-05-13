using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingEffectDuration))]
[MovedFrom(false, sourceClassName: "GameActionEffectDurationDataAuth")]
public class GameActionSettingEffectDurationAuth : GameActionSettingAuthBase, IItemSettingDescription
{
    public int Duration;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingEffectDuration() { Value = Duration });
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