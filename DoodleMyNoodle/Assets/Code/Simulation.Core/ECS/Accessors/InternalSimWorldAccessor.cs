using Unity.Entities;

public class InternalSimWorldAccessor : SimWorldReadAccessor, ISimWorldReadWriteAccessor
{
    void ISimWorldWriteAccessor.SetChunkComponentData<T>(ArchetypeChunk chunk, T componentValue)
        => EntityManager.SetChunkComponentData(chunk, componentValue);

    void ISimWorldWriteAccessor.SetComponentData<T>(Entity entity, T componentData)
        => EntityManager.SetComponentData(entity, componentData);

    void ISimWorldWriteAccessor.SetSharedComponentData<T>(Entity entity, T componentData)
        => EntityManager.SetSharedComponentData(entity, componentData);

    void ISimWorldWriteAccessor.SetSharedComponentData<T>(EntityQuery query, T componentData)
        => EntityManager.SetSharedComponentData(query, componentData);

    void ISimWorldWriteAccessor.SwapComponents(ArchetypeChunk leftChunk, int leftIndex, ArchetypeChunk rightChunk, int rightIndex)
        => EntityManager.SwapComponents(leftChunk, leftIndex, rightChunk, rightIndex);
}
