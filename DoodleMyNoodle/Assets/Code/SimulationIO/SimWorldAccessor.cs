using SimulationControl;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Core;
using Unity.Entities;

public class SimWorldAccessor
{
    internal SimulationWorld SimWorld;
    internal SubmitSimulationInputSystem SubmitSystem;
    internal BeginViewSystem BeginViewSystem;
    internal SimPreInitializationSystemGroup SomeSimSystem;

    public InputSubmissionId SubmitInput(SimInput simInput)
    {
        if (SubmitSystem != null)
            return SubmitSystem.SubmitInput(simInput);
        return InputSubmissionId.Invalid;
    }

    public SimWorldAccessorJob JobAccessor => new SimWorldAccessorJob(BeginViewSystem.ExclusiveSimWorld);

    public string Name
        => SimWorld.Name;

    public override string ToString()
    {
        return $"Accessor({SimWorld.ToString()})";
    }

    public int Version
        => SimWorld.Version;

    public bool IsCreated =>
        SimWorld.IsCreated;

    public ulong SequenceNumber =>
        SimWorld.SequenceNumber;

    public ref TimeData Time =>
        ref SimWorld.Time;

    public uint EntityClearAndReplaceCount
        => SimWorld.EntityClearAndReplaceCount;

    // fbessette: 
    //  Here we are giving the presentation access to a query builder in the simulation.
    //  Potential down sides:
    //      - This will cache ALL of our presentation-to-sim queries in one system, making the lookup potentially
    //      slower
    public EntityQueryBuilder Entities => SomeSimSystem.QueryBuilder;

    /// <summary>
    /// Gets an array-like container containing all components of type T, indexed by Entity.
    /// </summary>
    /// <param name="isReadOnly">Whether the data is only read, not written. Access data as
    /// read-only whenever possible.</param>
    /// <typeparam name="T">A struct that implements <see cref="IComponentData"/>.</typeparam>
    /// <returns>All component data of type T.</returns>
    public ComponentDataFromEntity<T> GetComponentDataFromEntity<T>() where T : struct, IComponentData
        => SomeSimSystem.GetComponentDataFromEntity<T>(true);

    /// <summary>
    /// Gets a BufferFromEntity&lt;T&gt; object that can access a <seealso cref="DynamicBuffer{T}"/>.
    /// </summary>
    /// <remarks>Assign the returned object to a field of your Job struct so that you can access the
    /// contents of the buffer in a Job.</remarks>
    /// <param name="isReadOnly">Whether the buffer data is only read or is also written. Access data in
    /// a read-only fashion whenever possible.</param>
    /// <typeparam name="T">The type of <see cref="IBufferElementData"/> stored in the buffer.</typeparam>
    /// <returns>An array-like object that provides access to buffers, indexed by <see cref="Entity"/>.</returns>
    /// <seealso cref="ComponentDataFromEntity{T}"/>
    public BufferFromEntity<T> GetBufferFromEntity<T>() where T : struct, IBufferElementData
        => SomeSimSystem.GetBufferFromEntity<T>(true);

    /// <summary>
    /// Gets the value of a singleton component.
    /// </summary>
    /// <typeparam name="T">The <see cref="IComponentData"/> subtype of the singleton component.</typeparam>
    /// <returns>The component.</returns>
    /// <seealso cref="EntityQuery.GetSingleton{T}"/>
    public T GetSingleton<T>() where T : struct, IComponentData
        => SomeSimSystem.GetSingleton<T>();

    /// <summary>
    /// Checks whether a singelton component of the specified type exists.
    /// </summary>
    /// <typeparam name="T">The <see cref="IComponentData"/> subtype of the singleton component.</typeparam>
    /// <returns>True, if a singleton of the specified type exists in the current <see cref="World"/>.</returns>
    public bool HasSingleton<T>() where T : struct, IComponentData
        => SomeSimSystem.HasSingleton<T>();

    /// <summary>
    /// Gets the Entity instance for a singleton.
    /// </summary>
    /// <typeparam name="T">The Type of the singleton component.</typeparam>
    /// <returns>The entity associated with the specified singleton component.</returns>
    /// <seealso cref="EntityQuery.GetSingletonEntity"/>
    public Entity GetSingletonEntity<T>()
        => SomeSimSystem.GetSingletonEntity<T>();

    /// <summary>
    /// Creates a EntityQuery from an array of component types.
    /// </summary>
    /// <param name="requiredComponents">An array containing the component types.</param>
    /// <returns>The EntityQuery derived from the specified array of component types.</returns>
    /// <seealso cref="EntityQueryDesc"/>
    public EntityQuery CreateEntityQuery(params ComponentType[] requiredComponents)
        => SimWorld.EntityManager.CreateEntityQuery(requiredComponents);

