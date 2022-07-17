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
        FireProjectileSettings fireSettings = new FireProjectileSettings()
        {
            InheritInstigatorVelocity = true,
            SimulateSpawnFromInstigatorCenter = true,
            SpawnOffset = settings.SpawnOffset,

        };

        if (!input.Accessor.HasComponent<BeserkerMagicProjectile>(input.Context.ActionInstigator) || 
            (input.Accessor.HasComponent<BeserkerMagicProjectile>(input.Context.ActionInstigator) && input.Accessor.GetComponent<BeserkerMagicProjectile>(input.Context.ActionInstigator) == Entity.Null))
        {
            Entity magicProjectile = CommonWrites.FireProjectile(input.Accessor,
                                                             input.Context.InstigatorSet,
                                                             settings.MagicBulletPrefab,
                                                             settings.ShootVector,
                                                             fireSettings);

            input.Accessor.SetOrAddComponent(input.Context.ActionInstigator, new BeserkerMagicProjectile() { Projectile = magicProjectile });
        }
        else
        {
            BeserkerMagicProjectile magicProjectileRef = input.Accessor.GetComponent<BeserkerMagicProjectile>(input.Context.ActionInstigator);

            Entity instance = input.Accessor.Instantiate(settings.AxePrefab);
            input.Accessor.SetOrAddComponent(instance, new FixTranslation(input.Accessor.GetComponent<FixTranslation>(magicProjectileRef.Projectile)));

            var endSimECMB = input.Accessor.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
            endSimECMB.CreateCommandBuffer().DestroyEntity(input.Accessor.GetComponent<BeserkerMagicProjectile>(input.Context.ActionInstigator));

            input.Accessor.SetComponent(input.Context.ActionInstigator, new BeserkerMagicProjectile() { Projectile = Entity.Null });
        }

        return true;
    }
}