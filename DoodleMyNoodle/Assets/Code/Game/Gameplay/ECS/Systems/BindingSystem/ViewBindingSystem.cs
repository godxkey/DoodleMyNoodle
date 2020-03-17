using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

[UpdateAfter(typeof(DestroyDanglingViewSystem))]
[UpdateAfter(typeof(BeginViewSystem))]
public class ViewBindingSystem : ViewJobComponentSystem
{
    private EntityQuery _newSimEntitiesQ;
    private EntityQuery _allSimEntitiesQ;
    private BeginPresentationEntityCommandBufferSystem _ecbSystem;

    private DirtyValue<uint> _simWorldEntityClearAndReplaceCount;

    protected override void OnCreate()
    {
        base.OnCreate();

        _newSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<BlueprintId>());
        _allSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<BlueprintId>());
        _ecbSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();

        RequireSingletonForUpdate<Settings_ViewBindingSystem_BlueprintDefinition>();
    }

    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        var settingsEntity = GetSingletonEntity<Settings_ViewBindingSystem_BlueprintDefinition>();

        // TODO fbessette: mini clean-up
        _simWorldEntityClearAndReplaceCount.Set(World.GetExistingSystem<SimulationControl.SimulationWorldSystem>().SimulationWorld.EntityClearAndReplaceCount);
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