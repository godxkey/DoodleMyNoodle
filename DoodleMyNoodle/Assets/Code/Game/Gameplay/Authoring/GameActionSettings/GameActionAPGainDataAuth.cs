using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionAPGainDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionAPGainData>
{
    public int ActionPointGain;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionAPGainData() { Value = ActionPointGain });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionAPGainData inputData)
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