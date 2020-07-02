using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemCooldownDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescriptionText, IItemSettingDescriptionColor
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

    public string GetDescription(object[] inputValues)
    {
        return $"Cooldown : {inputValues[0]} ({Cooldown})";
    }
}