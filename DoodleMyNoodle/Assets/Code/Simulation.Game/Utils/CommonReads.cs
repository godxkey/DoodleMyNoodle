using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using CCC.Fix2D;

public static partial class CommonReads
{
    public static bool IsInRange(ISimWorldReadAccessor accessor, Entity entityA, Entity entityB, fix rangeMax)
    {
        return fixMath.distancesq(
            accessor.GetComponent<FixTranslation>(entityB),
            accessor.GetComponent<FixTranslation>(entityA)) < rangeMax * rangeMax;
    }

    public static bool IsInRange(ISimWorldReadAccessor accessor, Entity entityA, fix2 position, fix rangeMax)
    {
        return fixMath.distancesq(
            position,
            accessor.GetComponent<FixTranslation>(entityA)) < rangeMax * rangeMax;
    }

    /// <summary>
    /// If the actor does not have a collider or is destroyed, the returned value will be an arbitrary fixed size.
    /// </summary>
    public static fix GetActorRadius(ISimWorldReadAccessor accessor, Entity entity)
    {
        if (accessor.TryGetComponent(entity, out PhysicsColliderBlob colliderBlob) && colliderBlob.Collider.IsCreated)
        {
            return (fix)colliderBlob.Collider.Value.Radius;
        }
        else
        {
            return (fix)0.5;
        }
    }

    public static InstigatorSet GetInstigatorSetFromLastPhysicalInstigator(ISimWorldReadAccessor accessor, Entity lastPhysicalInstigator)
    {
        InstigatorSet set = new InstigatorSet()
        {
            LastInstigator = lastPhysicalInstigator,
            LastPhysicalInstigator = lastPhysicalInstigator,
            FirstPhysicalInstigator = lastPhysicalInstigator,
        };

        if (accessor.TryGetComponent(lastPhysicalInstigator, out FirstInstigator firstInstigator))
        {
            set.FirstPhysicalInstigator = firstInstigator;
        }

        return set;
    }
}