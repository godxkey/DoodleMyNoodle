using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TurnAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum Team
    {
        Players = 0,
        AI = 1,
        [HideInInspector]
        Count = 2
    }

    public Team StartingTeam = Team.Players;
    public int TurnDuration = 10;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TurnDuration { Value = TurnDuration });

        dstManager.AddComponentData(entity, new TurnCurrentTeam { Value = (int)StartingTeam });
        dstManager.AddComponentData(entity, new MaximumInt<TurnCurrentTeam> { Value = (int)Team.Count });

        dstManager.AddComponentData(entity, new TurnTimer { Value = TurnDuration });
    }
}
