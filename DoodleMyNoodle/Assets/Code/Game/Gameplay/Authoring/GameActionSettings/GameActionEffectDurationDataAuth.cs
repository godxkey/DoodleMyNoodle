using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionEffectDurationDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionEffectDurationData>
{
    public int Duration;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionEffectDurationData() { Value = Duration });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionEffectDurationData inputData)
    {
        if (inputData.Value == Duration)
        {
            return $"Duration : {inputData.Value}";
        }
        else
        {
            return $"Duration : {inputData.Value} ({Duration})";
        }
    }
}