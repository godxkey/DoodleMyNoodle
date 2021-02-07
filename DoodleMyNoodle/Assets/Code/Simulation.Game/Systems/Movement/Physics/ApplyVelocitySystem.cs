using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public struct PotentialNewTranslation : IComponentData
{
    public fix2 Value;

    public static implicit operator fix2(PotentialNewTranslation val) => val.Value;
    public static implicit operator PotentialNewTranslation(fix2 val) => new PotentialNewTranslation() { Value = val };
}

[UpdateInGroup(typeof(MovementSystemGroup))]
public class ApplyVelocitySystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        fix deltaTime = Time.DeltaTime;

        Entities.ForEach((ref PotentialNewTranslation newTranslation, in FixTranslation pos, in Velocity vel) =>
        {
            newTranslation.Value = pos.Value + (vel.Value * deltaTime);
        }).Run();
    }
}