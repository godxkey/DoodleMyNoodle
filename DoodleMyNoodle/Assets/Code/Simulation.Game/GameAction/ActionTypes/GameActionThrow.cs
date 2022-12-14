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
using CCC.InspectorDisplay;

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
        public int Quantity = 1;
        public bool AimBot;
        [ShowIf(nameof(AimBot))]
        public bool AimBotPrioritizeFlying;
        [ShowIf(nameof(AimBot))]
        public float AimBotRangeMax = 12.5f;
        [Suffix("Degrees")]
        public float VolleyAngle = 0;
        [Header("Default Parameter")]
        public Vector2 ShootVector;
        public Vector2 SpawnOffset;

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
                ShootVector = new fix2((fix)ShootVector.x, (fix)ShootVector.y),
                SpawnOffset = (fix2)SpawnOffset,
                AimBot = AimBot,
                AimBotPrioritizeFlying = AimBotPrioritizeFlying,
                AimBotRangeMax = (fix)AimBotRangeMax,
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
        public fix2 SpawnOffset;
        public bool AimBot;
        public bool AimBotPrioritizeFlying;
        public fix AimBotRangeMax;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract(
            new GameActionParameterVector.Description()
            {
                SpeedMin = settings.SpeedMin,
                SpeedMax = settings.SpeedMax
            });
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        fix2 shootVelocity = settings.ShootVector;
        bool inheritInstigatorVelocity = true;

        // When 'originateFromCenter' is true, we simulate the projectile spawning at the center of the pawn.
        // We adjust the start position and the start velocity to where the projectile will exit the spawn-distance
        bool originateFromCenter = true;//input.Parameters != null && input.Parameters.TryGetParameter(1, out GameActionParameterBool.Data paramOriginateFromCenter, warnIfFailed: false)
                                        //&& paramOriginateFromCenter.Value;

        if (input.Parameters != null && input.Parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            shootVelocity = paramVector.Vector;
        }
        else if (settings.AimBot)
        {
            // NB: This only supports players shooting at mobs for the moment. We could modify it so that mobs can aimbot shoot at players as well
            fix2 instigatorPos = input.Accessor.GetComponent<FixTranslation>(input.ActionInstigatorActor);
            FixRange targetRange = new FixRange(instigatorPos.x, instigatorPos.x + settings.AimBotRangeMax);

            Entity closestEnemy = CommonReads.FindClosestEnemyInRange(input.Accessor, instigatorPos.x, targetRange, settings.AimBotPrioritizeFlying);
            if (closestEnemy != Entity.Null)
            {
                inheritInstigatorVelocity = false;
                originateFromCenter = true;
                var targetPos = input.Accessor.GetComponent<FixTranslation>(closestEnemy).Value;
                var gravity = input.Accessor.GetSingleton<PhysicsStepSettings>().GravityFix;
                var targetRadius = CommonReads.GetActorRadius(input.Accessor, closestEnemy);
                // aim a bit in front of the character to account for move speed (cheap fix instead of predicting end position)
                targetPos.x -= targetRadius * (fix)0.8;
                shootVelocity = Trajectory.SmallestLaunchVelocity(targetPos.x - instigatorPos.x, targetPos.y - instigatorPos.y, gravity);
            }
        }

        FireProjectileSettings fireSettings = new FireProjectileSettings()
        {
            InheritInstigatorVelocity = inheritInstigatorVelocity,
            SimulateSpawnFromInstigatorCenter = originateFromCenter,
            SpawnOffset = settings.SpawnOffset,
        };

        CommonWrites.FireProjectile(input.Accessor,
                           input.ActionInstigator,
                           settings.ProjectilePrefab,
                           shootVelocity,
                           fireSettings,
                           settings.Quantity,
                           settings.VolleyAngle);
        return true;
    }

    #region Used by presentation
    // used by presentation
    public fix2 GetSpawnPosOffset(ISimWorldReadAccessor accessor, ExecutionContext context, Entity actionPrefab, fix2 direction)
    {
        Settings settings = accessor.GetComponent<Settings>(actionPrefab);

        return fix2.zero;// (direction * CommonReads.GetThrowSpawnDistance(accessor, settings.ProjectilePrefab, context.LastPhysicalInstigator, settings.SpawnExtraDistance)) 
                         //+ settings.SpawnOffset;
    }

    // used by presentation
    public fix GetProjectileRadius(ISimWorldReadAccessor accessor, Entity actionPrefab)
    {
        Settings settings = accessor.GetComponent<Settings>(actionPrefab);

        return CommonReads.GetActorRadius(accessor, settings.ProjectilePrefab);
    }
    #endregion
}

public partial class CommonReads
{
    public static fix GetThrowSpawnDistance(ISimWorldReadAccessor accessor, Entity projectile, Entity instigator, fix extraDistance)
    {
        fix projectileRadius = CommonReads.GetActorRadius(accessor, projectile);
        fix instigatorRadius = CommonReads.GetActorRadius(accessor, instigator);

        return instigatorRadius + projectileRadius + (fix)0.05f + extraDistance;
    }
}

public struct FireProjectileSettings
{
    /// <summary>
    /// If true, will simulate the projectile spawning from the center of the instigator. The start velocity and position of the projectile will be adjusted, based
    /// on where the projectile would have exited the instigator collider. This is mostly useful for AI shooting accuracy.
    /// </summary>
    public bool SimulateSpawnFromInstigatorCenter;

    /// <summary>
    /// If true, adds the velocity of the last physical instigator to the projectile fire velocity
    /// </summary>
    public bool InheritInstigatorVelocity;

    /// <summary>
    /// The offset applied to the spawn position of the projectiles.
    /// </summary>
    public fix2 SpawnOffset;

