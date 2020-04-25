using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class TeamAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum DesignerFriendlyTeam
    {
        Player = 0,
        Baddies = 1
    }

    public DesignerFriendlyTeam StartingTeam = DesignerFriendlyTeam.Baddies;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Team() { Value = (int)StartingTeam });
    }
}
