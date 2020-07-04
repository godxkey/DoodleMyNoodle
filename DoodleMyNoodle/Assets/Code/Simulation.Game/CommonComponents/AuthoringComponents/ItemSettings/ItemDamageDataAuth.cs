using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemDamageDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemDamageData>
{
    public int Damage;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemDamageData() { Value = Damage });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemDamageData inputData)
    {
        return $"Damage : {inputData.Value} ({Damage})";
    }
}