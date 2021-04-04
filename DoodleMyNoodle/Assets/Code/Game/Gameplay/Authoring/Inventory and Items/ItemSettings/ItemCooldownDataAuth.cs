using CCC.InspectorDisplay;
using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public class ItemCooldownDataAuth : IItemSettingDescription
{
    public bool UseTime = true;
    [ShowIf("UseTime")]
    public int TimeCooldown = 1;
    [HideIf("UseTime")]
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

    public string GetDescription()
    {
        if (UseTime)
        {
            return $"Cooldown (time) : {TimeCooldown}";
        }
        else
        {
            return $"Cooldown (turn) : {TurnCooldown}";
        }
    }
}