using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using CCC.Fix2D;

public struct BeserkerMagicProjectile : IComponentData
{
    public Entity Projectile;

    public static implicit operator Entity(BeserkerMagicProjectile val) => val.Projectile;
    public static implicit operator BeserkerMagicProjectile(Entity val) => new BeserkerMagicProjectile() { Projectile = val };
}

public class GameActionMagicAxe : GameAction<GameActionMagicAxe.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix SpeedMin = 0;
        public fix SpeedMax = 10;
        public GameObject MagicBulletPrefab;
        public GameObject AxePrefab;
        [Header("Default Parameter")]
        public Vector2 ShootVector;
        public Vector2 SpawnOffset;
        public bool SaveReferenceOnLastSpell = false;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                SpeedMax = SpeedMax,
                SpeedMin = SpeedMin,
                MagicBulletPrefab = conversionSystem.GetPrimaryEntity(MagicBulletPrefab),
                AxePrefab = conversionSystem.GetPrimaryEntity(AxePrefab),
                ShootVector = new fix2((fix)ShootVector.x, (fix)ShootVector.y),
                SpawnOffset = (fix2)SpawnOffset,
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(MagicBulletPrefab);
            referencedPrefabs.Add(AxePrefab);

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public fix SpeedMax;
        public fix SpeedMin;
        public Entity MagicBulletPrefab;
        public Entity AxePrefab;
        public fix2 ShootVector;
        public fix2 SpawnOffset;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        Entity actionInstigator = input.Context.ActionInstigator;

        if (!input.Accessor.TryGetComponent<BeserkerMagicProjectile>(actionInstigator, out var projectile) || projectile == Entity.Null)
        {
            FireProjectileSettings fireSettings = new FireProjectileSettings()
            {
                InheritInstigatorVelocity = true,
                SimulateSpawnFromInstigatorCenter = true,
                SpawnOffset = settings.SpawnOffset,
            };

            Entity magicProjectile = CommonWrites.FireProjectile(input.Accessor,
                                                             input.ActionInstigator,
                                                             settings.MagicBulletPrefab,
                                                             settings.ShootVector,
                                                             fireSettings);

            input.Accessor.SetOrAddComponent<BeserkerMagicProjectile>(actionInstigator, magicProjectile);
        }
        else
        {
            Entity spinningAxeInstance = input.Accessor.Instantiate(settings.AxePrefab);

            // set first instigator on axe
            input.Accessor.SetOrAddComponent<FirstInstigator>(spinningAxeInstance, CommonReads.GetFirstInstigator(input.Accessor, input.ActionInstigator));
            
            // set position
            input.Accessor.SetComponent(spinningAxeInstance, input.Accessor.GetComponent<FixTranslation>(projectile));

            // destroy projectile
            CommonWrites.DisableAndScheduleForDestruction(input.Accessor, projectile);
            input.Accessor.SetComponent<BeserkerMagicProjectile>(actionInstigator, Entity.Null);
        }

        return true;
    }
}