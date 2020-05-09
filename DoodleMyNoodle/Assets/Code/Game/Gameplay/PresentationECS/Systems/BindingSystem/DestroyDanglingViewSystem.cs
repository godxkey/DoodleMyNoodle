using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class DestroyDanglingViewSystem : ViewJobComponentSystem
{
    private PostSimulationBindingCommandBufferSystem _ecbSystem;
    private DirtyValue<uint> _simWorldEntityClearAndReplaceCount;

    protected override void OnCreate()
    {
        base.OnCreate();
        _ecbSystem = World.GetOrCreateSystem<PostSimulationBindingCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        _simWorldEntityClearAndReplaceCount.Set(SimWorldAccessor.EntityClearAndReplaceCount);
        if (_simWorldEntityClearAndReplaceCount.IsDirty)
        {
            _simWorldEntityClearAndReplaceCount.Reset();

            jobHandle = new DestroyAllViewSystemJob()
            {
                Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, jobHandle);
        }
        else
        {
            jobHandle = new DestroyDanglingViewSystemJob()
            {
                SimWorld = SimWorldAccessor.JobAccessor,
                Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent()
            }.Schedule(this, jobHandle);
        }


        _ecbSystem.AddJobHandleForProducer(jobHandle);
        
        // Now that the job is set up, schedule it to be run. 
        return jobHandle;
    }

    struct DestroyDanglingViewSystemJob : IJobForEachWithEntity<BindedSimEntity>
    {
        public EntityCommandBuffer.Concurrent Ecb;
        public SimWorldAccessorJob SimWorld;

        public void Execute(Entity viewEntity, int jobIndex, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            if (!SimWorld.Exists(linkedSimEntity.SimEntity))
            {
                Ecb.DestroyEntity(jobIndex, viewEntity);
            }
        }
    }

    struct DestroyAllViewSystemJob : IJobForEachWithEntity<BindedSimEntity>
    {
        public EntityCommandBuffer.Concurrent Ecb;

        public void Execute(Entity viewEntity, int jobIndex, [ReadOnly] ref BindedSimEntity linkedSimEntity)
        {
            Ecb.DestroyEntity(jobIndex, viewEntity);
        }
    }
}