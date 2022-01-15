using CCC.Fix2D;
using CCC.Fix2D.Authoring;
using CCC.InspectorDisplay;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using Collider = CCC.Fix2D.Collider;

[DisallowMultipleComponent]
public class NavAgentAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix MoveSpeed;
    public fix AirControl;

    public fix MoveEnergyMaxValue = 10;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<NavAgentTag>(entity);
        dstManager.AddComponent<NavAgentFootingState>(entity);
        dstManager.AddComponentData(entity, new MoveSpeed { Value = MoveSpeed });
        dstManager.AddComponentData(entity, new MoveInput { Value = fix2.zero });
        dstManager.AddComponentData(entity, new AirControl { Value = AirControl });


        float normalFriction = 0.4f;
        var bodyAuth = GetComponent<PhysicsBodyAuth>();
        if (bodyAuth != null)
        {
            var normalCollider = dstManager.GetComponentData<PhysicsColliderBlob>(entity).Collider;

            if (!dstManager.TryGetComponentData<ActorColliderRefs>(entity, out var actorColliderRefs))
            {
                actorColliderRefs = new ActorColliderRefs(normalCollider);
            }

            // duplicate collider, but with alternate fricton
            var airControllerCollider = BlobAssetReference<Collider>.Create(normalCollider.Value);
            var mat = airControllerCollider.Value.Material;
            mat.Friction = 0;
            airControllerCollider.Value.Material = mat;

            actorColliderRefs.AirControlCollider = airControllerCollider;

            dstManager.SetOrAddComponentData(entity, actorColliderRefs);
        }
        dstManager.AddComponentData(entity, new NonAirControlFriction { Value = (fix)normalFriction });
    }
}