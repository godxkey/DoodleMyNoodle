using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(ValidatePotentialNewTranslationSystem))]
[UpdateInGroup(typeof(MovementSystemGroup))]
public class ApplyPotentialNewTranslationSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithChangeFilter<PotentialNewTranslation>()
            .ForEach((ref FixTranslation pos, ref PotentialNewTranslation newTranslation) =>
            {
                pos.Value = newTranslation.Value;
            }).Schedule();
    }
}