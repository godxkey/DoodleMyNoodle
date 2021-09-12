using Unity.Entities;
using CCC.Fix2D;

[UpdateBefore(typeof(DestroyDeadEntitiesSystem))]
public class ExplodeOnDeathSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Health health, ref FixTranslation translation, ref ExplodeOnDeath explodeOnDeath) =>
            {
                if (health.Value <= 0)
                {
                    CommonWrites.RequestExplosion(Accessor, entity, translation.Value, explodeOnDeath.Radius, explodeOnDeath.Damage, explodeOnDeath.DestroyTiles);
                }
            }).Run();
    }
}
