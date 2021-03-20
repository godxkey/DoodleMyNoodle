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
        return new UseContract(new GameActionParameterVector.Description());
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramTrajectory))
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
            fix2 instigatorPos = Helpers.GetTileCenter(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));

            fix2 instigatorVel = fix2.zero;
            if (accessor.TryGetComponentData(context.InstigatorPawn, out PhysicsVelocity vel))
            {
                instigatorVel = vel.Linear;
            }

            fix2 dir = paramTrajectory.Vector;

            accessor.SetOrAddComponentData(projectileInstance, new PhysicsVelocity(dir + instigatorVel));
            accessor.SetOrAddComponentData(projectileInstance, new FixTranslation(instigatorPos + normalize(dir)));

            // add 'DamageOnContact' if ItemDamageData found
            if (accessor.HasComponent<GameActionDamageData>(context.Entity))
            {
                accessor.SetOrAddComponentData(projectileInstance, new DamageOnContact() { Value = accessor.GetComponentData<GameActionDamageData>(context.Entity).Value, DestroySelf = true });
            }

            return true;
        }

        return false;
    }
}
