using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionDamageDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionDamageData>
{
    public int Damage;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionDamageData() { Value = Damage });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionDamageData inputData)
    {
        if (inputData.Value == Damage)
        {
            return $"Damage : {inputData.Value}";
        }
        else
        {
            return $"Damage : {inputData.Value} ({Damage})";
        }
    }
}