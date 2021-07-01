using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngineX;

public struct BindedViewType_GameObject : IComponentData { }
public struct BindedViewType_Tile : IComponentData { }

public class BindedViewEntityCommandBufferSystem : ViewEntityCommandBufferSystem { }

[UpdateBefore(typeof(BindedViewEntityCommandBufferSystem))]
[AlwaysUpdateSystem]
public class MaintainBindedViewEntitiesSystem : ViewSystemBase
{
    private EntityQuery _newSimEntitiesQ;
    private EntityQuery _allSimEntitiesQ;
    private EntityQuery _updatedSimEntitiesQ;

    private EntityArchetype _viewArchetypeTile;
    private EntityArchetype _viewArchetypeGameObject;

    private DirtyValue<uint> _simWorldReplaceVersion;
    private DirtyValue<uint> _simWorldTickId;

    private BindedViewEntityCommandBufferSystem _ecb;

    private NativeHashMap<Entity, Entity> _sim2ViewMap;

    protected override void OnCreate()
    {
        base.OnCreate();
        _ecb = World.GetOrCreateSystem<BindedViewEntityCommandBufferSystem>();
        _viewArchetypeTile = EntityManager.CreateArchetype(
            typeof(BindedSimEntity),
            typeof(SimAssetId),
            typeof(BindedViewType_Tile));
        _viewArchetypeGameObject = EntityManager.CreateArchetype(
            typeof(BindedSimEntity),
            typeof(SimAssetId),
            typeof(BindedViewType_GameObject));

        _sim2ViewMap = new NativeHashMap<Entity, Entity>(1024, Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _sim2ViewMap.Dispose();
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
            _updatedSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<SimAssetId>(), ComponentType.ReadOnly<MidLifeCycleTag>());
            _updatedSimEntitiesQ.SetChangedVersionFilter(ComponentType.ReadOnly<SimAssetId>());

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

        UpdateSim2ViewMap();

        // destroy then create view for entities that had their SimAssetId modified
        DestroyAndCreateViewEntitiesForModifiedSimAssetIds(SimAssetBankInstance.GetJobLookup(), _updatedSimEntitiesQ);

        _updatedSimEntitiesQ.SetChangedFilterRequiredVersion(SimWorldAccessor.GlobalSystemVersion);

        _ecb.AddJobHandleForProducer(Dependency);
    }

    private void UpdateSim2ViewMap()
    {
        NativeHashMap<Entity, Entity> sim2ViewMap = _sim2ViewMap;
        Job.WithCode(() =>
        {
            sim2ViewMap.Clear();
        }).Schedule();

        Entities.ForEach((Entity viewEntity, in BindedSimEntity simEntity) =>
        {
            sim2ViewMap[simEntity] = viewEntity;
        }).Schedule();
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
        Entities
            .WithReadOnly(simEntities)
            .ForEach((Entity viewEntity, int entityInQueryIndex, in BindedSimEntity linkedSimEntity) =>
            {
                if (!simEntities.HasComponent(linkedSimEntity.SimEntity))
                {
                    ecb.DestroyEntity(viewEntity);
                }
            }).Schedule();
    }

    private void CreateNewViewEntities(SimAssetBank.JobLookup simAssetBank, EntityQuery simEntityQuery)
    {
        if (simEntityQuery.IsEmpty)
            return;

        EntityCommandBuffer ecb = _ecb.CreateCommandBuffer();
        NativeArray<Entity> simEntities = simEntityQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle job1);
        NativeArray<SimAssetId> simAssetIds = simEntityQuery.ToComponentDataArrayAsync<SimAssetId>(Allocator.TempJob, out JobHandle job2);
        EntityArchetype viewArchetypeTile = _viewArchetypeTile;
        EntityArchetype viewArchetypeGameObject = _viewArchetypeGameObject;

        var jobHandle = Job.WithCode(() =>
        {
            for (int i = 0; i < simEntities.Length; i++)
            {
                TryCreateViewEntity(simAssetBank, ecb, simEntities[i], simAssetIds[i], viewArchetypeTile, viewArchetypeGameObject);
            }
        })
            .WithDisposeOnCompletion(simAssetIds)
            .WithDisposeOnCompletion(simEntities)
            .Schedule(JobHandle.CombineDependencies(Dependency, job1, job2));

        Dependency = jobHandle;
    }

    private void DestroyAndCreateViewEntitiesForModifiedSimAssetIds(SimAssetBank.JobLookup simAssetBank, EntityQuery simEntityQuery)
    {
        if (simEntityQuery.IsEmpty)
            return;

        EntityCommandBuffer ecb = _ecb.CreateCommandBuffer();
        NativeArray<Entity> simEntities = simEntityQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle job1);
        NativeArray<SimAssetId> simAssetIds = simEntityQuery.ToComponentDataArrayAsync<SimAssetId>(Allocator.TempJob, out JobHandle job2);
        EntityArchetype viewArchetypeTile = _viewArchetypeTile;
        EntityArchetype viewArchetypeGameObject = _viewArchetypeGameObject;
        NativeHashMap<Entity, Entity> sim2ViewMap = _sim2ViewMap;
        ComponentDataFromEntity<SimAssetId> viewSimAssetIds = GetComponentDataFromEntity<SimAssetId>(isReadOnly: true);

        var jobHandle = Job.WithCode(() =>
        {
            for (int i = 0; i < simEntities.Length; i++)
            {
                Entity simEntity = simEntities[i];
                if (sim2ViewMap.TryGetValue(simEntity, out Entity viewEntity))
                {
                    SimAssetId simAssetId = simAssetIds[i];

                    if (viewSimAssetIds.HasComponent(viewEntity))
                    {
                        var viewAssetId = viewSimAssetIds[viewEntity];
                        if (viewAssetId != simAssetId)
                        {
                            ecb.DestroyEntity(viewEntity);
                            TryCreateViewEntity(simAssetBank, ecb, simEntities[i], simAssetId, viewArchetypeTile, viewArchetypeGameObject);
                        }
                    }
                }

            }
        })
            .WithReadOnly(sim2ViewMap)
            .WithReadOnly(viewSimAssetIds)
            .WithDisposeOnCompletion(simAssetIds)
            .WithDisposeOnCompletion(simEntities)
            .Schedule(JobHandle.CombineDependencies(Dependency, job1, job2));

        Dependency = jobHandle;
    }

    private static void TryCreateViewEntity(SimAssetBank.JobLookup simAssetBank, EntityCommandBuffer ecb, Entity simEntity, SimAssetId simAssetId, EntityArchetype tileArchetype, EntityArchetype gameObjectArchetype)
    {
        if (simAssetBank.TryGetViewTechType(simAssetId, out ViewTechType viewTechType))
        {
            var archetype = viewTechType == ViewTechType.Tile ? tileArchetype : gameObjectArchetype;
            Entity viewEntity = ecb.CreateEntity(archetype);
            ecb.SetComponent(viewEntity, new BindedSimEntity() { SimEntity = simEntity });
            ecb.SetComponent(viewEntity, simAssetId);
        }
    }
}