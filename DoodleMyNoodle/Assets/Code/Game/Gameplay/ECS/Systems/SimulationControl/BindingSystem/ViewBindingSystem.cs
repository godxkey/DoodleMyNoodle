using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(BeginViewSystem))]
public class ViewBindingSystem : ViewJobComponentSystem
{
    private EntityQuery _newSimEntitiesQ;
    private BeginPresentationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _newSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<BlueprintId>());
        _ecbSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<Settings_ViewBindingSystem_BlueprintDefinition>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        var settingsEntity = GetSingletonEntity<Settings_ViewBindingSystem_BlueprintDefinition>();

        var jobHandle = new FetchNewSimEntitiesJob()
        {
            Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent(),
            BlueprintDefinitions = EntityManager.GetBuffer<Settings_ViewBindingSystem_BlueprintDefinition>(settingsEntity)
        }.Schedule(_newSimEntitiesQ, inputDependencies);

        _ecbSystem.AddJobHandleForProducer(jobHandle);

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
}