using System;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class DoodleAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DoodleId { Guid = Guid.Empty });
    }
}

public struct DoodleId : IComponentData
{
    // fbessette: for memory, we might want to store guids elsewhere in the future (they are 128bit each)
    public Guid Guid;
}