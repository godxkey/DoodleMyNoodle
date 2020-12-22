using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionAPCostDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionAPCostData>
{
    public int ActionPointCost;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionAPCostData() { Value = ActionPointCost });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionAPCostData inputData)
    {
        if (inputData.Value == ActionPointCost)
        {
            return $"Stamina Cost : {inputData.Value}";
        }
        else
        {
            return $"Stamina Cost : {inputData.Value} ({ActionPointCost})";
        }
    }
}