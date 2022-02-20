using Unity.Entities;

public class FrozenEffectSystem : SimGameSystemBase
{
    private EntityCommandBufferSystem _ecbSytem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSytem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        MultiTimeValue multiElapsedTime = GetElapsedTime();

        var ecb = _ecbSytem.CreateCommandBuffer();

        Entities
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((ref Frozen frozenEffect, in Entity entity) =>
            {
                TimeValue elapsedTime = multiElapsedTime.GetValue(frozenEffect.Duration.Type);

                if ((elapsedTime - frozenEffect.AppliedTime) >= frozenEffect.Duration)
                {
                    ecb.RemoveComponent<Frozen>(entity);
                }
            }).Run();

        _ecbSytem.AddJobHandleForProducer(Dependency);
    }
}