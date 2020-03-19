using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(CreateBindedViewEntitiesSystem))]
[UpdateBefore(typeof(CopyTransformToViewSystem))]
public class PostSimulationBindingCommandBufferSystem : ViewEntityCommandBufferSystem { }

[UpdateAfter(typeof(DestroyDanglingViewSystem))]
[UpdateAfter(typeof(BeginViewSystem))]
public class CreateBindedViewEntitiesSystem : ViewJobComponentSystem
{
    private EntityQuery _newSimEntitiesQ;
    private EntityQuery _allSimEntitiesQ;
    private PostSimulationBindingCommandBufferSystem _ecbSystem;

    private DirtyValue<uint> _simWorldEntityClearAndReplaceCount;

    protected override void OnCreate()
    {
        base.OnCreate();

        _newSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<BlueprintId>());
        _allSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<BlueprintId>());
        _ecbSystem = World.GetOrCreateSystem<PostSimulationBindingCommandBufferSystem>();

        RequireSingletonForUpdate<Settings_ViewBindingSystem_BlueprintDefinition>();
    }

    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        var settingsEntity = GetSingletonEntity<Settings_ViewBindingSystem_BlueprintDefinition>();

        // fbessette: we use the 'EntityClearAndReplaceCount' to mesure when we should replace all view entities.
        //            This doesn't feel like the best way to do it... Feel free to refactor
        _simWorldEntityClearAndReplaceCount.Set(SimWorldAccessor.EntityClearAndReplaceCount);
        if (_simWorldEntityClearAndReplaceCount.IsDirty)
        {
            _simWorldEntityClearAndReplaceCount.Reset();

            jobHandle = new FetchAllSimEntitiesJob()
            {
                Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent(),
                BlueprintDefinitions = EntityManager.GetBuffer<Settings_ViewBindingSystem_BlueprintDefinition>(settingsEntity)
            }.Schedule(_allSimEntitiesQ, jobHandle);
        }
        else
        {
            jobHandle = new FetchNewSimEntitiesJob()
            {
                Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent(),
                BlueprintDefinitions = EntityManager.GetBuffer<Settings_ViewBindingSystem_BlueprintDefinition>(settingsEntity)
            }.Schedule(_newSimEntitiesQ, jobHandle);
        }

        _ecbSystem.AddJobHandleForProducer(jobHandle);

        jobHandle.Complete();
        return jobHandle;
    }

    [BurstCompile]
    [RequireComponentTag(typeof(NewlyCreatedTag))]
    struct FetchNewSimEntitiesJob : IJobForEachWithEntity_EC<BlueprintId>
    {
        [ReadOnly] public DynamicBuffer<Settings_ViewBindingSystem_BlueprintDefinition> BlueprintDefinitions;
        public EntityCommandBuffer.Concurrent Ecb;

        public void Execute(Entity simEntity, int index, [ReadOnly] ref BlueprintId blueprintId)
        {
            for (int i = 0; i < BlueprintDefinitions.Length; i++)
            {
                if (BlueprintDefinitions[i].BlueprintId == blueprintId.Value)
                {
                    Entity presentationEntity = Ecb.Instantiate(index, BlueprintDefinitions[i].PresentationEntity);
                    Ecb.AddComponent(index, presentationEntity, new BindedSimEntity() { SimWorldEntity = simEntity });
                    break;
                }
            }
        }
    }

    // Same code, no RequireComponentTag
    [BurstCompile]
    struct FetchAllSimEntitiesJob : IJobForEachWithEntity_EC<BlueprintId>
    {
        [ReadOnly] public DynamicBuffer<Settings_ViewBindingSystem_BlueprintDefinition> BlueprintDefinitions;
        public EntityCommandBuffer.Concurrent Ecb;

        public void Execute(Entity simEntity, int index, [ReadOnly] ref BlueprintId blueprintId)
        {
            for (int i = 0; i < BlueprintDefinitions.Length; i++)
            {
                if (BlueprintDefinitions[i].BlueprintId == blueprintId.Value)
                {
                    Entity presentationEntity = Ecb.Instantiate(index, BlueprintDefinitions[i].PresentationEntity);
                    Ecb.AddComponent(index, presentationEntity, new BindedSimEntity() { SimWorldEntity = simEntity });
                    break;
                }
            }
        }
    }
}