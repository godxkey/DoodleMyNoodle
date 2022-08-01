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

    public static fix2 GetPlayerGroupHeaderPosition(ISimWorldReadAccessor accessor)
    {
        return accessor.GetComponent<FixTranslation>(accessor.GetSingletonEntity<PlayerGroupDataTag>());
    }

    public static Entity FindClosestEnemyInRange(ISimWorldReadAccessor accessor, fix posX, FixRange rangeX, bool prioritizeFlyingEnemies)
    {
        Entity recordHolder = Entity.Null;
        fix recordDistance = fix.MaxValue;

        if (prioritizeFlyingEnemies)
        {
            accessor.Entities
                .WithAll<MobEnemyTag>()
                .WithNone<DeadTag>()
                .ForEach((Entity entity, ref FixTranslation position, ref PhysicsGravity gravity) =>
                {
                    if (gravity.ScaleFix == 0 && rangeX.Contains(position.Value.x))
                    {
                        fix distance = fix.Abs(position.Value.x - posX);
                        if (distance < recordDistance)
                        {
                            recordHolder = entity;
                            recordDistance = distance;
                        }
                    }
                });

            if (recordHolder != Entity.Null)
                return recordHolder;
        }

        accessor.Entities
            .WithAll<MobEnemyTag>()
            .WithNone<DeadTag>()
            .ForEach((Entity entity, ref FixTranslation position) =>
            {
                if (rangeX.Contains(position.Value.x))
                {
                    fix distance = fix.Abs(position.Value.x - posX);
                    if (distance < recordDistance)
                    {
                        recordHolder = entity;
                        recordDistance = distance;
                    }
                }
            });

        return recordHolder;
    }
}