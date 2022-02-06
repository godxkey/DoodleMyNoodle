using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;
using System;
using System.Collections.Generic;
using UnityEngineX.InspectorDisplay;

public class GameActionThrow : Action<GameActionThrow.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix SpeedMin = 0;
        public fix SpeedMax = 10;
        public fix SpawnExtraDistance = 0;
        public GameObject ProjectilePrefab;
        public int Quantity = 1;
        [Suffix("Degrees")]
        public float VolleyAngle = 0;
        [Header("Default Parameter")]
        public Vector2 ShootVector;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                SpeedMax = SpeedMax,
                SpeedMin = SpeedMin,
                SpawnExtraDistance = SpawnExtraDistance,
                ProjectilePrefab = conversionSystem.GetPrimaryEntity(ProjectilePrefab),
                Quantity = Quantity,
                VolleyAngle = radians((fix)VolleyAngle),
                ShootVector = new fix2((fix)ShootVector.x, (fix)ShootVector.y) 
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
        public int Quantity;
        public fix VolleyAngle; // in radian
        public fix2 ShootVector;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Settings settings)
    {
        return new UseContract(
            new GameActionParameterVector.Description()
            {
                SpeedMin = settings.SpeedMin,
                SpeedMax = settings.SpeedMax
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        bool originateFromCenter = parameters.TryGetParameter(1, out GameActionParameterBool.Data paramOriginateFromCenter, warnIfFailed: false)
            && paramOriginateFromCenter.Value;

        fix2 shootVector;
        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            shootVector = paramVector.Vector;
        }
        else
        {
            shootVector = settings.ShootVector;
        }

        if (settings.Quantity <= 1)
        {
            settings.VolleyAngle = 0;
        }

        fix throwSpeed = clamp(length(paramVector.Vector), settings.SpeedMin, settings.SpeedMax);
        fix throwAngle = throwSpeed < (fix)0.01 ? 0 : angle2d(paramVector.Vector);
        fix throwAngleMin = throwAngle - (settings.VolleyAngle / 2);
        fix throwAngleIncrement = settings.Quantity == 1 ? 0 : settings.VolleyAngle / (settings.Quantity - 1);

        uint projectileGroupID = accessor.MakeUniquePersistentId().Value;
        for (int i = 0; i < settings.Quantity; i++)
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

            fix2 spawnPos;
            fix spawnDistance = GetSpawnDistance(accessor, projectileInstance, context.InstigatorPawn, settings.SpawnExtraDistance);

            fix itemThrowAngle = throwAngleMin + (i * throwAngleIncrement);
            fix2 itemThrowDir = fix2.FromAngle(itemThrowAngle);
            fix2 itemThrowVelocity = itemThrowDir * throwSpeed;

            // When 'originateFromCenter' is true, we simulate the projectile spawning at the center of the pawn.
            // We adjust the start position and the start velocity to where the projectile will exit the spawn-distance
            if (originateFromCenter)
            {
                // Find how much the projectile will be affected by gravity
                fix2 gravity = accessor.GetExistingSystem<PhysicsWorldSystem>().PhysicsWorld.StepSettings.GravityFix;
                if (accessor.HasComponent<PhysicsGravity>(settings.ProjectilePrefab))
                {
                    gravity *= accessor.GetComponent<PhysicsGravity>(settings.ProjectilePrefab).ScaleFix;
                }

                // Calculate travel time to exit 'spawn-distance' zone
                fix travelTime = Trajectory.TravelDurationApprox(itemThrowVelocity, gravity, spawnDistance + (fix)0.05 /* for extra safety*/, precision: (fix)0.005);

                if (travelTime > 10) // travel time too long ? use normal spawnPos
                {
                    spawnPos = instigatorPos + spawnDistance * itemThrowDir;
                }
                else
                {
                    spawnPos = Trajectory.Position(instigatorPos, itemThrowVelocity, gravity, travelTime);
                    itemThrowVelocity = Trajectory.Velocity(itemThrowVelocity, gravity, travelTime);
                }
            }
            else
            {
                spawnPos = instigatorPos + spawnDistance * itemThrowDir;
            }

            accessor.SetOrAddComponent(projectileInstance, new PhysicsVelocity(itemThrowVelocity + instigatorVel));
            accessor.SetOrAddComponent(projectileInstance, new FixTranslation(spawnPos));
            accessor.SetOrAddComponent(projectileInstance, new OwnerPawn(context.InstigatorPawn));

            if (settings.Quantity > 1)
            {
                accessor.SetOrAddComponent(projectileInstance, new EffectGroupComponent() { ID = projectileGroupID });
            }
        }

        return true;
    }

    private fix GetSpawnDistance(ISimWorldReadAccessor accessor, Entity projectile, Entity instigator, fix extraDistance)
    {
        fix projectileRadius = CommonReads.GetActorRadius(accessor, projectile);
        fix instigatorRadius = CommonReads.GetActorRadius(accessor, instigator);

        return instigatorRadius + projectileRadius + (fix)0.05f + extraDistance;
    }

    #region Used by presentation
    // used by presentation
    public fix2 GetSpawnPosOffset(ISimWorldReadAccessor accessor, UseContext context, Entity actionPrefab, fix2 direction)
    {
        Settings settings = accessor.GetComponent<Settings>(actionPrefab);

        return direction * GetSpawnDistance(accessor, settings.ProjectilePrefab, context.InstigatorPawn, settings.SpawnExtraDistance);
    }

    // used by presentation
    public fix GetProjectileRadius(ISimWorldReadAccessor accessor, Entity actionPrefab)
    {
        Settings settings = accessor.GetComponent<Settings>(actionPrefab);

        return CommonReads.GetActorRadius(accessor, settings.ProjectilePrefab);
    }
    #endregion
}
