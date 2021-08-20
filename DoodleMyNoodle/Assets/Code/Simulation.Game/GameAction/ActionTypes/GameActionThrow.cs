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
        bool orginateFromCenter = parameters.TryGetParameter(1, out GameActionParameterBool.Data paramOriginateFromCenter, warnIfFailed: false)
            && paramOriginateFromCenter.Value;

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

            fix2 spawnPos;
            fix spawnDistance = GetSpawnDistance(accessor, projectileInstance, context.InstigatorPawn, settings.SpawnExtraDistance);

            // When 'orginateFromCenter' is true, we simulate the projectile spawning at the center of the pawn.
            // We adjust the start position and the start velocity to where the projectile will exit the spawn-distance
            if (orginateFromCenter)
            {
                // Find how much the projectile will be affected by gravity
                fix2 gravity = accessor.GetExistingSystem<PhysicsWorldSystem>().PhysicsWorld.StepSettings.GravityFix;
                if (accessor.HasComponent<PhysicsGravity>(settings.ProjectilePrefab))
                {
                    gravity *= accessor.GetComponent<PhysicsGravity>(settings.ProjectilePrefab).ScaleFix;
                }

                // Calculate travel time to exit 'spawn-distance' zone
                fix travelTime = Trajectory.TravelDurationApprox(velocity, gravity, spawnDistance + (fix)0.05 /* for extra safety*/, precision: (fix)0.005);

                if (travelTime > 10) // travel time too long ? use normal spawnPos
                {
                    spawnPos = instigatorPos + spawnDistance * dir;
                }
                else
                {
                    spawnPos = Trajectory.Position(instigatorPos, velocity, gravity, travelTime);
                    velocity = Trajectory.Velocity(velocity, gravity, travelTime);
                }
            }
            else
            {
                spawnPos = instigatorPos + spawnDistance * dir;
            }

            accessor.SetOrAddComponent(projectileInstance, new PhysicsVelocity(velocity + instigatorVel));
            accessor.SetOrAddComponent(projectileInstance, new FixTranslation(spawnPos));

            return true;
        }

        return false;
    }

    private fix GetSpawnDistance(ISimWorldReadAccessor accessor, Entity projectile, Entity instigator, fix extraDistance)
    {
        fix projectileRadius = CommonReads.GetActorRadius(accessor, projectile);
        fix instigatorRadius = CommonReads.GetActorRadius(accessor, instigator);

        return instigatorRadius + projectileRadius + (fix)0.05f + extraDistance;
    }

    #region Used by presentation
    // used by presentation
    public fix2 GetSpawnPosOffset(ISimWorldReadAccessor accessor, UseContext context, fix2 direction)
    {
        Settings settings = accessor.GetComponent<Settings>(context.Item);

        return direction * GetSpawnDistance(accessor, settings.ProjectilePrefab, context.InstigatorPawn, settings.SpawnExtraDistance);
    }

    // used by presentation
    public fix GetProjectileRadius(ISimWorldReadAccessor accessor, UseContext context)
    {
        Settings settings = accessor.GetComponent<Settings>(context.Item);

        return CommonReads.GetActorRadius(accessor, settings.ProjectilePrefab);
    }
    #endregion
}
