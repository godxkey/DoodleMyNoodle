using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;

public interface ISimWorldReadWriteAccessor : ISimWorldWriteAccessor, ISimWorldReadAccessor { }

public interface ISimWorldWriteAccessor
{
    // TODO add stuff here!
    // ----------------------------------------------------------------------------------------------------------
    // PUBLIC
    // ----------------------------------------------------------------------------------------------------------

    /// <summary>
    /// Creates an entity having the specified archetype.
    /// </summary>
    /// <remarks>
    /// The EntityManager creates the entity in the first available chunk with the matching archetype that has
    /// enough space.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating the entity and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="archetype">The archetype for the new entity.</param>
    /// <returns>The Entity object that you can use to access the entity.</returns>
    Entity CreateEntity(EntityArchetype archetype);

    /// <summary>
    /// Creates an entity having components of the specified types.
    /// </summary>
    /// <remarks>
    /// The EntityManager creates the entity in the first available chunk with the matching archetype that has
    /// enough space.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating the entity and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="types">The types of components to add to the new entity.</param>
    /// <returns>The Entity object that you can use to access the entity.</returns>
    Entity CreateEntity(params ComponentType[] types);

    Entity CreateEntity();

    /// <summary>
    /// Creates a set of entities of the specified archetype.
    /// </summary>
    /// <remarks>Fills the [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html)
    /// object assigned to the `entities` parameter with the Entity objects of the created entities. Each entity
    /// has the components specified by the <see cref="EntityArchetype"/> object assigned
    /// to the `archetype` parameter. The EntityManager adds these entities to the <see cref="World"/> entity list. Use the
    /// Entity objects in the array for further processing, such as setting the component values.</remarks>
    /// <param name="archetype">The archetype defining the structure for the new entities.</param>
    /// <param name="entities">An array to hold the Entity objects needed to access the new entities.
    /// The length of the array determines how many entities are created.</param>
    void CreateEntity(EntityArchetype archetype, NativeArray<Entity> entities);

    /// <summary>
    /// Creates a set of entities of the specified archetype.
    /// </summary>
    /// <remarks>Creates a [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html) of entities,
    /// each of which has the components specified by the <see cref="EntityArchetype"/> object assigned
    /// to the `archetype` parameter. The EntityManager adds these entities to the <see cref="World"/> entity list.</remarks>
    /// <param name="archetype">The archetype defining the structure for the new entities.</param>
    /// <param name="entityCount">The number of entities to create with the specified archetype.</param>
    /// <param name="allocator">How the created native array should be allocated.</param>
    /// <returns>
    /// A [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html) of entities
    /// with the given archetype.
    /// </returns>
    NativeArray<Entity> CreateEntity(EntityArchetype archetype, int entityCount, Allocator allocator);

    /// <summary>
    /// Destroy all entities having a common set of component types.
    /// </summary>
    /// <remarks>Since entities in the same chunk share the same component structure, this function effectively destroys
    /// the chunks holding any entities identified by the `entityQueryFilter` parameter.</remarks>
    /// <param name="entityQueryFilter">Defines the components an entity must have to qualify for destruction.</param>
    void DestroyEntity(EntityQuery entityQuery);

    /// <summary>
    /// Destroys all entities in an array.
    /// </summary>
    /// <remarks>
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before destroying the entity and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entities">An array containing the Entity objects of the entities to destroy.</param>
    void DestroyEntity(NativeArray<Entity> entities);

    /// <summary>
    /// Destroys all entities in a slice of an array.
    /// </summary>
    /// <remarks>
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before destroying the entity and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entities">The slice of an array containing the Entity objects of the entities to destroy.</param>
    void DestroyEntity(NativeSlice<Entity> entities);

    /// <summary>
    /// Destroys an entity.
    /// </summary>
    /// <remarks>
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before destroying the entity and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The Entity object of the entity to destroy.</param>
    void DestroyEntity(Entity entity);

