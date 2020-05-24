using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public struct PotentialNewTranslation : IComponentData
{
    public fix3 Value;
}

public class ApplyVelocitySystem : SimJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PotentialNewTranslation newTranslation, in FixTranslation pos, in Velocity vel) =>
        {
            newTranslation.Value = pos.Value + (vel.Value * deltaTime);
        }).Schedule(inputDeps).Complete();

        return default;
    }
}

[UpdateAfter(typeof(ValidatePotentialNewTranslationSystem))]
public class ApplyPotentialNewTranslationSystem : SimJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Entities.ForEach((ref FixTranslation pos, in PotentialNewTranslation newTranslation) =>
        {
            pos.Value = newTranslation.Value;
        }).Schedule(inputDeps).Complete();

        return default;
    }
}