using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PortalAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] private Transform TeleportToPosition;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (TeleportToPosition != null)
        {
            Vector3 position = TeleportToPosition.position;
            dstManager.AddComponentData(entity, new Portal() { NextPos = new fix2((fix)position.x, (fix)position.y) });
        }
    }
}