using SimulationControl;
using Unity.Collections;
using Unity.Entities;

public class ExternalSimWorldAccessor : SimWorldReadAccessor
{
    internal SubmitSimulationInputSystem SubmitSystem;

    public InputSubmissionId SubmitInput(SimInput simInput)
    {
        if (SubmitSystem != null)
            return SubmitSystem.SubmitInput(simInput);
        return InputSubmissionId.Invalid;
    }
}

//public struct SimWorldAccessorJob
//{
//    [ReadOnly] EntityManager _exclusiveTransaction;

//    public SimWorldAccessorJob(EntityManager exclusiveEntityTransaction)
//    {
//        _exclusiveTransaction = exclusiveEntityTransaction;
//    }

//    public bool Exists(Entity entity)
//        => _exclusiveTransaction.Exists(entity);

//    public bool HasComponent(Entity entity, ComponentType type)
//        => _exclusiveTransaction.HasComponent(entity, type);

//    public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
//        => _exclusiveTransaction.GetComponentData<T>(entity);

//    public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
//        => _exclusiveTransaction.GetSharedComponentData<T>(entity);

//    public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
//        => _exclusiveTransaction.GetBufferReadOnly<T>(entity);
//}