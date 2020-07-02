using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemCooldownDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemCooldownData>
{
    public int Cooldown;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemCooldownData() { Value = Cooldown });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemCooldownData inputData)
    {
        return $"Cooldown : {inputData.Value} ({Cooldown})";
    }
}