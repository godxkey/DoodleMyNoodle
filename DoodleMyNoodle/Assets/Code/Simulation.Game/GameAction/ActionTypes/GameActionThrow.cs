using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionThrow : GameAction<GameActionThrow.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix SpeedMin = 0;
        public fix SpeedMax = 10;
        public fix SpawnExtraDistance = 0;
        public GameObject ProjectilePrefab;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                SpeedMax = SpeedMax,
                SpeedMin = SpeedMin,
                SpawnExtraDistance = SpawnExtraDistance,
                ProjectilePrefab = conversionSystem.GetPrimaryEntity(ProjectilePrefab)
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(ProjectilePrefab);

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public fix SpeedMax;
        public fix SpeedMin;
        public fix SpawnExtraDistance;
        public Entity ProjectilePrefab;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
            new GameActionParameterVector.Description()
            {
                SpeedMin = settings.SpeedMin,
                SpeedMax = settings.SpeedMax
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            // spawn projectile
            Entity projectileInstance = accessor.Instantiate(settings.ProjectilePrefab);

            // set projectile data
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
            fix2 instigatorVel = fix2.zero;
            if (accessor.TryGetComponent(context.InstigatorPawn, out PhysicsVelocity instigatorPhysicsVelocity))
            {
                instigatorVel = instigatorPhysicsVelocity.Linear;
            }

            fix2 velocity = clampLength(paramVector.Vector, settings.SpeedMin, settings.SpeedMax);
            fix inputSpeed = length(velocity);
            fix2 dir = inputSpeed < (fix)0.01 ? fix2(0, 1) : velocity / inputSpeed;

            fix2 spawnPos = instigatorPos + GetSpawnPosOffset(accessor, projectileInstance, context.InstigatorPawn, dir, settings.SpawnExtraDistance);

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
        Settings settings = accessor.GetComponent<Settings>(context.Item);

        return GetSpawnPosOffset(accessor, settings.ProjectilePrefab, context.InstigatorPawn, direction, settings.SpawnExtraDistance);
    }

    // used by presentation
    public fix GetProjectileRadius(ISimWorldReadAccessor accessor, UseContext context)
    {
        Settings settings = accessor.GetComponent<Settings>(context.Item);

        return CommonReads.GetActorRadius(accessor, settings.ProjectilePrefab);
    }
    #endregion
}
