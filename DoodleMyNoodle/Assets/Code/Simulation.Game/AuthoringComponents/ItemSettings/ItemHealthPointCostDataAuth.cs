using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemHealthPointCostDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemHealthPointCostData>
{
    public int HealthCost;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemHealthPointCostData() { Value = HealthCost });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemHealthPointCostData inputData)
    {
        if (inputData.Value == HealthCost)
        {
            return $"HP Cost : {inputData.Value}";
        }
        else
        {
            return $"HP Cost : {inputData.Value} ({HealthCost})";
        }
    }
}