using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemHealthPointsToHealDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescriptionText, IItemSettingDescriptionColor
{
    public int HealthToHeal;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemHealthPointsToHealData() { Value = HealthToHeal });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(object[] inputValues)
    {
        return $"Healing : {inputValues[0]} ({HealthToHeal})";
    }
}