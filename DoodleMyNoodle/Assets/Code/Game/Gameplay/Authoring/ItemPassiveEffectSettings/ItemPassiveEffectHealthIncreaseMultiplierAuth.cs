using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemPassiveEffectHealthIncreaseMultiplierAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemPassiveEffectHealthIncreaseData>
{
    public int Multiplier;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemPassiveEffectHealthIncreaseMultiplierData() { Value = Multiplier });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemPassiveEffectHealthIncreaseData inputData)
    {
        if (inputData.Value == Multiplier)
        {
            return $"Health Multiplier : {inputData.Value}";
        }
        else
        {
            return $"Health Multiplier : {inputData.Value} ({Multiplier})";
        }
    }
}