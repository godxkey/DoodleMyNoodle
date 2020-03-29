using SimulationControl;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;

public class ExternalSimWorldAccessor : SimWorldReadAccessor
{
    internal SubmitSimulationInputSystem SubmitSystem;
    internal BeginViewSystem BeginViewSystem;

    public InputSubmissionId SubmitInput(SimInput simInput)
    {
        if (SubmitSystem != null)
            return SubmitSystem.SubmitInput(simInput);
        return InputSubmissionId.Invalid;
    }

    public SimWorldAccessorJob JobAccessor => new SimWorldAccessorJob(BeginViewSystem.ExclusiveSimWorld);
}

public struct SimWorldAccessorJob
{
    [ReadOnly] ExclusiveEntityTransaction _exclusiveTransaction;

    public SimWorldAccessorJob(ExclusiveEntityTransaction exclusiveEntityTransaction)
    {
        _exclusiveTransaction = exclusiveEntityTransaction;
    }

    public bool Exists(Entity entity)
        => _exclusiveTransaction.Exists(entity);

    public bool HasComponent(Entity entity, ComponentType type)
        => _exclusiveTransaction.HasComponent(entity, type);

    public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        => _exclusiveTransaction.GetComponentData<T>(entity);

    public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        => _exclusiveTransaction.GetSharedComponentData<T>(entity);

    public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        => _exclusiveTransaction.GetBuffer<T>(entity);
}