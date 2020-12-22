using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionRangeDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionRangeData>
{
    public int Range;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionRangeData() { Value = Range });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionRangeData inputData)
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