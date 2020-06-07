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

    public Team StartingTeam = Team.Players;
    public fix AITurnDuration = 2;
    public fix PlayerTurnDuration = 20;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TurnDuration { DurationAI = AITurnDuration, DurationPlayer = PlayerTurnDuration });

        dstManager.AddComponentData(entity, new TurnCurrentTeam { Value = (int)StartingTeam });

        dstManager.AddComponentData(entity, new TurnTeamCount { Value = (int)Enum.GetValues(typeof(Team)).Length });

        dstManager.AddComponentData(entity, new TurnTimer { Value = PlayerTurnDuration });
    }
}
