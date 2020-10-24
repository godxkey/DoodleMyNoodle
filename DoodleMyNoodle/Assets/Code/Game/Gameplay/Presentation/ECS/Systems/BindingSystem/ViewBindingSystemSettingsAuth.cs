using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class ViewBindingSystemSettingsAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public ViewBindingDefinitionBank Bank;
    public SimAssetBank SimAssetBank;

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (ViewBindingDefinition item in Bank.ViewBindingDefinitions)
        {
            if (item && item.ViewTechType == ViewBindingDefinition.ETechType.Entity)
            {
                var viewGO = item.GetViewGameObject();
                if (viewGO)
                {
                    referencedPrefabs.Add(viewGO);
                }
            }
        }

        foreach (SimAsset item in SimAssetBank.GetLookUp().SimAssets)
        {
            if (item && item.ViewTechType == SimAsset.TechType.Entity)
            {
                if (item.BindedViewPrefab)
                {
                    referencedPrefabs.Add(item.BindedViewPrefab);
                }
            }
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        SimAssetBank.LookUp simAssetBank = SimAssetBank.GetLookUp();

        var bindingGOs = new Settings_ViewBindingSystem_BindingGameObjectList();
        dstManager.AddComponentObject(entity, bindingGOs);

        var bindingSettings = dstManager.AddBuffer<Settings_ViewBindingSystem_Binding>(entity);
        bindingSettings.Capacity = Bank.ViewBindingDefinitions.Count + simAssetBank.SimAssets.Count;

        HashSet<SimAssetId> doneSimAssets = new HashSet<SimAssetId>();

        foreach (SimAsset item in simAssetBank.SimAssets)
        {
            if (item)
            {
                GameObject viewGameObject = item.BindedViewPrefab;

                if (viewGameObject == null)
                    continue;

                add(item.gameObject.name, item.GetSimAssetId(), viewGameObject, item.ViewTechType == SimAsset.TechType.Entity);
            }
        }

        foreach (ViewBindingDefinition item in Bank.ViewBindingDefinitions)
        {
            if (item)
            {
                add(item.gameObject.name, item.GetSimAssetId(), item.GetViewGameObject(), item.ViewTechType == ViewBindingDefinition.ETechType.Entity);
            }
        }

        void add(string debugName, SimAssetId assetId, GameObject viewGameObject, bool useEntityTech)
        {
            if (viewGameObject == null)
            {
                Debug.LogWarning($"The view binding definition {debugName} doesn't have a valid 'view' gameobject. It will be ignored");
                return;
            }

            if (assetId == SimAssetId.Invalid)
            {
                Debug.LogWarning($"The view binding definition {debugName} has an invalid SimAssetId. It will be ignored");
                return;
            }

            if (doneSimAssets.Contains(assetId))
                return;

            doneSimAssets.Add(assetId);

            if (useEntityTech)
            {
                bindingSettings.Add(new Settings_ViewBindingSystem_Binding()
                {
                    SimAssetId = assetId,
                    PresentationEntity = conversionSystem.GetPrimaryEntity(viewGameObject),
                    UseGameObjectInsteadOfEntity = false,
                    PresentationGameObjectPrefabIndex = -1,
                });
            }
            else
            {
                bindingSettings.Add(new Settings_ViewBindingSystem_Binding()
                {
                    SimAssetId = assetId,
                    PresentationEntity = Entity.Null,
                    UseGameObjectInsteadOfEntity = true,
                    PresentationGameObjectPrefabIndex = bindingGOs.PresentationGameObjects.Count
                });

                bindingGOs.PresentationGameObjects.Add(viewGameObject);
            }
        }
    }

    //BlobAssetReference<ViewBindingSystemSettings> CreateBlobAsset(GameObjectConversionSystem conversionSystem)
    //{
    //    BlobBuilder blobBuilder = new BlobBuilder(Allocator.Temp);

    //    ref ViewBindingSystemSettings root = ref blobBuilder.ConstructRoot<ViewBindingSystemSettings>();

    //    var ids = blobBuilder.Allocate(ref root.BlueprintIds, BlueprintDefinitions.Count);
    //    var viewEntities = blobBuilder.Allocate(ref root.BlueprintPresentationEntities, BlueprintDefinitions.Count);

    //    for (int i = 0; i < BlueprintDefinitions.Count; i++)
    //    {
    //        ids[i] = BlueprintDefinitions[i].GetBlueprintId().Value;
    //        viewEntities[i] = conversionSystem.GetPrimaryEntity(BlueprintDefinitions[i].GetViewGameObject());
    //    }

    //    BlobAssetReference<ViewBindingSystemSettings> blobRef = blobBuilder.CreateBlobAssetReference<ViewBindingSystemSettings>(Allocator.Persistent);

    //    blobBuilder.Dispose();

    //    return blobRef;
    //}
}
