using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;
using System;

public class GameActionThrow : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingThrow),
        typeof(GameActionSettingEntityReference),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        GameActionSettingThrow settings = accessor.GetComponent<GameActionSettingThrow>(context.Item);

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
            GameActionSettingThrow settingsThrow = accessor.GetComponent<GameActionSettingThrow>(context.Item);
            GameActionSettingEntityReference settingsEntity = accessor.GetComponent<GameActionSettingEntityReference>(context.Item);

            // spawn projectile
            Entity projectileInstance = accessor.Instantiate(settingsEntity.EntityPrefab);

            // set projectile data
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
            fix2 instigatorVel = fix2.zero;
            if (accessor.TryGetComponent(context.InstigatorPawn, out PhysicsVelocity instigatorPhysicsVelocity))
            {
                instigatorVel = instigatorPhysicsVelocity.Linear;
            }

            fix2 velocity = clampLength(paramVector.Vector, settingsThrow.SpeedMin, settingsThrow.SpeedMax);
            fix inputSpeed = length(velocity);
            fix2 dir = inputSpeed < (fix)0.01 ? fix2(0, 1) : velocity / inputSpeed;

            fix2 spawnPos = instigatorPos + GetSpawnPosOffset(accessor, projectileInstance, context.InstigatorPawn, dir, settingsThrow.SpawnExtraDistance);

            accessor.SetOrAddComponent(projectileInstance, new PhysicsVelocity(velocity + instigatorVel));
            accessor.SetOrAddComponent(projectileInstance, new FixTranslation(spawnPos));

            return true;
        }

        return false;
    }

    private fix2 GetSpawnPosOffset(ISimWorldReadAccessor accessor, Entity projectile, Entity instigator, fix2 direction, fix extraDistance)
    {
        fix projectileRadius = CommonReads.GetActorRadius(accessor, projectile);
        fix instigatorRadius = CommonReads.GetActorRadius(accessor, instigator);

        return direction * (instigatorRadius + projectileRadius + (fix)0.05f + extraDistance);
    }


    #region Used by presentation
    // used by presentation
    public fix2 GetSpawnPosOffset(ISimWorldReadAccessor accessor, UseContext context, fix2 direction)
    {
        GameActionSettingEntityReference settingsPrefab = accessor.GetComponent<GameActionSettingEntityReference>(context.Item);
        GameActionSettingThrow settingsThrow = accessor.GetComponent<GameActionSettingThrow>(context.Item);

        return GetSpawnPosOffset(accessor, settingsPrefab.EntityPrefab, context.InstigatorPawn, direction, settingsThrow.SpawnExtraDistance);
    }

    // used by presentation
    public fix GetProjectileRadius(ISimWorldReadAccessor accessor, UseContext context)
    {
        GameActionSettingEntityReference settingsPrefab = accessor.GetComponent<GameActionSettingEntityReference>(context.Item);

        return CommonReads.GetActorRadius(accessor, settingsPrefab.EntityPrefab);
    }
    #endregion
}
