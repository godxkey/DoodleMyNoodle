using Unity.Collections;
using Unity.Entities;

public class InternalSimWorldAccessor : SimWorldReadAccessor, ISimWorldReadWriteAccessor
{
    DynamicBuffer<T> ISimWorldWriteAccessor.AddBuffer<T>(Entity entity)
        => EntityManager.AddBuffer<T>(entity);

    bool ISimWorldWriteAccessor.AddChunkComponentData<T>(Entity entity)
        => EntityManager.AddChunkComponentData<T>(entity);

    void ISimWorldWriteAccessor.AddChunkComponentData<T>(EntityQuery entityQuery, T componentData)
        => EntityManager.AddChunkComponentData<T>(entityQuery, componentData);

    bool ISimWorldWriteAccessor.AddComponent(Entity entity, ComponentType componentType)
        => EntityManager.AddComponent(entity, componentType);

    bool ISimWorldWriteAccessor.AddComponent<T>(Entity entity)
        => EntityManager.AddComponent<T>(entity);

    void ISimWorldWriteAccessor.AddComponent(EntityQuery entityQuery, ComponentType componentType)
        => EntityManager.AddComponent(entityQuery, componentType);

    void ISimWorldWriteAccessor.AddComponent<T>(EntityQuery entityQuery)
        => EntityManager.AddComponent<T>(entityQuery);

    void ISimWorldWriteAccessor.AddComponent(NativeArray<Entity> entities, ComponentType componentType)
        => EntityManager.AddComponent(entities, componentType);

    void ISimWorldWriteAccessor.AddComponent<T>(NativeArray<Entity> entities)
        => EntityManager.AddComponent<T>(entities);

    void ISimWorldWriteAccessor.AddComponentData<T>(EntityQuery entityQuery, NativeArray<T> componentArray)
        => EntityManager.AddComponentData<T>(entityQuery, componentArray);

    bool ISimWorldWriteAccessor.AddComponentData<T>(Entity entity, T componentData)
        => EntityManager.AddComponentData<T>(entity, componentData);

    void ISimWorldWriteAccessor.AddComponentObject(Entity entity, object componentData)
        => EntityManager.AddComponentObject(entity, componentData);

    void ISimWorldWriteAccessor.AddComponents(Entity entity, ComponentTypes types)
        => EntityManager.AddComponents(entity, types);

    bool ISimWorldWriteAccessor.AddSharedComponentData<T>(Entity entity, T componentData)
        => EntityManager.AddSharedComponentData<T>(entity, componentData);

    void ISimWorldWriteAccessor.AddSharedComponentData<T>(EntityQuery entityQuery, T componentData)
        => EntityManager.AddSharedComponentData<T>(entityQuery, componentData);

    void ISimWorldWriteAccessor.CreateChunk(EntityArchetype archetype, NativeArray<ArchetypeChunk> chunks, int entityCount)
        => EntityManager.CreateChunk(archetype, chunks, entityCount);

    Entity ISimWorldWriteAccessor.CreateEntity(EntityArchetype archetype)
        => EntityManager.CreateEntity(archetype);

    Entity ISimWorldWriteAccessor.CreateEntity(params ComponentType[] types)
        => EntityManager.CreateEntity(types);

    Entity ISimWorldWriteAccessor.CreateEntity()
        => EntityManager.CreateEntity();

    void ISimWorldWriteAccessor.CreateEntity(EntityArchetype archetype, NativeArray<Entity> entities)
        => EntityManager.CreateEntity(archetype, entities);

    NativeArray<Entity> ISimWorldWriteAccessor.CreateEntity(EntityArchetype archetype, int entityCount, Allocator allocator)
        => EntityManager.CreateEntity(archetype, entityCount, allocator);

    void ISimWorldWriteAccessor.DestroyEntity(EntityQuery entityQuery)
        => EntityManager.DestroyEntity(entityQuery);

