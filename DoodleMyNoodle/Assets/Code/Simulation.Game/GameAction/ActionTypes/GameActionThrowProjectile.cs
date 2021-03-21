using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;

public class GameActionThrowProjectile : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterVector.Description()
            {
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            // get settings
            if (!accessor.TryGetComponentData(context.Entity, out GameActionThrowProjectileSettings settings))
            {
                Debug.LogWarning($"Item {context.Entity} has no {nameof(GameActionThrowProjectileSettings)} component");
                return false;
            }

            // spawn projectile
            Entity projectileInstance = accessor.Instantiate(settings.ProjectilePrefab);

            // set projectile data
            fix2 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn);
            fix2 instigatorVel = fix2.zero;
            if (accessor.TryGetComponentData(context.InstigatorPawn, out PhysicsVelocity vel))
            {
                instigatorVel = vel.Linear;
            }
            fix2 dir = normalize(paramVector.Vector);
            fix projectileRadius = GetActorRadius(accessor, projectileInstance);
            fix instigatorRadius = GetActorRadius(accessor, context.InstigatorPawn);
            fix2 spawnPos = instigatorPos + dir * (instigatorRadius + projectileRadius + (fix)0.05f);

            accessor.SetOrAddComponentData(projectileInstance, new PhysicsVelocity(paramVector.Vector + instigatorVel));
            accessor.SetOrAddComponentData(projectileInstance, new FixTranslation(spawnPos));

            // add 'DamageOnContact' if ItemDamageData found
            if (accessor.HasComponent<GameActionDamageData>(context.Entity))
            {
                accessor.SetOrAddComponentData(projectileInstance, new DamageOnContact() { Value = accessor.GetComponentData<GameActionDamageData>(context.Entity).Value, DestroySelf = true });
            }

            return true;
        }

        return false;
    }

    private static fix GetActorRadius(ISimWorldReadWriteAccessor accessor, Entity projectileInstance)
    {
        if (accessor.TryGetComponentData(projectileInstance, out PhysicsColliderBlob colliderBlob) && colliderBlob.Collider.IsCreated)
        {
            return (fix)colliderBlob.Collider.Value.Radius;
        }
        else
        {
            return 1;
        }
    }
}