    /// <summary>
    /// Clones an entity.
    /// </summary>
    /// <remarks>
    /// The new entity has the same archetype and component values as the original.
    ///
    /// If the source entity was converted from a prefab and thus has a <see cref="LinkedEntityGroup"/> component, 
    /// the entire group is cloned as a new set of entities.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating the entity and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="srcEntity">The entity to clone</param>
    /// <returns>The Entity object for the new entity.</returns>
    Entity Instantiate(Entity srcEntity);

    /// <summary>
    /// Makes multiple clones of an entity.
    /// </summary>
    /// <remarks>
    /// The new entities have the same archetype and component values as the original.
    ///
    /// If the source entity has a <see cref="LinkedEntityGroup"/> component, the entire group is cloned as a new
    /// set of entities.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating these entities and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="srcEntity">The entity to clone.</param>
    /// <param name="outputEntities">An array to receive the Entity objects of the root entity in each clone.
    /// The length of this array determines the number of clones.</param>
    void Instantiate(Entity srcEntity, NativeArray<Entity> outputEntities);

    /// <summary>
    /// Makes multiple clones of an entity.
    /// </summary>
    /// <remarks>
    /// The new entities have the same archetype and component values as the original.
    /// 
    /// If the source entity has a <see cref="LinkedEntityGroup"/> component, the entire group is cloned as a new
    /// set of entities.
    /// 
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating these entities and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="srcEntity">The entity to clone.</param>
    /// <param name="instanceCount">The number of entities to instantiate with the same components as the source entity.</param>
    /// <param name="allocator">How the created native array should be allocated.</param>
    /// <returns>A [NativeArray](https://docs.unity3d.com/ScriptReference/Unity.Collections.NativeArray_1.html) of entities.</returns>
    NativeArray<Entity> Instantiate(Entity srcEntity, int instanceCount, Allocator allocator);

    /// <summary>
    /// Adds a component to an entity.
    /// </summary>
    /// <remarks>
    /// Adding a component changes the entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// The added component has the default values for the type.
    ///
    /// If the <see cref="Entity"/> object refers to an entity that has been destroyed, this function throws an ArgumentError
    /// exception.
    ///
    /// If the <see cref="Entity"/> object refers to an entity that already has the specified <see cref="ComponentType"/>,
    /// the function returns false without performing any modifications.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The Entity object.</param>
    /// <param name="componentType">The type of component to add.</param>
    bool AddComponent(Entity entity, ComponentType componentType);

    /// <summary>
    /// Adds a component to an entity.
    /// </summary>
    /// <remarks>
    /// Adding a component changes the entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// The added component has the default values for the type.
    ///
    /// If the <see cref="Entity"/> object refers to an entity that has been destroyed, this function throws an ArgumentError
    /// exception.
    ///
    /// If the <see cref="Entity"/> object refers to an entity that already has the specified <see cref="ComponentType"/>
    /// of type T, the function returns false without performing any modifications.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The Entity object.</param>
    /// <typeparam name="T">The type of component to add.</typeparam>
    bool AddComponent<T>(Entity entity);

    /// <summary>
    /// Adds a component to a set of entities defined by a EntityQuery.
    /// </summary>
    /// <remarks>
    /// Adding a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// The added components have the default values for the type.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
    /// <param name="componentType">The type of component to add.</param>
    void AddComponent(EntityQuery entityQuery, ComponentType componentType);

    /// <summary>
    /// Adds a component to a set of entities defines by the EntityQuery and
    /// sets the component of each entity in the query to the value in the component array.
    /// componentArray.Length must match entityQuery.ToEntityArray().Length.
    /// </summary>
    /// <param name="entityQuery">THe EntityQuery defining the entities to add component to</param>
    /// <param name="componentArray"></param>
    void AddComponentData<T>(EntityQuery entityQuery, NativeArray<T> componentArray) where T : struct, IComponentData;

    /// <summary>
    /// Adds a component to a set of entities defined by a EntityQuery.
    /// </summary>
    /// <remarks>
    /// Adding a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// The added components have the default values for the type.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
    /// <typeparam name="T">The type of component to add.</typeparam>
    void AddComponent<T>(EntityQuery entityQuery);

