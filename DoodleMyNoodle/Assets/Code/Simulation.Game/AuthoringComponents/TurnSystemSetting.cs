using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TurnSystemSetting : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum Team
    {
        Players = 0,
        AI = 1
    }

    public fix AITurnDuration = 2;
    public fix PlayerTurnDuration = 20;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TurnDurationSingletonComponent { DurationAI = AITurnDuration, DurationPlayer = PlayerTurnDuration });
        dstManager.AddComponentData(entity, new TurnCurrentTeamSingletonComponent { Value = -1 });
        dstManager.AddComponentData(entity, new TurnTeamCountSingletonComponent { Value = Enum.GetValues(typeof(Team)).Length });
        dstManager.AddComponentData(entity, new TurnTimerSingletonComponent { Value = PlayerTurnDuration });
        dstManager.AddComponentData(entity, new TurnCountSingletonComponent { Value = 0 });
    }
}
