using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemEffectDurationDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescriptionText, IItemSettingDescriptionColor
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

    public string GetDescription(object[] inputValues)
    {
        return $"Duration : {inputValues[0]} ({Duration})";
    }
}