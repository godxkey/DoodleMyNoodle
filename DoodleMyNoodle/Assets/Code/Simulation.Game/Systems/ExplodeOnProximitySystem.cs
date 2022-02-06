using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class ExplodeOnProximitySystem : SimGameSystemBase
{
    private List<Entity> _toDestroy = new List<Entity>();

    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entityA, ref ExplodeOnProximity explodeOnProximity, in FixTranslation fixTranslationA) =>
        {
            if (explodeOnProximity.Activated)
            {
                fix2 translationA = fixTranslationA.Value;
                fix distanceToExplode = explodeOnProximity.Distance;

                fix explosionRadius = explodeOnProximity.Radius;
                int explosionDamage = explodeOnProximity.Damage;

                bool destroyTiles = explodeOnProximity.DestroyTiles;

                Entities
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach((Entity entityB, in FixTranslation fixTranslationB, in PhysicsVelocity physicsVelocity) =>
                {
                    if (entityA != entityB)
                    {
                        fix2 deltaPos = fixTranslationB.Value - translationA;

                        if (deltaPos.lengthSquared <= pow2(distanceToExplode))
                        {
                            CommonWrites.RequestExplosion(Accessor, entityA, translationA, explosionRadius, explosionDamage, destroyTiles);
                            _toDestroy.Add(entityA);
                        }
                    }
                }).Run();
            }
        }).Run();

        foreach (var entity in _toDestroy)
        {
            EntityManager.DestroyEntity(entity);
        }

        _toDestroy.Clear();
    }
}