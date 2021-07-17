using Unity.Entities;

public class ReviveEntitySystem : SimSystemBase
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
        Entities
            .WithAll<DeadTag>()
            .ForEach((Entity entity, in Health health) =>
        {
            if (health.Value > 0)
            {
                ecb.RemoveComponent<DeadTag>(entity);
            }
        }).Run();
    }
}