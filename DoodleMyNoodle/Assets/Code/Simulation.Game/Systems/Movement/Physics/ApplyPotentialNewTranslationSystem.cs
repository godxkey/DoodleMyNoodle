using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(ValidatePotentialNewTranslationSystem))]
[UpdateInGroup(typeof(MovementSystemGroup))]
public class ApplyPotentialNewTranslationSystem : SimJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle deps)
    {
        return Entities
            .WithChangeFilter<PotentialNewTranslation>()
            .ForEach((ref FixTranslation pos, ref PotentialNewTranslation newTranslation) =>
            {
                pos.Value = newTranslation.Value;
            }).Schedule(deps);
    }
}