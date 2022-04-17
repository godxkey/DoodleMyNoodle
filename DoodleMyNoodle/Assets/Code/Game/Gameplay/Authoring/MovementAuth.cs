using CCC.Fix2D;
using CCC.Fix2D.Authoring;
using CCC.InspectorDisplay;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class MovementAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum EMoveDirection { Right, Left }
    public EMoveDirection MoveDirection = EMoveDirection.Left;
    public float MoveSpeed = 1;
    public bool StopAtCertainDistance = true;

    [ShowIf(nameof(StopAtCertainDistance))]
    public float DistanceFromFrontTarget = 1;

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
        dstManager.AddComponent<DistanceFromTarget>(entity);
        dstManager.AddComponentData<StopMoveFromTargetDistance>(entity, (fix)(StopAtCertainDistance ? DistanceFromFrontTarget : -100000));
    }

    private void OnDrawGizmosSelected()
    {
        if (StopAtCertainDistance)
        {
            float charRadius = (float)SimulationGameConstants.CharacterRadius;
            Gizmos.color = Color.red;
            float horizontalOffset = DistanceFromFrontTarget + charRadius;
            if (MoveDirection == EMoveDirection.Left)
                horizontalOffset *= -1;
            Gizmos.DrawWireSphere(transform.position + new Vector3(horizontalOffset, 0), charRadius);
        }
    }
}