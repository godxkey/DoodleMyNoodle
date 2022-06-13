using CCC.Fix2D;
using CCC.Fix2D.Authoring;
using CCC.InspectorDisplay;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using UnityEngine.Serialization;
using Collider = CCC.Fix2D.Collider;

[DisallowMultipleComponent]
public class MovementAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum EMoveDirection { Right, Left }
    public EMoveDirection MoveDirection = EMoveDirection.Left;
    public float MoveSpeed = 1;
    public bool StopAtCertainDistance = true;

    [ShowIf(nameof(StopAtCertainDistance))]
    public float DistanceFromFrontTargetMin = 0;
    [ShowIf(nameof(StopAtCertainDistance))]
    [FormerlySerializedAs("DistanceFromFrontTarget")]
    public float DistanceFromFrontTargetMax = 1;
    public bool KeepWalkingAfterPeriodicAction = false;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed
        {
            Value = (fix)(MoveDirection == EMoveDirection.Left ? -MoveSpeed : MoveSpeed)
        });

        dstManager.AddComponentData(entity, new BaseMoveSpeed
        {
            Value = (fix)(MoveDirection == EMoveDirection.Left ? -MoveSpeed : MoveSpeed)
        });

        if (TryGetComponent(out PhysicsBodyAuth physicsBodyAuth))
        {
            if (physicsBodyAuth.GravityScale != 0)
            {
                dstManager.AddComponent<Grounded>(entity);
                if (!physicsBodyAuth.FireEvents)
                {
                    Log.Error($"Entity {gameObject.name} should have PhysicsBodyAuth events enabled. Otherwise, 'Grounded' will not update correctly in game.");
                }
            }
        }
        dstManager.AddComponent<CanMove>(entity);
        dstManager.AddComponent<OffsetFromTarget>(entity);
        dstManager.AddComponentData<DesiredRangeFromTarget>(entity,
            StopAtCertainDistance ? new FixRange((fix)DistanceFromFrontTargetMin, (fix)DistanceFromFrontTargetMax) : FixRange.Invalid);

        if (KeepWalkingAfterPeriodicAction)
            dstManager.AddComponent<KeepWalkingAfterPeriodicAction>(entity);

        //// add ungrounded collider
        //if (dstManager.TryGetComponent(entity, out PhysicsColliderBlob colliderBlob))
        //{
        //    var normalCollider = colliderBlob.Collider;

        //    if (!dstManager.TryGetComponent<ActorColliderRefs>(entity, out var actorColliderRefs))
        //    {
        //        actorColliderRefs = new ActorColliderRefs(normalCollider);
        //    }

        //    var normalMaterial = normalCollider.Value.Material;

        //    // duplicate collider, and remove friction from normal collider
        //    actorColliderRefs.UngroundedCollider = BlobAssetReference<Collider>.Create(normalCollider.Value);

        //    normalMaterial.Friction = 0;
        //    normalCollider.Value.Material = normalMaterial;

        //    dstManager.AddComponentData(entity, actorColliderRefs);
        //}
    }

    private void OnDrawGizmosSelected()
    {
        if (StopAtCertainDistance)
        {
            float charRadius = (float)SimulationGameConstants.CharacterRadius;
            Gizmos.color = Color.red;
            float dir = MoveDirection == EMoveDirection.Right ? 1 : -1;
            float offsetMax = (DistanceFromFrontTargetMax + charRadius) * dir;
            float offsetMin = DistanceFromFrontTargetMin * dir;
            Vector3 rangeBegin = transform.position + new Vector3(offsetMax, 0);
            Vector3 rangeEnd = transform.position + new Vector3(offsetMin, 0);
            Gizmos.DrawWireSphere(rangeBegin, charRadius);
            Gizmos.DrawLine(rangeBegin + Vector3.up * charRadius, rangeEnd + Vector3.up * charRadius);
            Gizmos.DrawLine(rangeBegin + Vector3.down * charRadius, rangeEnd + Vector3.down * charRadius);
            if (DistanceFromFrontTargetMin > 0)
                Gizmos.DrawWireSphere(rangeEnd, charRadius);
        }
    }
}