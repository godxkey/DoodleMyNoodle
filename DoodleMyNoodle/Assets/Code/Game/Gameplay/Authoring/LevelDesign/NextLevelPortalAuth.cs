using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class NextLevelPortalAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private Transform NextLevelStartPosition;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Vector3 position = NextLevelStartPosition.position;
        dstManager.AddComponentData(entity, new NextLevelPortalData() { NextLevelPos = new fix2((fix)position.x, (fix)position.y) });
    }
}