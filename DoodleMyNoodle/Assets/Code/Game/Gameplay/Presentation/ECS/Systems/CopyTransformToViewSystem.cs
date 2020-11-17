using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Jobs;

[UpdateAfter(typeof(MaintainBindedViewEntitiesSystem))]
public class CopyTransformToViewSystem : ViewJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        ////////////////////////////////////////////////////////////////////////////////////////
        //      Copy Rotation
        ////////////////////////////////////////////////////////////////////////////////////////
        ComponentDataFromEntity<FixRotation> simRotations = SimWorldAccessor.GetComponentDataFromEntity<FixRotation>();

        var copyRotJobHandle = Entities
            .WithReadOnly(simRotations)
            .ForEach((ref Rotation rotation, in BindedSimEntity linkedSimEntity)=>
        {
            if (simRotations.HasComponent(linkedSimEntity.SimEntity))
            {
                rotation.Value = simRotations[linkedSimEntity.SimEntity].Value.ToUnityQuat();
            }
        }).Schedule(jobHandle);


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Copy Translation
        ////////////////////////////////////////////////////////////////////////////////////////
        ComponentDataFromEntity<FixTranslation> simTranslations = SimWorldAccessor.GetComponentDataFromEntity<FixTranslation>();
        
        var copyTrJobHandle = Entities
            .WithReadOnly(simTranslations)
            .ForEach((ref Translation translation, in BindedSimEntity linkedSimEntity) =>
        {
            if (simTranslations.HasComponent(linkedSimEntity.SimEntity))
            {
                translation.Value = simTranslations[linkedSimEntity.SimEntity].Value.ToUnityVec();
            }
        }).Schedule(jobHandle);

        return JobHandle.CombineDependencies(copyRotJobHandle, copyTrJobHandle);
    }
}



[UpdateAfter(typeof(MaintainBindedViewGameObjectsSystem))]
public class CopyTransformToViewGameObjectSystem : ViewSystemBase
{
    [BurstCompile]
    struct CopyTransforms : IJobParallelForTransform
    {
        [ReadOnly] 
        public ComponentDataFromEntity<FixTranslation> SimTranslations;
        [ReadOnly] 
        public ComponentDataFromEntity<FixRotation> SimRotations;

        [DeallocateOnJobCompletion]
        [ReadOnly] public NativeArray<BindedSimEntity> BindedSimEntities;

        public void Execute(int index, TransformAccess transform)
        {
            var simEntity = BindedSimEntities[index].SimEntity;

            if (SimTranslations.HasComponent(simEntity))
            {
                transform.localPosition = SimTranslations[simEntity].Value.ToUnityVec();
            }

            if (SimRotations.HasComponent(simEntity))
            {
                transform.localRotation = SimRotations[simEntity].Value.ToUnityQuat();
            }
        }
    }

    EntityQuery _viewTransformsQ;

    protected override void OnCreate()
    {
        base.OnCreate();
        _viewTransformsQ = GetEntityQuery(
            ComponentType.ReadOnly<BindedSimEntity>(),
            ComponentType.ReadOnly<BindedGameObjectTag>(),
            ComponentType.ReadWrite<Transform>());
    }

    protected override void OnUpdate()
    {
        TransformAccessArray transforms = _viewTransformsQ.GetTransformAccessArray();
        
        Dependency = new CopyTransforms
        {
            SimTranslations = SimWorldAccessor.GetComponentDataFromEntity<FixTranslation>(),
            SimRotations = SimWorldAccessor.GetComponentDataFromEntity<FixRotation>(),
            BindedSimEntities = _viewTransformsQ.ToComponentDataArrayAsync<BindedSimEntity>(Allocator.TempJob, out JobHandle dep),
        }.Schedule(transforms, dep);
    }
}