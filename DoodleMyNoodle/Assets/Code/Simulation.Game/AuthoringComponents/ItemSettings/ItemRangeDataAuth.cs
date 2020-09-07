using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemRangeDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemRangeData>
{
    public int Range;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemRangeData() { Value = Range });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemRangeData inputData)
    {
        if (inputData.Value == Range)
        {
            return $"Range : {inputData.Value}";
        }
        else
        {
            return $"Range : {inputData.Value} ({Range})";
        }
    }
}