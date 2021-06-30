using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class ItemPassiveEffectHealthIncreaseMultiplierAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription
{
    public int Multiplier;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemPassiveEffectHealthIncreaseMultiplierData() { Value = Multiplier });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"HP Increase Multiplier : {Multiplier}";
    }
}