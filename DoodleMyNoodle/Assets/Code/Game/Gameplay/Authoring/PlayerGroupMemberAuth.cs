using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerGroupMemberAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [Header("TODO: remove this later")]
    public int Index;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<PlayerGroupMemberTag>(entity);
        dstManager.AddComponentData<PlayerGroupMemberIndex>(entity, Index);
    }
}