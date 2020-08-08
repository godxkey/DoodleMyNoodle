using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemStackableDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemStackableData>
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemStackableData() { Value = 1 });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemStackableData inputData)
    {
        return $"Current Stack : {inputData.Value}";
    }
}