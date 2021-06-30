using System;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class DoodleAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DoodleId { Guid = Guid.Empty });
    }
}