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
        jobHandle = new CopyRotationJob()
        {
            SimValues = SimWorldAccessor.GetComponentDataFromEntity<FixRotation>()
        }.Schedule(this, jobHandle);

        jobHandle = new CopyTranslationJob()
        {
            SimValues = SimWorldAccessor.GetComponentDataFromEntity<FixTranslation>()
        }.Schedule(this, jobHandle);

        return jobHandle;
    }

    struct CopyTranslationJob : IJobForEach<Translation, BindedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<FixTranslation> SimValues;

        public void Execute(ref Translation translation, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (SimValues.Exists(linkedSimEntity.SimEntity))
            {
                translation.Value = SimValues[linkedSimEntity.SimEntity].Value.ToUnityVec();
            }
        }
    }

    struct CopyRotationJob : IJobForEach<Rotation, BindedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<FixRotation> SimValues;

        public void Execute(ref Rotation rotation, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (SimValues.Exists(linkedSimEntity.SimEntity))
            {
                rotation.Value = SimValues[linkedSimEntity.SimEntity].Value.ToUnityQuat();
            }
        }
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