    /// <summary>
    /// Adds a component to a set of entities.
    /// </summary>
    /// <remarks>
    /// Adding a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// The added components have the default values for the type.
    ///
    /// If an <see cref="Entity"/> object in the `entities` array refers to an entity that has been destroyed, this function
    /// throws an ArgumentError exception.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating these chunks and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entities">An array of Entity objects.</param>
    /// <param name="componentType">The type of component to add.</param>
    void AddComponent(NativeArray<Entity> entities, ComponentType componentType);

    /// <summary>
    /// Remove a component from a set of entities.
    /// </summary>
    /// <remarks>
    /// Removing a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// If an <see cref="Entity"/> object in the `entities` array refers to an entity that has been destroyed, this function
    /// throws an ArgumentError exception.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating these chunks and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entities">An array of Entity objects.</param>
    /// <param name="componentType">The type of component to remove.</param>
    void RemoveComponent(NativeArray<Entity> entities, ComponentType componentType);

    /// <summary>
    /// Adds a component to a set of entities.
    /// </summary>
    /// <remarks>
    /// Adding a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// The added components have the default values for the type.
    ///
    /// If an <see cref="Entity"/> object in the `entities` array refers to an entity that has been destroyed, this function
    /// throws an ArgumentError exception.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before creating these chunks and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entities">An array of Entity objects.</param>
    /// <typeparam name="T">The type of component to add.</typeparam>
    void AddComponent<T>(NativeArray<Entity> entities);

    /// <summary>
    /// Adds a set of component to an entity.
    /// </summary>
    /// <remarks>
    /// Adding components changes the entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// The added components have the default values for the type.
    ///
    /// If the <see cref="Entity"/> object refers to an entity that has been destroyed, this function throws an ArgumentError
    /// exception.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding these components and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity to modify.</param>
    /// <param name="types">The types of components to add.</param>
    void AddComponents(Entity entity, ComponentTypes types);

    /// <summary>
    /// Removes a component from an entity. Returns false if the entity did not have the component.
    /// </summary>
    /// <remarks>
    /// Removing a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity to modify.</param>
    /// <param name="componentType">The type of component to remove.</param>
    bool RemoveComponent(Entity entity, ComponentType componentType);

    /// <summary>
    /// Removes a component from a set of entities defined by a EntityQuery.
    /// </summary>
    /// <remarks>
    /// Removing a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
    /// <param name="componentType">The type of component to remove.</param>
    void RemoveComponent(EntityQuery entityQuery, ComponentType componentType);

    /// <summary>
    /// Removes a set of components from a set of entities defined by a EntityQuery.
    /// </summary>
    /// <remarks>
    /// Removing a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
    /// <param name="types">The types of components to add.</param>
    void RemoveComponent(EntityQuery entityQuery, ComponentTypes types);

    /// <summary>
    /// Removes a component from an entity. Returns false if the entity did not have the component.
    /// </summary>
    /// <remarks>
    /// Removing a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of component to remove.</typeparam>
    bool RemoveComponent<T>(Entity entity);

    /// <summary>
    /// Removes a component from a set of entities defined by a EntityQuery.
    /// </summary>
    /// <remarks>
    /// Removing a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery defining the entities to modify.</param>
    /// <typeparam name="T">The type of component to remove.</typeparam>
    void RemoveComponent<T>(EntityQuery entityQuery);

    /// <summary>
    /// Removes a component from a set of entities.
    /// </summary>
    /// <remarks>
    /// Removing a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entities">An array identifying the entities to modify.</param>
    /// <typeparam name="T">The type of component to remove.</typeparam>
    void RemoveComponent<T>(NativeArray<Entity> entities);

    /// <summary>
    /// Adds a component to an entity and set the value of that component. Returns true if the component was added,
    /// false if the entity already had the component. (The component's data is set either way.)
    /// </summary>
    /// <remarks>
    /// Adding a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk. 
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <param name="componentData">The data to set.</param>
    /// <typeparam name="T">The type of component.</typeparam>
    /// <returns></returns>
    bool AddComponentData<T>(Entity entity, T componentData) where T : struct, IComponentData;