    /// <summary>
    /// Gets the number of shared components managed by this EntityManager.
    /// </summary>
    /// <returns>The shared component count</returns>
    public int GetSharedComponentCount()
        => SimWorld.EntityManager.GetSharedComponentCount();

    /// <summary>
    /// Gets the value of a component for an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of component to retrieve.</typeparam>
    /// <returns>A struct of type T containing the component value.</returns>
    /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
    public T GetComponentData<T>(Entity entity) where T : struct, IComponentData
        => SimWorld.EntityManager.GetComponentData<T>(entity);

    /// <summary>
    /// Reports whether an Entity object is still valid.
    /// </summary>
    /// <remarks>
    /// An Entity object does not contain a reference to its entity. Instead, the Entity struct contains an index
    /// and a generational version number. When an entity is destroyed, the EntityManager increments the version
    /// of the entity within the internal array of entities. The index of a destroyed entity is recycled when a
    /// new entity is created.
    ///
    /// After an entity is destroyed, any existing Entity objects will still contain the
    /// older version number. This function compares the version numbers of the specified Entity object and the
    /// current version of the entity recorded in the entities array. If the versions are different, the Entity
    /// object no longer refers to an existing entity and cannot be used.
    /// </remarks>
    /// <param name="entity">The Entity object to check.</param>
    /// <returns>True, if <see cref="Entity.Version"/> matches the version of the current entity at
    /// <see cref="Entity.Index"/> in the entities array.</returns>
    public bool Exists(Entity entity)
        => SimWorld.EntityManager.Exists(entity);

    /// <summary>
    /// Checks whether an entity has a specific type of component.
    /// </summary>
    /// <remarks>Always returns false for an entity that has been destroyed.</remarks>
    /// <param name="entity">The Entity object.</param>
    /// <typeparam name="T">The data type of the component.</typeparam>
    /// <returns>True, if the specified entity has the component.</returns>
    public bool HasComponent<T>(Entity entity)
        => SimWorld.EntityManager.HasComponent<T>(entity);

    /// <summary>
    /// Checks whether an entity has a specific type of component.
    /// </summary>
    /// <remarks>Always returns false for an entity that has been destroyed.</remarks>
    /// <param name="entity">The Entity object.</param>
    /// <param name="type">The data type of the component.</param>
    /// <returns>True, if the specified entity has the component.</returns>
    public bool HasComponent(Entity entity, ComponentType type)
        => SimWorld.EntityManager.HasComponent(entity, type);

    /// <summary>
    /// Checks whether the chunk containing an entity has a specific type of component.
    /// </summary>
    /// <remarks>Always returns false for an entity that has been destroyed.</remarks>
    /// <param name="entity">The Entity object.</param>
    /// <typeparam name="T">The data type of the chunk component.</typeparam>
    /// <returns>True, if the chunk containing the specified entity has the component.</returns>
    public bool HasChunkComponent<T>(Entity entity)
        => SimWorld.EntityManager.HasChunkComponent<T>(entity);

    /// <summary>
    /// Gets the value of a chunk component.
    /// </summary>
    /// <remarks>
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk.
    /// </remarks>
    /// <param name="chunk">The chunk.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>A struct of type T containing the component value.</returns>
    /// <exception cref="ArgumentException">Thrown if the ArchetypeChunk object is invalid.</exception>
    public T GetChunkComponentData<T>(ArchetypeChunk chunk) where T : struct, IComponentData
        => SimWorld.EntityManager.GetChunkComponentData<T>(chunk);

    /// <summary>
    /// Gets the value of chunk component for the chunk containing the specified entity.
    /// </summary>
    /// <remarks>
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <returns>A struct of type T containing the component value.</returns>
    public T GetChunkComponentData<T>(Entity entity) where T : struct, IComponentData
        => SimWorld.EntityManager.GetChunkComponentData<T>(entity);

    /// <summary>
    /// Gets the managed [UnityEngine.Component](https://docs.unity3d.com/ScriptReference/Component.html) object
    /// from an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of the managed object.</typeparam>
    /// <returns>The managed object, cast to type T.</returns>
    public T GetComponentObject<T>(Entity entity)
        => SimWorld.EntityManager.GetComponentObject<T>(entity);

    public T GetComponentObject<T>(Entity entity, ComponentType componentType)
        => SimWorld.EntityManager.GetComponentObject<T>(entity, componentType);

