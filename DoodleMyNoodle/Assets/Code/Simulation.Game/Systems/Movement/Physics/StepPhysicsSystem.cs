/*using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Collections;
using System;
using static Unity.Mathematics.math;
using Unity.Jobs;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngineX;

public enum PhysicsLayer : byte
{
    None = 0,
    Terrain = 1 << 0,
    Unit = 1 << 1,
}

public struct PhysicsBody
{
    public Entity Entity;
    public Aabb Aabb;
    public PhysicsLayer BelongsTo;
    public PhysicsLayer CollidesWith;
    public PhysicsLayer TriggersWith;

    public struct MotionVelocity
    {
        public fix2 Velocity;
    }
}

public struct CollisionWorld : IDisposable
{
    public NativeSlice<PhysicsBody> AllBodies => new NativeSlice<PhysicsBody>(_physicsBodies, 0, BodyCount);
    public NativeSlice<PhysicsBody> StaticBodies => new NativeSlice<PhysicsBody>(_physicsBodies, DynamicBodyCount, StaticBodyCount);
    public NativeSlice<PhysicsBody> DynamicBodies => new NativeSlice<PhysicsBody>(_physicsBodies, 0, DynamicBodyCount);
    public NativeSlice<PhysicsBody.MotionVelocity> DynamicBodies => new NativeSlice<PhysicsBody>(_physicsBodies, 0, DynamicBodyCount);

    public int BodyCount => StaticBodyCount + DynamicBodyCount;
    public int StaticBodyCount { get; private set; }
    public int DynamicBodyCount { get; private set; }

    private NativeArray<PhysicsBody> _physicsBodies;

    public CollisionWorld(int staticBodyCount, int dynamicBodyCount)
    {
        StaticBodyCount = staticBodyCount;
        DynamicBodyCount = dynamicBodyCount;
        _physicsBodies = new NativeArray<PhysicsBody>(staticBodyCount + dynamicBodyCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
    }

    internal void Reset(int staticBodyCount, int dynamicBodyCount)
    {
        StaticBodyCount = staticBodyCount;
        DynamicBodyCount = dynamicBodyCount;
        SetCapacity(staticBodyCount + dynamicBodyCount);
    }

    private void SetCapacity(int bodyCount)
    {
        // Increase body storage if necessary
        if (_physicsBodies.Length < bodyCount)
        {
            _physicsBodies.Dispose();
            _physicsBodies = new NativeArray<PhysicsBody>(bodyCount, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        }
    }

    public void Dispose()
    {
        if (_physicsBodies.IsCreated)
        {
            _physicsBodies.Dispose();
        }
    }
}

public struct PhysicsWorld : IDisposable
{
    public CollisionWorld CollisionWorld;

    public int StaticBodyCount => CollisionWorld.StaticBodyCount;
    public int DynamicBodyCount => CollisionWorld.DynamicBodyCount;
    public int BodyCount => CollisionWorld.BodyCount;

    public NativeSlice<PhysicsBody> AllBodies => CollisionWorld.AllBodies;
    public NativeSlice<PhysicsBody> StaticBodies => CollisionWorld.StaticBodies;
    public NativeSlice<PhysicsBody> DynamicBodies => CollisionWorld.DynamicBodies;

    public PhysicsWorld(int staticBodyCount, int dynamicBodyCount)
    {
        CollisionWorld = new CollisionWorld(staticBodyCount, dynamicBodyCount);
    }

    public void Reset(int staticBodyCount, int dynamicBodyCount)
    {
        CollisionWorld.Reset(staticBodyCount: staticBodyCount, dynamicBodyCount: dynamicBodyCount);
    }

    public void Dispose()
    {
        CollisionWorld.Dispose();
    }
}

public class BuildPhysicsWorldSystem : SimJobComponentSystem
{
    public PhysicsWorld PhysicsWorld;

    // A look-up from an Entity to a Physics Body Index.
    public struct EntityToPhysicsBodyIndex : IDisposable
    {
        public NativeHashMap<Entity, int> Lookup;

        // Reset the look-up and increase its capacity if required.
        internal void Reset(int totalBodyCount)
        {
            totalBodyCount = max(1, totalBodyCount);

            if (Lookup.IsCreated)
            {
                Lookup.Clear();
                Lookup.Capacity = max(Lookup.Capacity, totalBodyCount);
            }
            else
            {
                Lookup = new NativeHashMap<Entity, int>(totalBodyCount, Allocator.Persistent);
            }
        }

        public void Dispose()
        {
            if (Lookup.IsCreated)
                Lookup.Dispose();
        }
    }

    public EntityToPhysicsBodyIndex EntityToPhysicsBody => m_EntityToPhysicsBody;
    private EntityToPhysicsBodyIndex m_EntityToPhysicsBody;

    public EntityQuery StaticEntityGroup { get; private set; }
    public EntityQuery DynamicEntityGroup { get; private set; }
    public JobHandle FinalJobHandle { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        PhysicsWorld = new PhysicsWorld(staticBodyCount: 0, dynamicBodyCount: 0);

        // Definition of a static body entity.
        StaticEntityGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<PhysicsCollider>(),
                ComponentType.ReadOnly<FixTranslation>()
            },
            None = new ComponentType[]
            {
                ComponentType.ReadOnly<Velocity>()
            }
        });

        // Definition of a dynamic body entity.
        DynamicEntityGroup = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[]
            {
                ComponentType.ReadOnly<PhysicsCollider>(),
                ComponentType.ReadOnly<FixTranslation>(),
                ComponentType.ReadOnly<Velocity>()
            }
        });

        // Create the Entity to Physics Body Lookup.
        m_EntityToPhysicsBody = new EntityToPhysicsBodyIndex();
        m_EntityToPhysicsBody.Reset(0);
    }

    protected override void OnDestroy()
    {
        PhysicsWorld.Dispose();
        m_EntityToPhysicsBody.Dispose();
        base.OnDestroy();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var entityType = GetArchetypeChunkEntityType();

        var translationType = GetArchetypeChunkComponentType<FixTranslation>(true);
        var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>(true);

        var staticBodyCount = StaticEntityGroup.CalculateEntityCount();
        var dynamicBodyCount = DynamicEntityGroup.CalculateEntityCount();

        // Ensure we have adequate world simulation capacity for the bodies.
        PhysicsWorld.Reset(
            staticBodyCount: staticBodyCount,
            dynamicBodyCount: dynamicBodyCount
            );

        // Reset the Entity to Body look-up.
        // NOTE: We don't need to add the static "ground" body here.
        m_EntityToPhysicsBody.Reset(dynamicBodyCount + staticBodyCount);

        using (var jobHandles = new NativeList<JobHandle>(2, Allocator.Temp))
        {
            // Create dynamic bodies.
            if (dynamicBodyCount > 0)
            {
                jobHandles.Add(
                    new CreatePhysicsBodiesJob
                    {
                        EntityType = entityType,
                        TranslationType = translationType,
                        ColliderType = physicsColliderType,

                        PhysicsBodies = PhysicsWorld.DynamicBodies,

                    }.Schedule(DynamicEntityGroup, inputDeps));
            }

            // Create static bodies.
            if (staticBodyCount > 0)
            {
                jobHandles.Add(
                    new CreatePhysicsBodiesJob
                    {
                        EntityType = entityType,
                        TranslationType = translationType,
                        ColliderType = physicsColliderType,

                        PhysicsBodies = PhysicsWorld.StaticBodies,

                    }.Schedule(StaticEntityGroup, inputDeps));
            }

            // Combine all scheduled jobs.
            var handle = JobHandle.CombineDependencies(jobHandles);
            jobHandles.Clear();

            // Build the Entity to PhysicsBody Look-ups.
            var totalBodyCount = staticBodyCount + dynamicBodyCount;
            if (totalBodyCount > 0)
            {
                handle = new CreateEntityToPhysicsBodyLookupsJob
                {
                    PhysicsBodies = PhysicsWorld.AllBodies,
                    EntityToPhysicsBodyIndex = m_EntityToPhysicsBody.Lookup.AsParallelWriter()

                }.Schedule(totalBodyCount, 128, handle);
            }

            // TODO: Build the broadphase (in the future baby!)

            FinalJobHandle = handle;
        }

        return JobHandle.CombineDependencies(FinalJobHandle, inputDeps);
    }

    #region Jobs


    // [BurstCompile] TODO
    private struct CreatePhysicsBodiesJob : IJobChunk
    {
        [ReadOnly] public ArchetypeChunkEntityType EntityType;
        [ReadOnly] public ArchetypeChunkComponentType<FixTranslation> TranslationType;
        [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> ColliderType;

        [NativeDisableContainerSafetyRestriction]
        public NativeSlice<PhysicsBody> PhysicsBodies;

        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<Entity> entities = chunk.GetNativeArray(EntityType);
            NativeArray<FixTranslation> translations = chunk.GetNativeArray(TranslationType);
            NativeArray<PhysicsCollider> colliders = chunk.GetNativeArray(ColliderType);
            
            int instanceCount = chunk.Count;

            for (int i = 0, physicsBodyIndex = firstEntityIndex; i < instanceCount; ++i, ++physicsBodyIndex)
            {
                var collider = colliders[i];
                var pos = translations[i].Value.xy;
                var halfColliderSize = collider.Size / 2;

                PhysicsBodies[physicsBodyIndex] = new PhysicsBody
                {
                    Entity = entities[i],
                    BelongsTo = collider.BelongsTo,
                    CollidesWith = collider.CollidesWith,
                    TriggersWith = collider.TriggersWith,
                    Aabb = new Aabb(min: pos - halfColliderSize, max: pos + halfColliderSize),
                };
            }
        }
    }

    // [BurstCompile] TODO
    private struct CreateEntityToPhysicsBodyLookupsJob : IJobParallelFor
    {
        [ReadOnly] public NativeSlice<PhysicsBody> PhysicsBodies;
        public NativeHashMap<Entity, int>.ParallelWriter EntityToPhysicsBodyIndex;

        public void Execute(int index)
        {
            // Add the Entity to the lookup.
            EntityToPhysicsBodyIndex.TryAdd(PhysicsBodies[index].Entity, index);
        }
    }
    #endregion
}

[UpdateAfter(typeof(BuildPhysicsWorldSystem))]
public class StepPhysicsSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
    }
}*/