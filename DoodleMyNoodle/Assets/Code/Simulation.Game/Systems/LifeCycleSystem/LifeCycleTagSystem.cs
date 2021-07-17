using Unity.Entities;

public class LifeCycleTagSystem : SimSystemBase
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

        // Entities life cycle:
        // tick 0: Freshly created - no components
        // tick 1: NewlyCreatedTag + MidLifeCycleTag + MidLifeCycleSystemTag
        // tick 2:                   MidLifeCycleTag + MidLifeCycleSystemTag
        // ...
        // (destroy!)
        // tick 38:                                    MidLifeCycleSystemTag                        <- NB:  here the entity lost its MidLifeCycleTag because it is not a ISystemStateComponentData
        // tick 39:                                                            NewlyDestroyedTag
        // tick 40: (entity truly destroyed)


        // Each job represents one of those transformations
        // They should be scheduled in reverse order so that we let 1 tick happen inbetween some transformations
        //  (e.g. NewlyCreatedTag should stay for 1 tick before getting removed)

        // Remove NewlyDestroyedTag - truly destroying the entity
        Entities
            .WithAll<NewlyDestroyedTag>()
            .ForEach((Entity entity) =>
        {
            ecb.RemoveComponent<NewlyDestroyedTag>(entity);
        }).Run();

        // Change MidLifeCycleSystemTag for NewlyDestroyedTag
        Entities
            .WithAll<MidLifeCycleSystemTag>()
            .WithNone<MidLifeCycleTag>()
            .ForEach((Entity entity) =>
        {
            ecb.AddComponent<NewlyDestroyedTag>(entity);
            ecb.RemoveComponent<MidLifeCycleSystemTag>(entity);
        }).Run();

        // Remove NewlyCreatedTag
        Entities
            .WithAll<NewlyCreatedTag>()
            .ForEach((Entity entity) =>
        {
            ecb.RemoveComponent<NewlyCreatedTag>(entity);
        }).Run();

        // New entity! Add NewlyCreatedTag + MidLifeCycleTag + MidLifeCycleSystemTag
        Entities
            .WithNone<MidLifeCycleTag, MidLifeCycleSystemTag, NewlyDestroyedTag>()
            .WithAll<SimAssetId>()
            .ForEach((Entity entity) =>
        {
            ecb.AddComponent<NewlyCreatedTag>(entity);
            ecb.AddComponent<MidLifeCycleTag>(entity);
            ecb.AddComponent<MidLifeCycleSystemTag>(entity);
        }).Run();

        // mid-life cycle entity! Add MidLifeCycleSystemTag
        Entities
            .WithAll<MidLifeCycleTag>()
            .WithNone<MidLifeCycleSystemTag>()
            .ForEach((Entity entity) =>
            {
                ecb.AddComponent<MidLifeCycleSystemTag>(entity);
            }).Run();
    }
}