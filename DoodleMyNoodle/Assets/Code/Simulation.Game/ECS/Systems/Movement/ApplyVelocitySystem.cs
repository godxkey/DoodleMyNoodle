using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

public class ApplyVelocitySystem : SimJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        Fix64 deltaTime = Time.DeltaTime;

        Entities.ForEach((ref FixTranslation pos, in Velocity vel) =>
        {
            pos.Value += vel.Value * deltaTime;

        }).Schedule(inputDeps).Complete();

        return default;
    }
}