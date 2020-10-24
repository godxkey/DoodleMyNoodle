using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngineX;

[UpdateAfter(typeof(CreateBindedViewEntitiesSystem))]
[UpdateBefore(typeof(CopyTransformToViewSystem))]
public class PostSimulationBindingCommandBufferSystem : ViewEntityCommandBufferSystem { }

[UpdateAfter(typeof(DestroyDanglingViewSystem))]
//[UpdateAfter(typeof(BeginViewSystem))]
public class CreateBindedViewEntitiesSystem : ViewJobComponentSystem
{
    private EntityQuery _newSimEntitiesQ;
    private EntityQuery _allSimEntitiesQ;
    private PostSimulationBindingCommandBufferSystem _ecbSystem;

    private DirtyValue<uint> _simWorldReplaceVersion;

    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSystem = World.GetOrCreateSystem<PostSimulationBindingCommandBufferSystem>();
        RequireSingletonForUpdate<Settings_ViewBindingSystem_Binding>();
    }

    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        // fbessette: we use the 'ReplaceVersion' to mesure when we should replace all view entities.
        //            This doesn't feel like the best way to do it... Feel free to refactor
        _simWorldReplaceVersion.Set(SimWorldAccessor.ReplaceVersion);

        EntityQuery simEntitiesInNeedOfViewBinding;
        if (_simWorldReplaceVersion.ClearDirty())
        {
            // NB: No need to dispose of these queries because the world gets disposed ...
            _newSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<SimAssetId>());
            _allSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<SimAssetId>());

            simEntitiesInNeedOfViewBinding = _allSimEntitiesQ;
        }
        else
        {
            simEntitiesInNeedOfViewBinding = _newSimEntitiesQ;
        }

        jobHandle = new CreateBindedEntitiesForSimEntities()
        {
            SimEntityAccessor = SimWorldAccessor.EntityManager.GetEntityTypeHandle(),
            SimAssetIdAccessor = SimWorldAccessor.EntityManager.GetComponentTypeHandle<SimAssetId>(true),

            Ecb = _ecbSystem.CreateCommandBuffer().AsParallelWriter(),
            Bindings = EntityManager.GetBuffer<Settings_ViewBindingSystem_Binding>(GetSingletonEntity<Settings_ViewBindingSystem_Binding>())
        }.Schedule(simEntitiesInNeedOfViewBinding, jobHandle);

        _ecbSystem.AddJobHandleForProducer(jobHandle);


        jobHandle.Complete();
        return jobHandle;
    }

    [BurstCompile]
    private struct CreateBindedEntitiesForSimEntities : IJobChunk
    {
        [ReadOnly]
        public DynamicBuffer<Settings_ViewBindingSystem_Binding> Bindings;

        // Entity command buffer used to create new presentation entities
        public EntityCommandBuffer.ParallelWriter Ecb;

        [ReadOnly]
        public ComponentTypeHandle<SimAssetId> SimAssetIdAccessor;
        [ReadOnly]
        public EntityTypeHandle SimEntityAccessor;


        public void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
        {
            NativeArray<SimAssetId> simAssetIds = chunk.GetNativeArray<SimAssetId>(SimAssetIdAccessor);
            NativeArray<Entity> simEntities = chunk.GetNativeArray(SimEntityAccessor);

            for (int i = 0; i < simEntities.Length; i++)
            {
                if (!FindBindingSetting(simAssetIds[i], out Settings_ViewBindingSystem_Binding bindingSetting))
                    continue;

                Entity newPresentationEntity;

                if (bindingSetting.UseGameObjectInsteadOfEntity)
                {
                    newPresentationEntity = Ecb.CreateEntity(chunkIndex);
                    Ecb.AddComponent(chunkIndex, newPresentationEntity, new BindedGameObject() { PresentationGameObjectPrefabIndex = bindingSetting.PresentationGameObjectPrefabIndex });
                }
                else
                {
                    newPresentationEntity = Ecb.Instantiate(chunkIndex, bindingSetting.PresentationEntity);
                }

                Ecb.AddComponent(chunkIndex, newPresentationEntity, new BindedSimEntity() { SimEntity = simEntities[i] });
            }
        }


        bool FindBindingSetting(in SimAssetId simAssetId, out Settings_ViewBindingSystem_Binding bindingSetting)
        {
            for (int i = 0; i < Bindings.Length; i++)
            {
                if (Bindings[i].SimAssetId == simAssetId)
                {
                    bindingSetting = Bindings[i];
                    return true;
                }
            }

            bindingSetting = default;
            return false;
        }
    }
}

public struct BindedGameObject : IComponentData
{
    public int PresentationGameObjectPrefabIndex;
}
public struct BindedGameObjectInstantiatedTag : ISystemStateComponentData
{
}

[UpdateAfter(typeof(PostSimulationBindingCommandBufferSystem))]
[UpdateBefore(typeof(CopyTransformToViewSystem))]
public class CreateBindedViewGameObjectsSystem : ViewComponentSystem
{
    Settings_ViewBindingSystem_BindingGameObjectList _cachedSettings = null;

    Settings_ViewBindingSystem_BindingGameObjectList GetSettings()
    {
        if (_cachedSettings == null)
        {
            _cachedSettings = EntityManager.GetComponentObject<Settings_ViewBindingSystem_BindingGameObjectList>(GetSingletonEntity<Settings_ViewBindingSystem_BindingGameObjectList>());
        }

        return _cachedSettings;
    }

    protected override void OnCreate()
    {
        base.OnCreate();

        RequireSingletonForUpdate<Settings_ViewBindingSystem_BindingGameObjectList>();
    }

    protected override void OnUpdate()
    {
        // Destroy GameObjects from entities that were destroyed
        Entities
            .WithAll<BindedGameObjectInstantiatedTag>()
            .WithNone<BindedGameObject>()
            .ForEach((Entity entity) =>
            {
                if (EntityManager.HasComponent<BindedSimEntityManaged>(entity))
                {
                    BindedSimEntityManaged goComponent = EntityManager.GetComponentObject<BindedSimEntityManaged>(entity);

                    // destroy binded gameobject
                    if (goComponent)
                    {
                        goComponent.Unregister();
                        Object.Destroy(goComponent.gameObject);
                    }

                    EntityManager.RemoveComponent<BindedSimEntityManaged>(entity);
                }

                EntityManager.RemoveComponent<BindedGameObjectInstantiatedTag>(entity);
            });

        // Instantiate GameObjects for new entities
        Entities
            .WithAll<BindedGameObject>()
            .WithNone<BindedGameObjectInstantiatedTag>()
            .ForEach((Entity entity, ref BindedGameObject go, ref BindedSimEntity bindedSimEntity) =>
        {
            var settings = GetSettings();
            var goIndex = go.PresentationGameObjectPrefabIndex;

            if (goIndex >= 0 && goIndex < settings.PresentationGameObjects.Count)
            {
                GameObject gameObject = Object.Instantiate(settings.PresentationGameObjects[goIndex]);

                var bindedSimEntityManaged = gameObject.GetOrAddComponent<BindedSimEntityManaged>();
                bindedSimEntityManaged.Register(bindedSimEntity.SimEntity);

                EntityManager.AddComponentObject(entity, bindedSimEntityManaged);
                EntityManager.AddComponentObject(entity, gameObject.transform);
            }

            EntityManager.AddComponent<BindedGameObjectInstantiatedTag>(entity);
        });
    }
}