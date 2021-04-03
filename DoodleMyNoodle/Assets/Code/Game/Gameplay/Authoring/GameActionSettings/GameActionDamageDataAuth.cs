using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
[GameActionSettingAuth(typeof(GameActionDamageData))]
public class GameActionDamageDataAuth : GameActionSettingAuthBase, IItemSettingDescription<GameActionDamageData>
{
    public int Damage;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionDamageData() { Value = Damage });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionDamageData inputData)
    {
        if (inputData.Value == Damage)
        {
            return $"Damage : {inputData.Value}";
        }
        else
        {
            return $"Damage : {inputData.Value} ({Damage})";
        }
    }
}