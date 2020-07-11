using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public struct PotentialNewTranslation : IComponentData
{
    public fix3 Value;
}

public class ApplyVelocitySystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PotentialNewTranslation newTranslation, ref FixTranslation pos, ref Velocity vel) =>
        {
            newTranslation.Value = pos.Value + (vel.Value * deltaTime);
        });
    }
}

[UpdateAfter(typeof(ValidatePotentialNewTranslationSystem))]
public class ApplyPotentialNewTranslationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities//            .WithChangeFilter<PotentialNewTranslation>() UNDO
            .ForEach((ref FixTranslation pos, ref PotentialNewTranslation newTranslation) =>
            {
                pos.Value = newTranslation.Value;
            });
    }
}

[UpdateAfter(typeof(ApplyPotentialNewTranslationSystem))]
public class RecordPreviousTranslationSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities//            .WithChangeFilter<FixTranslation>() UNDO
            .ForEach((ref PreviousFixTranslation pos, ref FixTranslation newTranslation) =>
            {
                pos.Value = newTranslation.Value;
            });
    }
}