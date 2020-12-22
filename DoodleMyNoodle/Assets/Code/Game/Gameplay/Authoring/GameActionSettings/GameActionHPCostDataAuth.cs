using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionHPCostDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionHPCostData>
{
    public int HealthCost;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionHPCostData() { Value = HealthCost });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionHPCostData inputData)
    {
        if (inputData.Value == HealthCost)
        {
            return $"HP Cost : {inputData.Value}";
        }
        else
        {
            return $"HP Cost : {inputData.Value} ({HealthCost})";
        }
    }
}