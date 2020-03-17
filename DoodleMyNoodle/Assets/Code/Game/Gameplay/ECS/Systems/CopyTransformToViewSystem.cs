using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;

[UpdateAfter(typeof(ViewBindingSystem))]
public class CopyTransformToViewSystem : ViewJobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        jobHandle = new CopyRotationJob()
        {
            SimValues = SimWorldAccessor.GetComponentDataFromEntity<Rotation>()
        }.Schedule(this, jobHandle);

        jobHandle = new CopyTranslationJob()
        {
            SimValues = SimWorldAccessor.GetComponentDataFromEntity<Translation>()
        }.Schedule(this, jobHandle);

        return jobHandle;
    }

    struct CopyTranslationJob : IJobForEach<Translation, BindedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<Translation> SimValues;

        public void Execute(ref Translation translation, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (SimValues.Exists(linkedSimEntity.SimWorldEntity))
            {
                translation.Value = SimValues[linkedSimEntity.SimWorldEntity].Value;
            }
        }
    }

    struct CopyRotationJob : IJobForEach<Rotation, BindedSimEntity>
    {
        [ReadOnly] public ComponentDataFromEntity<Rotation> SimValues;

        public void Execute(ref Rotation rotation, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (SimValues.Exists(linkedSimEntity.SimWorldEntity))
            {
                rotation.Value = SimValues[linkedSimEntity.SimWorldEntity].Value;
            }
        }
    }
}
