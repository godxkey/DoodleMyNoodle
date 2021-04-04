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
        GameActionThrowSettings settings = accessor.GetComponentData<GameActionThrowSettings>(context.Item);

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
            GameActionThrowSettings settings = accessor.GetComponentData<GameActionThrowSettings>(context.Item);

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
            fix inputSpeed = length(velocity);
            fix2 dir = inputSpeed < MIN_VELOCITY ? fix2(0, 1) : velocity / inputSpeed;

            // Clamp vector and speed to min/max speed setting
            {
                if (inputSpeed < settings.SpeedMin)
                {
                    velocity = dir * settings.SpeedMin;
                }
                else if (inputSpeed > settings.SpeedMax)
                {
                    velocity = dir * settings.SpeedMax;
                }
            }

            fix2 spawnPos = instigatorPos + GetSpawnPosOffset(accessor, projectileInstance, context.InstigatorPawn, dir);

            accessor.SetOrAddComponentData(projectileInstance, new PhysicsVelocity(velocity + instigatorVel));
            accessor.SetOrAddComponentData(projectileInstance, new FixTranslation(spawnPos));

            return true;
        }

        return false;
    }

    // used by presentation
    public fix2 GetSpawnPosOffset(ISimWorldReadAccessor accessor, UseContext context, fix2 direction)
    {
        GameActionThrowSettings settings = accessor.GetComponentData<GameActionThrowSettings>(context.Item);

        return GetSpawnPosOffset(accessor, settings.ProjectilePrefab, context.InstigatorPawn, direction);
    }

    private fix2 GetSpawnPosOffset(ISimWorldReadAccessor accessor, Entity projectile, Entity instigator, fix2 direction)
    {
        fix projectileRadius = GetActorRadius(accessor, projectile);
        fix instigatorRadius = GetActorRadius(accessor, instigator);

        return direction * (instigatorRadius + projectileRadius + (fix)0.05f);
    }

    private static fix GetActorRadius(ISimWorldReadAccessor accessor, Entity projectileInstance)
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