    void ISimWorldWriteAccessor.DestroyEntity(NativeArray<Entity> entities)
        => EntityManager.DestroyEntity(entities);

    void ISimWorldWriteAccessor.DestroyEntity(NativeSlice<Entity> entities)
        => EntityManager.DestroyEntity(entities);

    void ISimWorldWriteAccessor.DestroyEntity(Entity entity)
        => EntityManager.DestroyEntity(entity);

    Entity ISimWorldWriteAccessor.Instantiate(Entity srcEntity)
        => EntityManager.Instantiate(srcEntity);

    void ISimWorldWriteAccessor.Instantiate(Entity srcEntity, NativeArray<Entity> outputEntities)
        => EntityManager.Instantiate(srcEntity, outputEntities);

    NativeArray<Entity> ISimWorldWriteAccessor.Instantiate(Entity srcEntity, int instanceCount, Allocator allocator)
        => EntityManager.Instantiate(srcEntity, instanceCount, allocator);

    bool ISimWorldWriteAccessor.RemoveChunkComponent<T>(Entity entity)
        => EntityManager.RemoveChunkComponent<T>(entity);

    void ISimWorldWriteAccessor.RemoveChunkComponentData<T>(EntityQuery entityQuery)
        => EntityManager.RemoveChunkComponentData<T>(entityQuery);

    void ISimWorldWriteAccessor.RemoveComponent(NativeArray<Entity> entities, ComponentType componentType)
        => EntityManager.RemoveComponent(entities, componentType);

    bool ISimWorldWriteAccessor.RemoveComponent(Entity entity, ComponentType componentType)
        => EntityManager.RemoveComponent(entity, componentType);

    void ISimWorldWriteAccessor.RemoveComponent(EntityQuery entityQuery, ComponentType componentType)
        => EntityManager.RemoveComponent(entityQuery, componentType);

    void ISimWorldWriteAccessor.RemoveComponent(EntityQuery entityQuery, ComponentTypes types)
        => EntityManager.RemoveComponent(entityQuery, types);

    bool ISimWorldWriteAccessor.RemoveComponent<T>(Entity entity)
        => EntityManager.RemoveComponent<T>(entity);

    void ISimWorldWriteAccessor.RemoveComponent<T>(EntityQuery entityQuery)
        => EntityManager.RemoveComponent<T>(entityQuery);

    void ISimWorldWriteAccessor.RemoveComponent<T>(NativeArray<Entity> entities)
        => EntityManager.RemoveComponent<T>(entities);

    void ISimWorldWriteAccessor.SetArchetype(Entity entity, EntityArchetype archetype)
        => EntityManager.SetArchetype(entity, archetype);

    void ISimWorldWriteAccessor.SetChunkComponentData<T>(ArchetypeChunk chunk, T componentValue)
        => EntityManager.SetChunkComponentData(chunk, componentValue);

    void ISimWorldWriteAccessor.SetComponentData<T>(Entity entity, T componentData)
        => EntityManager.SetComponentData(entity, componentData);

    void ISimWorldWriteAccessor.SetEnabled(Entity entity, bool enabled)
        => EntityManager.SetEnabled(entity, enabled);

    void ISimWorldWriteAccessor.SetName(Entity entity, string name)
        => EntityManager.SetName(entity, name);

    void ISimWorldWriteAccessor.SetSharedComponentData<T>(Entity entity, T componentData)
        => EntityManager.SetSharedComponentData(entity, componentData);

    void ISimWorldWriteAccessor.SetSharedComponentData<T>(EntityQuery query, T componentData)
        => EntityManager.SetSharedComponentData(query, componentData);

    void ISimWorldWriteAccessor.SetSingleton<T>(T value)
        => SomeSimSystem.SetSingleton(value);

    void ISimWorldWriteAccessor.SwapComponents(ArchetypeChunk leftChunk, int leftIndex, ArchetypeChunk rightChunk, int rightIndex)
        => EntityManager.SwapComponents(leftChunk, leftIndex, rightChunk, rightIndex);

}