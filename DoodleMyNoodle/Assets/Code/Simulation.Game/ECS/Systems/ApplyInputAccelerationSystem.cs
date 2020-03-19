using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateAfter(typeof(ExecutePlayerInputSystem))]
[UpdateBefore(typeof(ApplyVelocitySystem))]
public class ApplyInputAccelerationSystem : SimJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Velocity velocity, in InputAcceleration acceleration) =>
        {
            velocity.Value += acceleration.Value * deltaTime;

        }).Schedule(inputDeps).Complete();

        return default;
    }
}