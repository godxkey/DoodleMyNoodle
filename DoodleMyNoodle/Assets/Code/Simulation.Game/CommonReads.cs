using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using CCC.Fix2D;

public static partial class CommonReads
{
    public static bool IsInRange(ISimWorldReadAccessor accessor, Entity entityA, Entity entityB, fix rangeMax)
    {
        return fixMath.distance(
            accessor.GetComponent<FixTranslation>(entityB), 
            accessor.GetComponent<FixTranslation>(entityA)) < rangeMax;
    }

    public static fix GetActorRadius(ISimWorldReadAccessor accessor, Entity projectileInstance)
    {
        if (accessor.TryGetComponent(projectileInstance, out PhysicsColliderBlob colliderBlob) && colliderBlob.Collider.IsCreated)
        {
            return (fix)colliderBlob.Collider.Value.Radius;
        }
        else
        {
            return (fix)0.5;
        }
    }
}