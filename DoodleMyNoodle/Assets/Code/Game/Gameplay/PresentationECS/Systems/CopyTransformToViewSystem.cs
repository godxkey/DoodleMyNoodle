using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

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
            if (SimValues.Exists(linkedSimEntity.SimWorldEntity))
            {
                translation.Value = SimValues[linkedSimEntity.SimWorldEntity].Value.ToUnityVec();
            }
        }
    }

    struct CopyRotationJob : IJobForEach<Rotation, BindedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<FixRotation> SimValues;

        public void Execute(ref Rotation rotation, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (SimValues.Exists(linkedSimEntity.SimWorldEntity))
            {
                rotation.Value = SimValues[linkedSimEntity.SimWorldEntity].Value.ToUnityQuat();
            }
        }
    }
}
