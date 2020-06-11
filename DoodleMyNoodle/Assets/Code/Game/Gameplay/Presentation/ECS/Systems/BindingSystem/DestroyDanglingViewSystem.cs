using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[AlwaysUpdateSystem]
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
        EntityCommandBuffer.Concurrent Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent();

        // If sim world was cleared and replaced
        _simWorldEntityClearAndReplaceCount.Set(SimWorldAccessor.EntityClearAndReplaceCount);
        if (_simWorldEntityClearAndReplaceCount.IsDirty)
        {
            _simWorldEntityClearAndReplaceCount.Reset();

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Destroy all binded view entities
            ////////////////////////////////////////////////////////////////////////////////////////
            jobHandle = Entities.WithAll<BindedSimEntity>()
                .ForEach((Entity viewEntity, int entityInQueryIndex) =>
            {
                Ecb.DestroyEntity(entityInQueryIndex, viewEntity);
            }).Schedule(jobHandle);
        }
        else
        {
            var simEntities = SimWorldAccessor.GetComponentDataFromEntity<SimAssetId>();

            ////////////////////////////////////////////////////////////////////////////////////////
            //      Destroy view entities binded with non-existant sim entities
            ////////////////////////////////////////////////////////////////////////////////////////
            jobHandle = Entities.WithReadOnly(simEntities)
                .ForEach((Entity viewEntity, int entityInQueryIndex, in BindedSimEntity linkedSimEntity) =>
            {
                if (!simEntities.Exists(linkedSimEntity.SimEntity))
                {
                    Ecb.DestroyEntity(entityInQueryIndex, viewEntity);
                }
            }).Schedule(jobHandle);
        }


        _ecbSystem.AddJobHandleForProducer(jobHandle);

        return jobHandle;
    }
}