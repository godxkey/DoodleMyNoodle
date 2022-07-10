﻿using System;
using System.Diagnostics;
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
public partial class MaintainBindedViewEntitiesSystem : ViewSystemBase
{
    private EntityQuery _updatedSimEntitiesQ;

    private EntityArchetype _viewArchetypeTile;
    private EntityArchetype _viewArchetypeGameObject;

    private DirtyValue<uint> _simWorldReplaceVersion;
    private DirtyValue<uint> _simWorldTickId;

    private BindedViewEntityCommandBufferSystem _ecb;

    private NativeParallelHashMap<Entity, Entity> _sim2ViewMap;

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

        _sim2ViewMap = new NativeParallelHashMap<Entity, Entity>(1024, Allocator.Persistent);
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
            _updatedSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<SimAssetId>());
            _updatedSimEntitiesQ.SetChangedVersionFilter(ComponentType.ReadOnly<SimAssetId>());

            // Destroy all view
            DestroyAllViewEntities();

        }
        else
        {
            // Destroy dangling view
            DestroyDanglingViewEntities();
        }

        // update map
        UpdateSim2ViewMap();

        // destroy then create view for entities that had their SimAssetId modified
        UpdateViewEntities(SimAssetBankInstance.GetJobLookup(), _updatedSimEntitiesQ);

        _updatedSimEntitiesQ.SetChangedFilterRequiredVersion(SimWorldAccessor.GlobalSystemVersion);

        _ecb.AddJobHandleForProducer(Dependency);
    }

    private void UpdateSim2ViewMap()
    {
        NativeParallelHashMap<Entity, Entity> sim2ViewMap = _sim2ViewMap;
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
            .ForEach((Entity viewEntity, in BindedSimEntity simEntity) =>
            {
                if (!simEntities.HasComponent(simEntity))
                {
                    ecb.DestroyEntity(viewEntity);
                }
            }).Schedule();
    }

    private void UpdateViewEntities(SimAssetBank.JobLookup simAssetBank, EntityQuery simEntityQuery)
    {
        if (simEntityQuery.IsEmpty)
            return;

        EntityCommandBuffer ecb = _ecb.CreateCommandBuffer();
        NativeArray<Entity> simEntities = simEntityQuery.ToEntityArrayAsync(Allocator.TempJob, out JobHandle job1);
        NativeArray<SimAssetId> simAssetIds = simEntityQuery.ToComponentDataArrayAsync<SimAssetId>(Allocator.TempJob, out JobHandle job2);
        EntityArchetype viewArchetypeTile = _viewArchetypeTile;
        EntityArchetype viewArchetypeGameObject = _viewArchetypeGameObject;
        NativeParallelHashMap<Entity, Entity> sim2ViewMap = _sim2ViewMap;
        ComponentDataFromEntity<SimAssetId> viewSimAssetIds = GetComponentDataFromEntity<SimAssetId>(isReadOnly: true);

        var jobHandle = Job.WithCode(() =>
        {
            for (int i = 0; i < simEntities.Length; i++)
            {
                SimAssetId viewAssetId = SimAssetId.Invalid;
                SimAssetId simAssetId = simAssetIds[i];
                Entity simEntity = simEntities[i];

                if (sim2ViewMap.TryGetValue(simEntity, out Entity viewEntity))
                {
                    if (viewSimAssetIds.HasComponent(viewEntity))
                    {
                        viewAssetId = viewSimAssetIds[viewEntity];
                    }
                }

                if (viewAssetId != simAssetId)
                {
                    if (viewEntity != Entity.Null)
                        ecb.DestroyEntity(viewEntity);
                    TryCreateViewEntity(simAssetBank, ecb, simEntities[i], simAssetId, viewArchetypeTile, viewArchetypeGameObject);
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
            ecb.SetComponent(viewEntity, (BindedSimEntity)simEntity);
            ecb.SetComponent(viewEntity, simAssetId);
        }
    }
}