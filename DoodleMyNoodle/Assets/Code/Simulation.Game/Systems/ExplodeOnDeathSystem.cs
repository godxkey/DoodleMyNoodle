using Unity.Entities;
using CCC.Fix2D;

[UpdateBefore(typeof(DestroyDeadEntitiesSystem))]
public class ExplodeOnDeathSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Health health, ref FixTranslation translation, ref ExplodeOnDeath explodeOnDeath) =>
            {
                if (health.Value <= 0 && !explodeOnDeath.HasExploded)
                {
                    CommonWrites.RequestExplosion(Accessor, entity, translation.Value, explodeOnDeath.Radius, explodeOnDeath.Damage, explodeOnDeath.DestroyTiles);
                    explodeOnDeath.HasExploded = true;
                }
            }).Run();
    }
}
