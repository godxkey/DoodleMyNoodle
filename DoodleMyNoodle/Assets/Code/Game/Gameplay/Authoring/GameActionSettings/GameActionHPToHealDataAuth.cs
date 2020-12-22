using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionHPToHealDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionHPToHealData>
{
    public int HealthToHeal;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionHPToHealData() { Value = HealthToHeal });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionHPToHealData inputData)
    {
        if (inputData.Value == HealthToHeal)
        {
            return $"Healing : {inputData.Value}";
        }
        else
        {
            return $"Healing : {inputData.Value} ({HealthToHeal})";
        }
    }
}