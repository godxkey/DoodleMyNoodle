using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyTileOnOverlapAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<DestroyTileOnOverlapTag>(entity);
    }
}