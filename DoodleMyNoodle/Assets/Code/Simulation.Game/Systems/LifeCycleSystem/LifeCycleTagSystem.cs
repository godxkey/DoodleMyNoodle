using Unity.Entities;

//public struct NewlyCreatedTag : ISystemStateComponentData { }
//public struct MidLifeCycleTag : IComponentData { }
//public struct MidLifeCycleSystemTag : ISystemStateComponentData { }
//public struct NewlyDestroyedTag : ISystemStateComponentData { }

//[UpdateInGroup(typeof(PresentationSystemGroup))] // update at the very end of the simulation
//public partial class LifeCycleTagSystem : SimGameSystemBase
//{
//    protected override void OnUpdate()
//    {
//        // Entities life cycle:
//        // tick 0: Freshly created - no components
//        // tick 1: NewlyCreatedTag + MidLifeCycleTag + MidLifeCycleSystemTag
//        // tick 2:                   MidLifeCycleTag + MidLifeCycleSystemTag
//        // ...
//        // (destroy!)
//        // tick 38:                                    MidLifeCycleSystemTag                        <- NB:  here the entity lost its MidLifeCycleTag because it is not a ISystemStateComponentData
//        // tick 39:                                                            NewlyDestroyedTag
//        // tick 40: (entity truly destroyed)


//        // Each job represents one of those transformations
//        // They should be scheduled in reverse order so that we let 1 tick happen inbetween some transformations
//        //  (e.g. NewlyCreatedTag should stay for 1 tick before getting removed)

//        // Remove NewlyDestroyedTag - truly destroying the entity
//        Entities
//            .WithAll<NewlyDestroyedTag>()
//            .ForEach((Entity entity) =>
//        {
//            EntityManager.RemoveComponent<NewlyDestroyedTag>(entity);
//        })
//            .WithStructuralChanges()
//            .WithoutBurst()
//            .Run();

//        // Change MidLifeCycleSystemTag for NewlyDestroyedTag
//        Entities
//            .WithAll<MidLifeCycleSystemTag>()
//            .WithNone<MidLifeCycleTag>()
//            .ForEach((Entity entity) =>
//        {
//            EntityManager.AddComponent<NewlyDestroyedTag>(entity);
//            EntityManager.RemoveComponent<MidLifeCycleSystemTag>(entity);
//        })
//            .WithStructuralChanges()
//            .WithoutBurst()
//            .Run();

//        // Remove NewlyCreatedTag
//        Entities
//            .WithAll<NewlyCreatedTag>()
//            .ForEach((Entity entity) =>
//        {
//            EntityManager.RemoveComponent<NewlyCreatedTag>(entity);
//        })
//            .WithStructuralChanges()
//            .WithoutBurst()
//            .Run();

//        // New entity! Add NewlyCreatedTag + MidLifeCycleTag + MidLifeCycleSystemTag
//        Entities
//            .WithNone<MidLifeCycleTag, MidLifeCycleSystemTag, NewlyDestroyedTag>()
//            .WithAll<SimAssetId>()
//            .ForEach((Entity entity) =>
//        {
//            EntityManager.AddComponent<NewlyCreatedTag>(entity);
//            EntityManager.AddComponent<MidLifeCycleTag>(entity);
//            EntityManager.AddComponent<MidLifeCycleSystemTag>(entity);
//        })
//            .WithStructuralChanges()
//            .WithoutBurst()
//            .Run();

//        // mid-life cycle entity! Add MidLifeCycleSystemTag
//        Entities
//            .WithAll<MidLifeCycleTag>()
//            .WithNone<MidLifeCycleSystemTag>()
//            .ForEach((Entity entity) =>
//            {
//                EntityManager.AddComponent<MidLifeCycleSystemTag>(entity);
//            })
//            .WithStructuralChanges()
//            .WithoutBurst()
//            .Run();
//    }
//}