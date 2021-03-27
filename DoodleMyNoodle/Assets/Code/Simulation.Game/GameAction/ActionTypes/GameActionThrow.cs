using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;

public class GameActionThrow : GameAction
{
    static readonly fix MIN_VELOCITY = fix(0.05);

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        GameActionThrowSettings settings = accessor.GetComponentData<GameActionThrowSettings>(context.Entity);

        return new UseContract(
            new GameActionParameterVector.Description()
            {
                SpeedMin = settings.SpeedMin,
                SpeedMax = settings.SpeedMax
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            // get settings
            GameActionThrowSettings settings = accessor.GetComponentData<GameActionThrowSettings>(context.Entity);

            // spawn projectile
            Entity projectileInstance = accessor.Instantiate(settings.ProjectilePrefab);

            // set projectile data
            fix2 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn);
            fix2 instigatorVel = fix2.zero;
            if (accessor.TryGetComponentData(context.InstigatorPawn, out PhysicsVelocity instigatorPhysicsVelocity))
            {
                instigatorVel = instigatorPhysicsVelocity.Linear;
            }

            fix2 velocity = paramVector.Vector;
            fix speed = length(velocity);
            fix2 dir = speed < MIN_VELOCITY ? fix2(0, 1) : velocity / speed;

            // Clamp vector and speed to min/max speed setting
            {
                if (speed < settings.SpeedMin)
                {
                    velocity = dir * settings.SpeedMin;
                    speed = settings.SpeedMin;
                }
                else if (speed > settings.SpeedMax)
                {
                    velocity = dir * settings.SpeedMax;
                    speed = settings.SpeedMax;
                }
            }


            fix projectileRadius = GetActorRadius(accessor, projectileInstance);
            fix instigatorRadius = GetActorRadius(accessor, context.InstigatorPawn);
            fix2 spawnPos = instigatorPos + dir * (instigatorRadius + projectileRadius + (fix)0.05f);

            accessor.SetOrAddComponentData(projectileInstance, new PhysicsVelocity(velocity + instigatorVel));
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
