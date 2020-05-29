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

[UpdateAfter(typeof(CreateBindedViewEntitiesSystem))]
public class CopyTransformToViewSystem : ViewJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        var simTranslations = SimWorldAccessor.GetComponentDataFromEntity<FixTranslation>();
        var simRotations = SimWorldAccessor.GetComponentDataFromEntity<FixRotation>();

        var copyRotJobHandle = Entities.ForEach((ref Rotation rotation, in BindedSimEntity linkedSimEntity)=>
        {
            if (simRotations.Exists(linkedSimEntity.SimEntity))
            {
                rotation.Value = simRotations[linkedSimEntity.SimEntity].Value.ToUnityQuat();
            }
        }).Schedule(jobHandle);
        
        var copyTrJobHandle = Entities.ForEach((ref Translation translation, in BindedSimEntity linkedSimEntity) =>
        {
            if (simTranslations.Exists(linkedSimEntity.SimEntity))
            {
                translation.Value = simTranslations[linkedSimEntity.SimEntity].Value.ToUnityVec();
            }
        }).Schedule(jobHandle);

        return JobHandle.CombineDependencies(copyRotJobHandle, copyTrJobHandle);
    }
}



[UpdateAfter(typeof(CreateBindedViewGameObjectsSystem))]
public class CopyTransformToViewGameObjectSystem : ViewJobComponentSystem
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

            if (SimTranslations.Exists(simEntity))
            {
                transform.localPosition = SimTranslations[simEntity].Value.ToUnityVec();
            }

            if (SimRotations.Exists(simEntity))
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
            ComponentType.ReadOnly<BindedGameObject>(),
            ComponentType.ReadWrite<Transform>());
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        TransformAccessArray transforms = _viewTransformsQ.GetTransformAccessArray();
        
        inputDeps = new CopyTransforms
        {
            SimTranslations = SimWorldAccessor.GetComponentDataFromEntity<FixTranslation>(),
            SimRotations = SimWorldAccessor.GetComponentDataFromEntity<FixRotation>(),
            BindedSimEntities = _viewTransformsQ.ToComponentDataArrayAsync<BindedSimEntity>(Allocator.TempJob, out inputDeps),
        }.Schedule(transforms, inputDeps);

        return inputDeps;
    }
}