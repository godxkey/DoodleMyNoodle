using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemPassiveEffectHealthIncreaseMultiplierAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemPassiveEffectHealthIncreaseMultiplierData>
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

    public string GetDescription(ItemPassiveEffectHealthIncreaseMultiplierData inputData)
    {
        if (inputData.Value == Multiplier)
        {
            return $"HP Increase Multiplier : {inputData.Value}";
        }
        else
        {
            return $"HP Increase Multiplier : {inputData.Value} ({Multiplier})";
        }
    }
}