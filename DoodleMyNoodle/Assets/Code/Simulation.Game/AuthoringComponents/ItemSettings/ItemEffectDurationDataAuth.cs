using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemEffectDurationDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemEffectDurationData>
{
    public int Duration;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemEffectDurationData() { Value = Duration });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemEffectDurationData inputData)
    {
        if (inputData.Value == Duration)
        {
            return $"Duration : {inputData.Value}";
        }
        else
        {
            return $"Duration : {inputData.Value} ({Duration})";
        }
    }
}