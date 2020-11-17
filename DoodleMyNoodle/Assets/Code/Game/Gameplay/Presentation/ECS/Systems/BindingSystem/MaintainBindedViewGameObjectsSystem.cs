using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngineX;


public struct BindedGameObjectTag : ISystemStateComponentData
{
}

[UpdateAfter(typeof(BindedViewEntityCommandBufferSystem))]
public class MaintainBindedViewGameObjectsSystem : ViewSystemBase
{
    protected override void OnUpdate()
    {
        DestroyOldGameObjects();

        if (SimAssetBankInstance.Ready)
        {
            CreateNewGameObjects(SimAssetBankInstance.GetLookup());
        }
    }

    private void DestroyOldGameObjects()
    {
        Entities
            .WithAll<BindedGameObjectTag>()
            .WithNone<BindedSimEntity>()
            .WithoutBurst()
            .WithStructuralChanges()
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

                EntityManager.RemoveComponent<BindedGameObjectTag>(entity);
            }).Run();
    }

    private void CreateNewGameObjects(SimAssetBank.Lookup simAssetBank)
    {
        Entities
            .WithSharedComponentFilter(new BindedViewType() { Value = ViewTechType.GameObject })
            .WithNone<BindedGameObjectTag>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity viewEntity, in SimAssetId id, in BindedSimEntity simEntity) =>
            {
                SimAsset simAsset = simAssetBank.GetSimAsset(id);

                if (simAsset.BindedViewPrefab != null)
                {
                    GameObject viewGO = Object.Instantiate(simAsset.BindedViewPrefab);

                    var bindedSimEntityManaged = viewGO.GetOrAddComponent<BindedSimEntityManaged>();
                    bindedSimEntityManaged.Register(simEntity);

                    EntityManager.AddComponentObject(viewEntity, bindedSimEntityManaged);
                    EntityManager.AddComponentObject(viewEntity, viewGO.transform);
                }

                EntityManager.AddComponent<BindedGameObjectTag>(viewEntity);
            }).Run();
    }
}