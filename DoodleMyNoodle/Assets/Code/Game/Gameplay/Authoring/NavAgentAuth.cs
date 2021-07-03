using CCC.Fix2D;
using CCC.Fix2D.Authoring;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
public class NavAgentAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix MoveSpeed;
    public fix AirControl;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<NavAgentTag>(entity);
        dstManager.AddComponent<NavAgentFootingState>(entity);
        dstManager.AddComponentData(entity, new MoveSpeed { Value = MoveSpeed });
        dstManager.AddComponentData(entity, new MoveInput { Value = fix2.zero });
        dstManager.AddComponentData(entity, new AirControl { Value = AirControl });

        float normalFriction = 0.4f;
        var bodyAuth = GetComponent<PhysicsBodyAuth>();
        if (bodyAuth?.Material != null)
        {
            normalFriction = bodyAuth.Material.Friction;
        }
        dstManager.AddComponentData(entity, new NonAirControlFriction { Value = (fix)normalFriction });
    }
}