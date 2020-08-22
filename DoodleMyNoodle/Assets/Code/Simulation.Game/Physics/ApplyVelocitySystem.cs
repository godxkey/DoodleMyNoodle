using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;

public struct PotentialNewTranslation : IComponentData
{
    public fix3 Value;

    public static implicit operator fix3(PotentialNewTranslation val) => val.Value;
    public static implicit operator PotentialNewTranslation(fix3 val) => new PotentialNewTranslation() { Value = val };
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