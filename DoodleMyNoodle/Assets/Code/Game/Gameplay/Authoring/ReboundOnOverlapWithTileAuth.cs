using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ReboundOnOverlapWithTileAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] 
    private int ReboundMax = 1;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ReboundOnOverlapWithTileSetting()
        {
            ReboundMax = ReboundMax
        });

        dstManager.AddComponentData(entity, new ReboundOnOverlapWithTileState() { ReboundCount = 0 });
    }
}