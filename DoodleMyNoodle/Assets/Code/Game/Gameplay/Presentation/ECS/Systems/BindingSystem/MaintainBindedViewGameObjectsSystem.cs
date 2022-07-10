using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using UnityEngineX;


public struct BindedGameObjectTag : ISystemStateComponentData
{
}

[UpdateAfter(typeof(BindedViewEntityCommandBufferSystem))]
public partial class MaintainBindedViewGameObjectsSystem : ViewSystemBase
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
            .WithAll<BindedViewType_GameObject>()
            .WithNone<BindedGameObjectTag>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity viewEntity, in SimAssetId id, in BindedSimEntity simEntity) =>
            {
                EntityManager.AddComponent<BindedGameObjectTag>(viewEntity);

                SimAsset simAsset = simAssetBank.GetSimAsset(id);

                if (simAsset.BindedViewPrefab != null)
                {
                    GameObject viewGO = Object.Instantiate(simAsset.BindedViewPrefab);

                    var bindedSimEntityManaged = viewGO.GetOrAddComponent<BindedSimEntityManaged>();
                    bindedSimEntityManaged.Register(simEntity);

                    EntityManager.AddComponentObject(viewEntity, bindedSimEntityManaged);
                    EntityManager.AddComponentObject(viewEntity, viewGO.transform);
                }
            }).Run();
    }
}