    public static FireProjectileSettings Default => new()
    {
        SimulateSpawnFromInstigatorCenter = false,
        InheritInstigatorVelocity = true,
        SpawnOffset = default
    };
}

internal partial class CommonWrites
{
    /// <summary>
    /// Spawn a projectile copy and throw it with a velocity.
    /// </summary>
    /// <param name="instigatorSet">The instigators of this action. Should always be set properly.</param>
    /// <param name="projectilePrefab">The prefab of the projectile.</param>
    /// <param name="throwVelocity">The velocity of the projectile.</param>
    public static Entity FireProjectile(
        ISimGameWorldReadWriteAccessor accessor,
        Entity instigator,
        Entity projectilePrefab,
        fix2 throwVelocity,
        FireProjectileSettings settings)
    {
        return FireProjectileInternal(accessor, instigator, projectilePrefab, throwVelocity, settings, 1, 0, default);
    }

    /// <summary>
    /// Spawn 1 or more projectile copies and throw them with a velocity.
    /// </summary>
    /// <param name="instigatorSet">The instigators of this action. Should always be set properly.</param>
    /// <param name="projectilePrefab">The prefab of the projectile.</param>
    /// <param name="fireVelocity">The velocity of the projectile.</param>
    /// <param name="quantity">The quantity of projectiles to throw.</param>
    /// <param name="volleyAngle">The separation angle between each projectile thrown.</param>
    public static Entity FireProjectile(
        ISimGameWorldReadWriteAccessor accessor,
        Entity instigator,
        Entity projectilePrefab,
        fix2 fireVelocity,
        FireProjectileSettings settings,
        int quantity,
        fix volleyAngle,
        NativeList<Entity> outSpawnedProjectiles = default)
    {
        return FireProjectileInternal(accessor, instigator, projectilePrefab, fireVelocity, settings, quantity, volleyAngle, outSpawnedProjectiles);
    }

    private static Entity FireProjectileInternal(
        ISimGameWorldReadWriteAccessor accessor,
        Entity instigator,
        Entity projectilePrefab,
        fix2 throwVelocity,
        FireProjectileSettings settings,
        int quantity,
        fix volleyAngle,
        NativeList<Entity> outSpawnedProjectiles)
    {
        bool simulateSpawnFromInstigatorCenter = settings.SimulateSpawnFromInstigatorCenter;
        fix spawnExtraDistance = 0; // kept here for legacy
        fix2 spawnOffset = settings.SpawnOffset;
        bool inheritInstigatorVelocity = settings.InheritInstigatorVelocity;

        if (quantity <= 1)
        {
            volleyAngle = 0;
        }

        Entity instigatorBody = CommonReads.GetOwnerActor(accessor, instigator);
        fix throwSpeed = length(throwVelocity);
        fix throwAngle = throwSpeed < (fix)0.01 ? 0 : angle2d(throwVelocity);
        fix throwAngleMin = throwAngle - (volleyAngle / 2);
        fix throwAngleIncrement = quantity == 1 ? 0 : volleyAngle / (quantity - 1);
        fix2 instigatorPos = accessor.GetComponent<FixTranslation>(instigatorBody);
        fix2 instigatorVel = fix2.zero;
        if (inheritInstigatorVelocity && accessor.TryGetComponent(instigatorBody, out PhysicsVelocity instigatorPhysicsVelocity))
        {
            instigatorVel = instigatorPhysicsVelocity.Linear;
        }

        fix spawnDistance = CommonReads.GetThrowSpawnDistance(accessor, projectilePrefab, instigatorBody, spawnExtraDistance);
        uint projectileGroupID = quantity > 1 ? accessor.MakeUniquePersistentId().Value : 0;

        Entity lastProjectileInstance = Entity.Null;
        Entity firstInstigator = CommonReads.GetFirstInstigator(accessor, instigator);
        for (int i = 0; i < quantity; i++)
        {
            // _________________________________________ Spawn Projectile _________________________________________ //
            Entity projectileInstance = accessor.Instantiate(projectilePrefab);

            // _________________________________________ Calculate Component Details _________________________________________ //
            fix itemThrowAngle = throwAngleMin + (i * throwAngleIncrement);
            fix2 itemThrowDir = fix2.FromAngle(itemThrowAngle);
            fix2 itemThrowVelocity = itemThrowDir * throwSpeed;

            fix2 spawnPos;
            if (simulateSpawnFromInstigatorCenter)
            {
                // Find how much the projectile will be affected by gravity
                fix2 gravity = fix2.zero;
                if (accessor.HasComponent<PhysicsGravity>(projectilePrefab))
                {
                    gravity = accessor.GetExistingSystem<PhysicsWorldSystem>().PhysicsWorld.StepSettings.GravityFix
                        * accessor.GetComponent<PhysicsGravity>(projectilePrefab).ScaleFix;
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

            spawnPos += spawnOffset;

            // _________________________________________ Set Projectile Data _________________________________________ //
            accessor.SetOrAddComponent(projectileInstance, new PhysicsVelocity(itemThrowVelocity + instigatorVel));
            accessor.SetOrAddComponent(projectileInstance, new FixTranslation(spawnPos));
            accessor.SetOrAddComponent(projectileInstance, new FirstInstigator() { Value = firstInstigator });

            if (quantity > 1)
            {
                accessor.SetOrAddComponent(projectileInstance, new EffectGroupComponent() { ID = projectileGroupID });
            }

            if (outSpawnedProjectiles.IsCreated)
                outSpawnedProjectiles.Add(projectileInstance);

            lastProjectileInstance = projectileInstance;
        }

        return lastProjectileInstance;
    }
}