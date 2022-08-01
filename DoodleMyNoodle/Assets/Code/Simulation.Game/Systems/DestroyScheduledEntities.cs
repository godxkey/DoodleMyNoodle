using Unity.Entities;
using CCC.Fix2D;

public struct ScheduledDestroyTimestamp : IComponentData
{
    public fix Value;

    public static implicit operator fix(ScheduledDestroyTimestamp val) => val.Value;
    public static implicit operator ScheduledDestroyTimestamp(fix val) => new ScheduledDestroyTimestamp() { Value = val };
}

public partial class DestroyScheduledEntities : SimGameSystemBase
{
    private EndSimulationEntityCommandBufferSystem _ecbSystem;
    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer();
        fix time = Time.ElapsedTime;
        Entities
            .WithAll<Disabled>()
            .ForEach((Entity entity, in ScheduledDestroyTimestamp scheduledDestroyTimestamp) =>
        {
            if (scheduledDestroyTimestamp.Value < time)
            {
                ecb.DestroyEntity(entity);
            }
        }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}
