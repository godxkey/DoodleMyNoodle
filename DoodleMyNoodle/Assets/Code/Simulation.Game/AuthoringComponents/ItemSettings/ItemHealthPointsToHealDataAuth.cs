using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemHealthPointsToHealDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemHealthPointsToHealData>
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

    public string GetDescription(ItemHealthPointsToHealData inputData)
    {
        if (inputData.Value == HealthToHeal)
        {
            return $"Healing : {inputData.Value}";
        }
        else
        {
            return $"Healing : {inputData.Value} ({HealthToHeal})";
        }
    }
}