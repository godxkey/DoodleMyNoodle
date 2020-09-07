using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemActionPointGainDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemActionPointGainData>
{
    public int ActionPointGain;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemActionPointGainData() { Value = ActionPointGain });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemActionPointGainData inputData)
    {
        if (inputData.Value == ActionPointGain)
        {
            return $"AP Gain : {inputData.Value}";
        }
        else
        {
            return $"AP Gain : {inputData.Value} ({ActionPointGain})";
        }
    }
}