    /// <summary>
    /// Removes a chunk component from the specified entity. Returns false if the entity did not have the component.
    /// </summary>
    /// <remarks>
    /// A chunk component is common to all entities in a chunk. Removing the chunk component from an entity changes
    /// that entity's archetype and results in the entity being moved to a different chunk (that does not have the
    /// removed component).
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of component to remove.</typeparam>
    bool RemoveChunkComponent<T>(Entity entity);

    /// <summary>
    /// Adds a chunk component to the specified entity. Returns true if the chunk component was added, false if the
    /// entity already had the chunk component. (The chunk component's data is set either way.)
    /// </summary>
    /// <remarks>
    /// Adding a chunk component to an entity changes that entity's archetype and results in the entity being moved
    /// to a different chunk, either one that already has an archetype containing the chunk component or a new
    /// chunk.
    ///
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk. In either case, getting
    /// or setting the component reads or writes the same data.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of component, which must implement IComponentData.</typeparam>
    bool AddChunkComponentData<T>(Entity entity) where T : struct, IComponentData;

    /// <summary>
    /// Adds a component to each of the chunks identified by a EntityQuery and set the component values.
    /// </summary>
    /// <remarks>
    /// This function finds all chunks whose archetype satisfies the EntityQuery and adds the specified
    /// component to them.
    ///
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery identifying the chunks to modify.</param>
    /// <param name="componentData">The data to set.</param>
    /// <typeparam name="T">The type of component, which must implement IComponentData.</typeparam>
    void AddChunkComponentData<T>(EntityQuery entityQuery, T componentData) where T : unmanaged, IComponentData;

    /// <summary>
    /// Removes a component from the chunks identified by a EntityQuery.
    /// </summary>
    /// <remarks>
    /// A chunk component is common to all entities in a chunk. You can access a chunk <see cref="IComponentData"/>
    /// instance through either the chunk itself or through an entity stored in that chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before removing the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery identifying the chunks to modify.</param>
    /// <typeparam name="T">The type of component to remove.</typeparam>
    void RemoveChunkComponentData<T>(EntityQuery entityQuery);

    /// <summary>
    /// Adds a dynamic buffer component to an entity.
    /// </summary>
    /// <remarks>
    /// A buffer component stores the number of elements inside the chunk defined by the [InternalBufferCapacity]
    /// attribute applied to the buffer element type declaration. Any additional elements are stored in a separate memory
    /// block that is managed by the EntityManager.
    ///
    /// Adding a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the buffer and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of buffer element. Must implement IBufferElementData.</typeparam>
    /// <returns>The buffer.</returns>
    /// <seealso cref="InternalBufferCapacityAttribute"/>
    DynamicBuffer<T> AddBuffer<T>(Entity entity) where T : struct, IBufferElementData;

    /// <summary>
    /// Gets the dynamic buffer of an entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <typeparam name="T">The type of the buffer's elements.</typeparam>
    /// <returns>The DynamicBuffer object for accessing the buffer contents.</returns>
    /// <exception cref="ArgumentException">Thrown if T is an unsupported type.</exception>
    DynamicBuffer<T> GetBuffer<T>(Entity entity) where T : struct, IBufferElementData;

    /// <summary>
    /// Adds a managed [UnityEngine.Component](https://docs.unity3d.com/ScriptReference/Component.html)
    /// object to an entity.
    /// </summary>
    /// <remarks>
    /// Accessing data in a managed object forfeits many opportunities for increased performance. Adding
    /// managed objects to an entity should be avoided or used sparingly.
    ///
    /// Adding a component changes an entity's archetype and results in the entity being moved to a different
    /// chunk.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the object and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity to modify.</param>
    /// <param name="componentData">An object inheriting UnityEngine.Component.</param>
    /// <exception cref="ArgumentNullException">If the componentData object is not an instance of
    /// UnityEngine.Component.</exception>
    void AddComponentObject(Entity entity, object componentData);

