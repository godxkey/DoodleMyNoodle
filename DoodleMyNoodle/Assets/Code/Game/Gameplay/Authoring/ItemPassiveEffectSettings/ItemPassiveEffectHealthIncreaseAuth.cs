using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemPassiveEffectHealthIncreaseAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemPassiveEffectHealthIncreaseData>
{
    public int Amount;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemPassiveEffectHealthIncreaseData() { Value = Amount });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemPassiveEffectHealthIncreaseData inputData)
    {
        if (inputData.Value == Amount)
        {
            return $"HP Increase : {inputData.Value}";
        }
        else
        {
            return $"HP Increase : {inputData.Value} ({Amount})";
        }
    }
}