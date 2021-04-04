using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemPassiveEffectHealthIncreaseAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription
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

    public string GetDescription()
    {
        return $"HP Increase : {Amount}";
    }
}