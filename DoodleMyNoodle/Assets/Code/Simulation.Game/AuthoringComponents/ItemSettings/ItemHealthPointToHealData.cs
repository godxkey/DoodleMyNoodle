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
        return $"Healing : {inputData.Value} ({HealthToHeal})";
    }
}