using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemCooldownDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemTimeCooldownData>
{
    public bool UseTime = true;
    public int TimeCooldown = 1;
    public int TurnCooldown = 1;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (UseTime)
        {
            dstManager.AddComponentData(entity, new ItemTimeCooldownData() { Value = TimeCooldown });
        }
        else
        {
            dstManager.AddComponentData(entity, new ItemTurnCooldownData() { Value = TurnCooldown });
        }
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemTimeCooldownData inputData)
    {
        if (UseTime)
        {
            return $"Cooldown (time) : {inputData.Value} ({TimeCooldown})";
        }
        else
        {
            return $"Cooldown (turn) : {inputData.Value} ({TurnCooldown})";
        }
    }
}