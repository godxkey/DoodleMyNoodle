using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

[UpdateAfter(typeof(CreateBindedViewEntitiesSystem))]
[UpdateBefore(typeof(CopyTransformToViewSystem))]
public class PostSimulationBindingCommandBufferSystem : ViewEntityCommandBufferSystem { }

[UpdateAfter(typeof(DestroyDanglingViewSystem))]
[UpdateAfter(typeof(BeginViewSystem))]
public class CreateBindedViewEntitiesSystem : ViewJobComponentSystem
{
    private EntityQuery _newSimEntitiesQ;
    private EntityQuery _allSimEntitiesQ;
    private PostSimulationBindingCommandBufferSystem _ecbSystem;

    private DirtyValue<uint> _simWorldEntityClearAndReplaceCount;

    protected override void OnCreate()
    {
        base.OnCreate();

        _newSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<NewlyCreatedTag>(), ComponentType.ReadOnly<SimAssetId>());
        _allSimEntitiesQ = SimWorldAccessor.CreateEntityQuery(ComponentType.ReadOnly<SimAssetId>());
        _ecbSystem = World.GetOrCreateSystem<PostSimulationBindingCommandBufferSystem>();

        RequireSingletonForUpdate<Settings_ViewBindingSystem_Binding>();
    }

    protected override JobHandle OnUpdate(JobHandle jobHandle)
    {
        // fbessette: we use the 'EntityClearAndReplaceCount' to mesure when we should replace all view entities.
        //            This doesn't feel like the best way to do it... Feel free to refactor
        _simWorldEntityClearAndReplaceCount.Set(SimWorldAccessor.EntityClearAndReplaceCount);


        EntityQuery simEntitiesInNeedOfViewBindingQ;
        if (_simWorldEntityClearAndReplaceCount.IsDirty)
        {
            _simWorldEntityClearAndReplaceCount.Reset();

            simEntitiesInNeedOfViewBindingQ = _allSimEntitiesQ;
        }
        else
        {
            simEntitiesInNeedOfViewBindingQ = _newSimEntitiesQ;
        }

        jobHandle = new CreateBindedEntitiesForSimEntities()
        {
            Ecb = _ecbSystem.CreateCommandBuffer().ToConcurrent(),
            Bindings = EntityManager.GetBuffer<Settings_ViewBindingSystem_Binding>(GetSingletonEntity<Settings_ViewBindingSystem_Binding>())
        }.Schedule(simEntitiesInNeedOfViewBindingQ, jobHandle);

        _ecbSystem.AddJobHandleForProducer(jobHandle);


        jobHandle.Complete();
        return jobHandle;
    }

    [BurstCompile]
    struct CreateBindedEntitiesForSimEntities : IJobForEachWithEntity_EC<SimAssetId>
    {
        [ReadOnly] public DynamicBuffer<Settings_ViewBindingSystem_Binding> Bindings;
        public EntityCommandBuffer.Concurrent Ecb;

        public void Execute(Entity simEntity, int index, [ReadOnly] ref SimAssetId simAssetId)
        {
            if (!FindBindingSetting(ref simAssetId, out Settings_ViewBindingSystem_Binding bindingSetting))
                return;

            Entity presentationEntity;

            if (bindingSetting.UseGameObjectInsteadOfEntity)
            {
                presentationEntity = Ecb.CreateEntity(index);
                Ecb.AddComponent(index, presentationEntity, new BindedGameObject() { PresentationGameObjectPrefabIndex = bindingSetting.PresentationGameObjectPrefabIndex });
            }
            else
            {
                presentationEntity = Ecb.Instantiate(index, bindingSetting.PresentationEntity);
            }

            Ecb.AddComponent(index, presentationEntity, new BindedSimEntity() { SimEntity = simEntity });
        }

        bool FindBindingSetting([ReadOnly] ref SimAssetId simAssetId, out Settings_ViewBindingSystem_Binding bindingSetting)
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

                    if (goComponent)
                    {
                        Object.Destroy(goComponent.gameObject);
                    }
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
                bindedSimEntityManaged.SimEntity = bindedSimEntity.SimEntity;
                EntityManager.AddComponentObject(entity, bindedSimEntityManaged);
                EntityManager.AddComponentObject(entity, gameObject.transform);
            }

            EntityManager.AddComponent<BindedGameObjectInstantiatedTag>(entity);
        });
    }
}