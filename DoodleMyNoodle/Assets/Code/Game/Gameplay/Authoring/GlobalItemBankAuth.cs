using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GlobalItemBankAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GlobalItemBank Bank;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GlobalItemBankTag());
        dstManager.AddBuffer<GlobalItemBankInstanceElement>(entity);
        dstManager.AddBuffer<GlobalItemBankSimAssetIdElement>(entity);

        var prefabBuffer = dstManager.AddBuffer<GlobalItemBankPrefabElement>(entity);

        foreach (ItemAuth item in Bank.Items)
        {
            if (item == null)
                continue;

            Entity itemPrefabEntity = conversionSystem.GetPrimaryEntity(item.gameObject);

            if (itemPrefabEntity == Entity.Null)
                continue;

            prefabBuffer.Add(itemPrefabEntity);
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (Bank != null)
        {
            foreach (ItemAuth item in Bank.Items)
            {
                if (item != null)
                {
                    referencedPrefabs.Add(item.gameObject);
                }
            }
        }
    }
}