    /// <summary>
    /// Adds a shared component to an entity. Returns true if the shared component was added, false if the entity
    /// already had the shared component. (The shared component's data is set either way.)
    /// </summary>
    /// <remarks>
    /// The fields of the `componentData` parameter are assigned to the added shared component.
    ///
    /// Adding a component to an entity changes its archetype and results in the entity being moved to a
    /// different chunk. The entity moves to a chunk with other entities that have the same shared component values.
    /// A new chunk is created if no chunk with the same archetype and shared component values currently exists.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entity">The entity.</param>
    /// <param name="componentData">An instance of the shared component having the values to set.</param>
    /// <typeparam name="T">The shared component type.</typeparam>
    bool AddSharedComponentData<T>(Entity entity, T componentData) where T : struct, ISharedComponentData;

    /// <summary>
    /// Adds a shared component to a set of entities defined by a EntityQuery.
    /// </summary>
    /// <remarks>
    /// The fields of the `componentData` parameter are assigned to all of the added shared components.
    ///
    /// Adding a component to an entity changes its archetype and results in the entity being moved to a
    /// different chunk. The entity moves to a chunk with other entities that have the same shared component values.
    /// A new chunk is created if no chunk with the same archetype and shared component values currently exists.
    ///
    /// **Important:** This function creates a sync point, which means that the EntityManager waits for all
    /// currently running Jobs to complete before adding the component and no additional Jobs can start before
    /// the function is finished. A sync point can cause a drop in performance because the ECS framework may not
    /// be able to make use of the processing power of all available cores.
    /// </remarks>
    /// <param name="entityQuery">The EntityQuery defining a set of entities to modify.</param>
    /// <param name="componentData">The data to set.</param>
    /// <typeparam name="T">The data type of the shared component.</typeparam>
    void AddSharedComponentData<T>(EntityQuery entityQuery, T componentData) where T : struct, ISharedComponentData;

    void SetArchetype(Entity entity, EntityArchetype archetype);

    /// <summary>
    /// Enabled entities are processed by systems, disabled entities are not.
    /// Adds or removes the <see cref="Disabled"/> component. By default EntityQuery does not include entities containing the Disabled component.
    ///
    /// If the entity was converted from a prefab and thus has a <see cref="LinkedEntityGroup"/> component, the entire group will enabled or disabled.
    /// </summary>
    /// <param name="entity">The entity to enable or disable</param>
    /// <param name="enabled">True if the entity should be enabled</param>
    void SetEnabled(Entity entity, bool enabled);

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

    /// <summary>
    /// Sets the value of a singleton component.
    /// </summary>
    /// <param name="value">A component containing the value to assign to the singleton.</param>
    /// <typeparam name="T">The <see cref="IComponentData"/> subtype of the singleton component.</typeparam>
    /// <seealso cref="EntityQuery.SetSingleton{T}"/>
    void SetSingleton<T>(T value) where T : struct, IComponentData;

#if UNITY_EDITOR
    void SetName(Entity entity, string name);
#endif

    T GetOrCreateSystem<T>() where T : ComponentSystemBase;
}

public interface ISimWorldReadAccessor
{
    string Name { get; }
    int Version { get; }

    bool IsCreated { get; }

    ulong SequenceNumber { get; }
    ref FixTimeData Time { get; }

    uint EntityClearAndReplaceCount { get; }
    event Action OnEntityClearedAndReplaced;
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
    DynamicBuffer<T> GetBufferReadOnly<T>(Entity entity) where T : struct, IBufferElementData;

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

#if UNITY_EDITOR
    /// <summary>
    /// Gets the name assigned to an entity.
    /// </summary>
    /// <remarks>For performance, entity names only exist when running in the Unity Editor.</remarks>
    /// <param name="entity">The Entity object of the entity of interest.</param>
    /// <returns>The entity name.</returns>
    string GetName(Entity entity);
#endif
}
