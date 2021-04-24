using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

public class CheatsDataAuth : MonoBehaviour//, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
    }
}