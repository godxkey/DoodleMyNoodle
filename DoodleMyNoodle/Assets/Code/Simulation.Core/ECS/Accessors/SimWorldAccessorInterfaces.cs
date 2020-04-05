using System.Collections.Generic;
using Unity.Entities;

public interface ISimWorldReadWriteAccessor : ISimWorldWriteAccessor, ISimWorldReadAccessor { }

public interface ISimWorldWriteAccessor
{
    // TODO add stuff here!

    /// <summary>
    /// Sets the value of a component of an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="componentData">The data to set.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
    void SetComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData;

    /// <summary>
    /// Sets the value of a chunk component.
    /// </summary>
    /// <remarks>
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk.
    /// </remarks>
    /// <param name="chunk">The chunk to modify.</param>
    /// <param name="componentValue">The component data to set.</param>
    /// <typeparam name="T">The component type.</typeparam>
    /// <exception cref="ArgumentException">Thrown if the ArchetypeChunk object is invalid.</exception>
    void SetChunkComponentData<T>(ArchetypeChunk chunk, T componentValue) where T : struct, IComponentData;

    /// <summary>
    /// Sets the shared component of an entity.
    /// </summary>
    /// <remarks>
    /// Changing a shared component value of an entity results in the entity being moved to a
    /// different chunk. The entity moves to a chunk with other entities that have the same shared component values.
    /// A new chunk is created if no chunk with the same archetype and shared component values currently exists.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before setting the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity</param>
    /// <param name="componentData">A shared component object containing the values to set.</param>
    /// <typeparam name="T">The shared component type.</typeparam>
    void SetSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData;

    /// <summary>
    /// Sets the shared component of all entities in the query.
    /// </summary>
    /// <remarks>
    /// The component data stays in the same chunk, the internal shared component data indices will be adjusted.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before setting the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity</param>
    /// <param name="componentData">A shared component object containing the values to set.</param>
    /// <typeparam name="T">The shared component type.</typeparam>
    void SetSharedComponentData<T>(EntityQuery query, T componentData) where T : struct, ISharedComponentData;

    /// <summary>
    /// Swaps the components of two entities.
    /// </summary>
    /// <remarks>
    /// The entities must have the same components. However, this function can swap the components of entities in
    /// different worlds, so they do not need to have identical archetype instances.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before swapping the components and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="leftChunk">A chunk containing one of the entities to swap.</param>
    /// <param name="leftIndex">The index within the `leftChunk` of the entity and components to swap.</param>
    /// <param name="rightChunk">The chunk containing the other entity to swap. This chunk can be the same as
    /// the `leftChunk`. It also does not need to be in the same World as `leftChunk`.</param>
    /// <param name="rightIndex">The index within the `rightChunk`  of the entity and components to swap.</param>
    void SwapComponents(ArchetypeChunk leftChunk, int leftIndex, ArchetypeChunk rightChunk, int rightIndex);
}

public interface ISimWorldReadAccessor
{
    string Name { get; }
    int Version { get; }

    bool IsCreated { get; }

    ulong SequenceNumber { get; }
    ref FixTimeData Time { get; }

    uint EntityClearAndReplaceCount { get; }
    SimInput[] TickInputs { get; }


    // fbessette: 
    //  Here we are giving the presentation access to a query builder in the simulation.
    //  Potential down sides:
    //      - This will cache ALL of our presentation-to-sim queries in one system, making the lookup potentially
    //      slower
    EntityQueryBuilder Entities { get; }

    /// <summary>
    /// Gets an array-like container containing all components of type T, indexed by Entity.
    /// </summary>
    /// <param name="isReadOnly">Whether the data is only read, not written. Access data as
    /// read-only whenever possible.</param>
    /// <typeparam name="T">A struct that implements <see cref="IComponentData"/>.</typeparam>
    /// <returns>All component data of type T.</returns>
    ComponentDataFromEntity<T> GetComponentDataFromEntity<T>() where T : struct, IComponentData;

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
    BufferFromEntity<T> GetBufferFromEntity<T>() where T : struct, IBufferElementData;

    /// <summary>
    /// Gets the value of a singleton component.
    /// </summary>
    /// <typeparam name="T">The <see cref="IComponentData"/> subtype of the singleton component.</typeparam>
    /// <returns>The component.</returns>
    /// <seealso cref="EntityQuery.GetSingleton{T}"/>
    T GetSingleton<T>() where T : struct, IComponentData;

    /// <summary>
    /// Checks whether a singelton component of the specified type exists.
    /// </summary>
    /// <typeparam name="T">The <see cref="IComponentData"/> subtype of the singleton component.</typeparam>
    /// <returns>True, if a singleton of the specified type exists in the current <see cref="World"/>.</returns>
    bool HasSingleton<T>() where T : struct, IComponentData;

