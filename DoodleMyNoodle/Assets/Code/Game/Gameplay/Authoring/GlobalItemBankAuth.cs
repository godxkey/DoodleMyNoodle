using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GlobalItemBankAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public ItemBank Bank;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GlobalItemPrefabBankSingletonTag());

        DynamicBuffer<ItemBankPrefabReference> itemBank = dstManager.AddBuffer<ItemBankPrefabReference>(entity);

        foreach (GameObject item in Bank.Items)
        {
            Entity itemEntity = conversionSystem.GetPrimaryEntity(item);

            ItemBankPrefabReference newItemPrefabReference = new ItemBankPrefabReference() { ItemEntityPrefab = itemEntity };

            itemBank.Add(newItemPrefabReference);
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (Bank != null)
        {
            referencedPrefabs.AddRange(Bank.Items);
        }
    }
}