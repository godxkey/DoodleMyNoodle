using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class TeamAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public DesignerFriendlyTeam Team;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Team() { Value = (short)Team });
    }
}