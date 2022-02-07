using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DelayedExplosionSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {

        var deltaTime = Time.DeltaTime;

        Entities.ForEach((Entity entity, ref DelayedExplosion delayedExplosion) =>
        {
            delayedExplosion.TimeDuration -= deltaTime;

            if (delayedExplosion.TimeDuration <= 0)
            {
                var entityTilePos = EntityManager.GetComponentData<FixTranslation>(entity);
                CommonWrites.RequestExplosion(Accessor, entity, entityTilePos, delayedExplosion.Radius, delayedExplosion.Damage, delayedExplosion.DestroyTiles);
                EntityManager.DestroyEntity(entity);
            }
        })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();

        Entities.ForEach((Entity entity, ref DelayedExplosionBlackHole delayedExplosionBlackHole, ref FixTranslation translation) =>
        {
            delayedExplosionBlackHole.TimeDuration -= deltaTime;

            if (delayedExplosionBlackHole.TimeDuration <= 0)
            {
                var hits = CommonReads.Physics.OverlapCircle(Accessor, translation.Value, delayedExplosionBlackHole.Radius, ignoreEntity: entity);

                foreach (DistanceHit hit in hits)
                {
                    if (Accessor.TryGetComponent(hit.Entity, out FixTranslation targetTranslation))
                    {
                        fix2 force = translation.Value - targetTranslation.Value;
                        if (delayedExplosionBlackHole.CustomForce)
                        {
                            force.Normalize();
                            force *= delayedExplosionBlackHole.Force;
                        }

                        // make it so it's not towards the ground
                        force = rotateTowards(force, Angle2DUp, Angle2DUp * (fix)0.3);

                        CommonWrites.RequestImpulse(Accessor, hit.Entity, force);
                    }
                }
                // feedback
                CommonWrites.RequestExplosion(Accessor, entity, translation, delayedExplosionBlackHole.Radius, 0, false);

                EntityManager.DestroyEntity(entity);
            }
        })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();

        Entities.ForEach((Entity entity, ref DelayedExplosionShield delayedExplosionShield, ref FixTranslation translation) =>
        {
            delayedExplosionShield.TimeDuration -= deltaTime;

            if (delayedExplosionShield.TimeDuration <= 0)
            {
                var hits = CommonReads.Physics.OverlapCircle(Accessor, translation.Value, delayedExplosionShield.Radius, ignoreEntity: entity);

                foreach (DistanceHit hit in hits)
                {
                    if (Accessor.HasComponent<Controllable>(hit.Entity))
                    {
                        if (Accessor.TryGetComponent(CommonReads.TryGetPawnController(Accessor, hit.Entity), out Team team))
                        {
                            // Only Player Team (if AI need to use this item, change this to track instigator)
                            if (team.Value == (int)DesignerFriendlyTeam.Player)
                            {
                                if (Accessor.TryGetComponent(hit.Entity, out Invincible invincible))
                                {
                                    Accessor.AddComponent(hit.Entity, new Invincible() { Duration = max(delayedExplosionShield.ShieldDuration, invincible.Duration) });
                                }
                                else
                                {
                                    Accessor.AddComponent(hit.Entity, new Invincible() { Duration = delayedExplosionShield.ShieldDuration });
                                }
                            }
                        }
                    }
                }
                // feedback
                CommonWrites.RequestExplosion(Accessor, entity, translation, delayedExplosionShield.Radius, 0, false);

                EntityManager.DestroyEntity(entity);
            }
        })
            .WithStructuralChanges()
            .WithoutBurst()
            .Run();
    }
}