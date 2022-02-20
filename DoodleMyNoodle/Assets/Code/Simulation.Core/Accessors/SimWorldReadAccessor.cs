using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;

public class SimWorldReadAccessor : ISimWorldReadAccessor
{
    internal SimulationWorld SimWorld;
    public EntityManager EntityManager;
    internal SimPreInitializationSystemGroup SomeSimSystem;

    protected SimulationWorld GetSimWorld() => SimWorld;

    public bool IsWorldCreated => SimWorld != null && SimWorld.IsCreated;

    public string Name
        => SimWorld.Name;

    public override string ToString()
    {
        return $"Accessor({SimWorld})";
    }

    public int Version
        => SimWorld.Version;

    public uint GlobalSystemVersion
        => SimWorld.EntityManager.GlobalSystemVersion;

    public bool IsCreated
        => SimWorld.IsCreated;

    public ulong SequenceNumber
        => SimWorld.SequenceNumber;

    public ref FixTimeData Time => ref SimWorld.FixTime;

    public uint ReplaceVersion
        => SimWorld.Owner.ReplaceVersion;

    public uint ExpectedNewTickId
        => SimWorld.ExpectedNewTickId;

    public event Action WorldReplaced
    {
        add => SimWorld.Owner.WorldReplaced += value;
        remove => SimWorld.Owner.WorldReplaced -= value;
    }

    // fbessette: 
    //  Here we are giving the presentation access to a query builder in the simulation.
    //  Potential down sides:
    //      - This will cache ALL of our presentation-to-sim queries in one system, making the lookup potentially
    //      slower
    public EntityQueryBuilder Entities => SomeSimSystem.QueryBuilder;
    public ForEachLambdaJobDescriptionJCS EntitiesJob
        => throw new System.NotImplementedException("EntitiesJob is not yet supported. Unity's codegen makes it difficult" +
            " to implement. Use typical struct job declaration instead.");

    public int EntityCapacity => EntityManager.EntityCapacity;

    public SimInput[] TickInputs => SimWorld.TickInputs;

    public ComponentDataFromEntity<T> GetComponentDataFromEntity<T>() where T : struct, IComponentData
        => SomeSimSystem.GetComponentDataFromEntity<T>(true);

    public BufferFromEntity<T> GetBufferFromEntity<T>() where T : struct, IBufferElementData
        => SomeSimSystem.GetBufferFromEntity<T>(true);

    public T GetSingleton<T>() where T : struct, IComponentData
        => SomeSimSystem.GetSingleton<T>();

    public bool HasSingleton<T>() where T : struct, IComponentData
        => SomeSimSystem.HasSingleton<T>();

    public Entity GetSingletonEntity<T>()
        => SomeSimSystem.GetSingletonEntity<T>();

    public EntityQuery CreateEntityQuery(params ComponentType[] requiredComponents)
        => EntityManager.CreateEntityQuery(requiredComponents);

    public int GetSharedComponentCount()
        => EntityManager.GetSharedComponentCount();

    public T GetComponent<T>(Entity entity) where T : struct, IComponentData
        => EntityManager.GetComponentData<T>(entity);

    public bool Exists(Entity entity)
        => EntityManager.Exists(entity);

    public bool HasComponent<T>(Entity entity)
        => EntityManager.HasComponent<T>(entity);

    public bool HasComponent(Entity entity, ComponentType type)
        => EntityManager.HasComponent(entity, type);

    public bool HasChunkComponent<T>(Entity entity)
        => EntityManager.HasChunkComponent<T>(entity);

    public T GetChunkComponentData<T>(ArchetypeChunk chunk) where T : struct, IComponentData
        => EntityManager.GetChunkComponentData<T>(chunk);

    public T GetChunkComponentData<T>(Entity entity) where T : struct, IComponentData
        => EntityManager.GetChunkComponentData<T>(entity);

    public T GetComponentObject<T>(Entity entity)
        => EntityManager.GetComponentObject<T>(entity);

    public T GetComponentObject<T>(Entity entity, ComponentType componentType)
        => EntityManager.GetComponentObject<T>(entity, componentType);

    public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        => EntityManager.GetSharedComponentData<T>(entity);

    public int GetSharedComponentDataIndex<T>(Entity entity) where T : struct, ISharedComponentData
        => EntityManager.GetSharedComponentDataIndex<T>(entity);

    public T GetSharedComponentData<T>(int sharedComponentIndex) where T : struct, ISharedComponentData
        => EntityManager.GetSharedComponentData<T>(sharedComponentIndex);

    public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues) where T : struct, ISharedComponentData
        => EntityManager.GetAllUniqueSharedComponentData<T>(sharedComponentValues);

    public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues, List<int> sharedComponentIndices) where T : struct, ISharedComponentData
        => EntityManager.GetAllUniqueSharedComponentData<T>(sharedComponentValues, sharedComponentIndices);

    public DynamicBuffer<T> GetBufferReadOnly<T>(Entity entity) where T : struct, IBufferElementData
        => EntityManager.GetBufferReadOnly<T>(entity);

    public ArchetypeChunk GetChunk(Entity entity)
        => EntityManager.GetChunk(entity);

    public int GetComponentCount(Entity entity)
        => EntityManager.GetComponentCount(entity);

    public bool GetEnabled(Entity entity)
        => EntityManager.GetEnabled(entity);

    public ComponentDataFromEntity<T> GetComponentFromEntity<T>() where T : struct, IComponentData
        => SomeSimSystem.GetComponentDataFromEntity<T>(isReadOnly: true);

#if UNITY_EDITOR
    public string GetName(Entity entity)
        => EntityManager.GetName(entity);
#endif

    public T GetExistingSystem<T>() where T : ComponentSystemBase
    {
        return SimWorld.GetExistingSystem<T>();
    }

    public DynamicBuffer<T> GetSingletonBufferReadOnly<T>() where T : struct, ISingletonBufferElementData
    {
        return EntityManager.GetBufferReadOnly<T>(GetSingletonEntity<SingletonBuffersTag>());
    }
}
