using CCC.Fix2D;
using CCC.Fix2D.Authoring;
using CCC.InspectorDisplay;
using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using Collider = CCC.Fix2D.Collider;

[DisallowMultipleComponent]
public class MovementAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public enum EMoveDirection { Right, Left }
    public EMoveDirection MoveDirection = EMoveDirection.Left;
    public fix MoveSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed
        {
            Value = MoveDirection == EMoveDirection.Left ? -MoveSpeed : MoveSpeed
        });
        dstManager.AddComponent<CanMove>(entity);
    }
}