    /// <summary>
    /// Gets the Entity instance for a singleton.
    /// </summary>
    /// <typeparam name="T">The Type of the singleton component.</typeparam>
    /// <returns>The entity associated with the specified singleton component.</returns>
    /// <seealso cref="EntityQuery.GetSingletonEntity"/>
    Entity GetSingletonEntity<T>();

    /// <summary>
    /// Creates a EntityQuery from an array of component types.
    /// </summary>
    /// <param name="requiredComponents">An array containing the component types.</param>
    /// <returns>The EntityQuery derived from the specified array of component types.</returns>
    /// <seealso cref="EntityQueryDesc"/>
    EntityQuery CreateEntityQuery(params ComponentType[] requiredComponents);

    /// <summary>
    /// Gets the number of shared components managed by this EntityManager.
    /// </summary>
    /// <returns>The shared component count</returns>
    int GetSharedComponentCount();

    /// <summary>
    /// Gets the value of a component for an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of component to retrieve.</typeparam>
    /// <returns>A struct of type T containing the component value.</returns>
    /// <exception cref="ArgumentException">Thrown if the component type has no fields.</exception>
    T GetComponentData<T>(Entity entity) where T : struct, IComponentData;

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
    bool Exists(Entity entity);

    /// <summary>
    /// Checks whether an entity has a specific type of component.
    /// </summary>
    /// <remarks>Always returns false for an entity that has been destroyed.</remarks>
    /// <param name="entity">The Entity object.</param>
    /// <typeparam name="T">The data type of the component.</typeparam>
    /// <returns>True, if the specified entity has the component.</returns>
    bool HasComponent<T>(Entity entity);

    /// <summary>
    /// Checks whether an entity has a specific type of component.
    /// </summary>
    /// <remarks>Always returns false for an entity that has been destroyed.</remarks>
    /// <param name="entity">The Entity object.</param>
    /// <param name="type">The data type of the component.</param>
    /// <returns>True, if the specified entity has the component.</returns>
    bool HasComponent(Entity entity, ComponentType type);

    /// <summary>
    /// Checks whether the chunk containing an entity has a specific type of component.
    /// </summary>
    /// <remarks>Always returns false for an entity that has been destroyed.</remarks>
    /// <param name="entity">The Entity object.</param>
    /// <typeparam name="T">The data type of the chunk component.</typeparam>
    /// <returns>True, if the chunk containing the specified entity has the component.</returns>
    bool HasChunkComponent<T>(Entity entity);

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
    T GetChunkComponentData<T>(ArchetypeChunk chunk) where T : struct, IComponentData;

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
    T GetChunkComponentData<T>(Entity entity) where T : struct, IComponentData;

    /// <summary>
    /// Gets the managed [UnityEngine.Component](https://docs.unity3d.com/ScriptReference/Component.html) object
    /// from an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of the managed object.</typeparam>
    /// <returns>The managed object, cast to type T.</returns>
    T GetComponentObject<T>(Entity entity);

    T GetComponentObject<T>(Entity entity, ComponentType componentType);

    /// <summary>
    /// Gets a shared component from an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of shared component.</typeparam>
    /// <returns>A copy of the shared component.</returns>
    T GetSharedComponentData<T>(Entity entity) where T : struct, ISharedComponentData;

    int GetSharedComponentDataIndex<T>(Entity entity) where T : struct, ISharedComponentData;

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
    T GetSharedComponentData<T>(int sharedComponentIndex) where T : struct, ISharedComponentData;

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
    void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues) where T : struct, ISharedComponentData;

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
    void GetAllUniqueSharedComponentData<T>(List<T> sharedComponentValues, List<int> sharedComponentIndices) where T : struct, ISharedComponentData;

    /// <summary>
    /// Gets the dynamic buffer of an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of the buffer's elements.</typeparam>
    /// <returns>The DynamicBuffer object for accessing the buffer contents.</returns>
    /// <exception cref="ArgumentException">Thrown if T is an unsupported type.</exception>
    DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData;

    /// <summary>
    /// Gets the chunk in which the specified entity is stored.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The chunk containing the entity.</returns>
    ArchetypeChunk GetChunk(Entity entity);

    /// <summary>
    /// Gets the number of component types associated with an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <returns>The number of components.</returns>
    int GetComponentCount(Entity entity);

    /// <summary>
    /// Returns false if the component has the 'Disabled' component. Disabled entities are excluded from entity queries by default
    /// </summary>
    bool GetEnabled(Entity entity);
}
