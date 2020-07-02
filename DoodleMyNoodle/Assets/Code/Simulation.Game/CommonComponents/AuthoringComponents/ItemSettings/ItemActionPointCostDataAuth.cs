using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemActionPointCostDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemActionPointCostData>
{
    public int ActionPointCost;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemActionPointCostData() { Value = ActionPointCost });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemActionPointCostData inputData)
    {
        return $"AP Cost : {inputData.Value} ({ActionPointCost})";
    }
}