    /// <summary>
    /// Gets a shared component from an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of shared component.</typeparam>
    /// <returns>A copy of the shared component.</returns>
    public T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData
        => SimWorld.EntityManager.GetSharedComponentData<T>(entity);

    public int GetSharedComponentDataIndex<T>(Entity entity) where T : struct, ISharedComponentData
        => SimWorld.EntityManager.GetSharedComponentDataIndex<T>(entity);

    /// <summary>
    /// Gets a shared component by index.
    /// </summary>
    /// <remarks>
    /// The ECS framework maintains an internal list of unique shared components. You can get the components in this
    /// list, along with their indices using
    /// <see cref="GetAllUniqueSharedComponentData{T}(List{T},List{int})"/>. An
    /// index in the list is valid and points to the same shared component index as long as the shared component
    /// order version from <see cref="GetSharedComponentOrderVersion{T}(T)"/> remains the same.
    /// </remarks>
    /// <param name="sharedComponentIndex">The index of the shared component in the internal shared component
    /// list.</param>
    /// <typeparam name="T">The data type of the shared component.</typeparam>
    /// <returns>A copy of the shared component.</returns>
    public T GetSharedComponentData<T>(int sharedComponentIndex) where T : struct, ISharedComponentData
        => SimWorld.EntityManager.GetSharedComponentData<T>(sharedComponentIndex);

    /// <summary>
    /// Gets a list of all the unique instances of a shared component type.
    /// </summary>
    /// <remarks>
    /// All entities with the same archetype and the same values for a shared component are stored in the same set
    /// of chunks. This function finds the unique shared components existing across chunks and archetype and
    /// fills a list with copies of those components.
    /// </remarks>
    /// <param name="sharedComponentValues">A List<T> object to receive the unique instances of the
    /// shared component of type T.</param>
    /// <typeparam name="T">The type of shared component.</typeparam>
    public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues) where T : struct, ISharedComponentData
        => SimWorld.EntityManager.GetAllUniqueSharedComponentData<T>(sharedComponentValues);

    /// <summary>
    /// Gets a list of all unique shared components of the same type and a corresponding list of indices into the
    /// internal shared component list.
    /// </summary>
    /// <remarks>
    /// All entities with the same archetype and the same values for a shared component are stored in the same set
    /// of chunks. This function finds the unique shared components existing across chunks and archetype and
    /// fills a list with copies of those components and fills in a separate list with the indices of those components
    /// in the internal shared component list. You can use the indices to ask the same shared components directly
    /// by calling <see cref="GetSharedComponentData{T}(int)"/>, passing in the index. An index remains valid until
    /// the shared component order version changes. Check this version using
    /// <see cref="GetSharedComponentOrderVersion{T}(T)"/>.
    /// </remarks>
    /// <param name="sharedComponentValues"></param>
    /// <param name="sharedComponentIndices"></param>
    /// <typeparam name="T"></typeparam>
    public void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues, List<int> sharedComponentIndices) where T : struct, ISharedComponentData
        => SimWorld.EntityManager.GetAllUniqueSharedComponentData<T>(sharedComponentValues, sharedComponentIndices);

    /// <summary>
    /// Gets the dynamic buffer of an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of the buffer's elements.</typeparam>
    /// <returns>The DynamicBuffer object for accessing the buffer contents.</returns>
    /// <exception cref="ArgumentException">Thrown if T is an unsupported type.</exception>
    public DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData
        => SimWorld.EntityManager.GetBuffer<T>(entity);

    /// <summary>
    /// Gets the chunk in which the specified entity is stored.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The chunk containing the entity.</returns>
    public ArchetypeChunk GetChunk(Entity entity)
        => SimWorld.EntityManager.GetChunk(entity);

    /// <summary>
    /// Gets the number of component types associated with an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The number of components.</returns>
    public int GetComponentCount(Entity entity)
        => SimWorld.EntityManager.GetComponentCount(entity);

    /// <summary>
    /// Returns false if the component has the 'Disabled' component. Disabled entities are excluded from entity queries by default
    /// </summary>
    public bool GetEnabled(Entity entity)
        => SimWorld.EntityManager.GetEnabled(entity);
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

public static class SimWorldAccessorExtensions
{
    public static bool TryGetComponentData<T>(this SimWorldAccessor accessor, Entity entity, out T componentData)
         where T : struct, IComponentData
    {
        if (accessor.HasComponent<T>(entity))
        {
            componentData = accessor.GetComponentData<T>(entity);
            return true;
        }

        componentData = default;
        return false;
    }
}