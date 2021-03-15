using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngineX;

public struct BindedViewType : ISharedComponentData
{
    public ViewTechType Value;

    public static implicit operator ViewTechType(BindedViewType val) => val.Value;
    public static implicit operator BindedViewType(ViewTechType val) => new BindedViewType() { Value = val };
}

public class BindedViewEntityCommandBufferSystem : ViewEntityCommandBufferSystem { }

[UpdateBefore(typeof(BindedViewEntityCommandBufferSystem))]
[AlwaysUpdateSystem]
public class MaintainBindedViewEntitiesSystem : ViewSystemBase
{
    private EntityQuery _newSimEntitiesQ;
    private EntityQuery _allSimEntitiesQ;

    private DirtyValue<uint> _simWorldReplaceVersion;
    private DirtyValue<uint> _simWorldTickId;
    private BindedViewEntityCommandBufferSystem _ecb;
    private EntityArchetype _viewEntityArchetype;

    protected override void OnCreate()
    {
        base.OnCreate();
        _ecb = World.GetOrCreateSystem<BindedViewEntityCommandBufferSystem>();
        _viewEntityArchetype = EntityManager.CreateArchetype(
            typeof(BindedSimEntity),
            typeof(SimAssetId),
            typeof(BindedViewType));
    }

    protected override void OnUpdate()
    {
        if (!SimAssetBankInstance.Ready)
            return;

        _simWorldReplaceVersion.Set(SimWorldAccessor.ReplaceVersion);
        _simWorldTickId.Set(SimWorldAccessor.ExpectedNewTickId);

        if (!_simWorldTickId.IsDirty && !_simWorldReplaceVersion.IsDirty) // only continue if sim has changed since last update
            return;

        _simWorldTickId.ClearDirty();

        if (_simWorldReplaceVersion.ClearDirty()) // world replaced!
        {
            // update cached sim queries since world got replaced
            // NB: No need to dispose of these queries because the world gets disposed ...
            _newSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<SimAssetId>());
            _allSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<SimAssetId>());

            // Destroy all view
            DestroyAllViewEntities();

            // Create all view
            CreateNewViewEntities(SimAssetBankInstance.GetJobLookup(), _allSimEntitiesQ);
        }
        else
        {
            // Destroy dangling view
            DestroyDanglingViewEntities();

            // Create new view
            CreateNewViewEntities(SimAssetBankInstance.GetJobLookup(), _newSimEntitiesQ);
        }

    }

    private void DestroyAllViewEntities()
    {
        EntityManager.DestroyEntity(GetEntityQuery(typeof(BindedSimEntity)));
    }

    private void DestroyDanglingViewEntities()
    {
        // Destroy view entities binded with non-existant sim entities
        var ecb = _ecb.CreateCommandBuffer();
        var simEntities = SimWorldAccessor.GetComponentDataFromEntity<SimAssetId>();
        var jobHandle = Entities
            .WithReadOnly(simEntities)
            .ForEach((Entity viewEntity, int entityInQueryIndex, in BindedSimEntity linkedSimEntity) =>
            {
                if (!simEntities.HasComponent(linkedSimEntity.SimEntity))
                {
                    ecb.DestroyEntity(viewEntity);
                }
            }).Schedule(Dependency);

        _ecb.AddJobHandleForProducer(jobHandle);
    }

    private void CreateNewViewEntities(SimAssetBank.JobLookup simAssetBank, EntityQuery simEntitiesInNeedOfViewBinding)
    {
        var ecb = _ecb.CreateCommandBuffer();
        NativeArray<Entity> simEntities = simEntitiesInNeedOfViewBinding.ToEntityArray(Allocator.TempJob);
        NativeArray<SimAssetId> simAssetIds = simEntitiesInNeedOfViewBinding.ToComponentDataArray<SimAssetId>(Allocator.Temp);

        for (int i = 0; i < simEntities.Length; i++)
        {
            if (simAssetBank.TryGetViewTechType(simAssetIds[i], out ViewTechType viewTechType))
            {
                Entity viewEntity = ecb.CreateEntity(_viewEntityArchetype);

                ecb.SetComponent(viewEntity, new BindedSimEntity() { SimEntity = simEntities[i] });
                ecb.SetComponent(viewEntity, simAssetIds[i]);
                ecb.SetSharedComponent(viewEntity, new BindedViewType() { Value = viewTechType });
            }
        }

        simEntities.Dispose();
    }
}