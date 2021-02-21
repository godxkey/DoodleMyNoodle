using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[RequiresEntityConversion]
[DisallowMultipleComponent]
public class NavAgentAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix MoveSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<NavAgentTag>(entity);
        dstManager.AddComponent<NavAgentFootingState>(entity);
        dstManager.AddComponentData(entity, new MoveSpeed { Value = MoveSpeed });
    }
}