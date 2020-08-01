using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct NewInventoryItem : IBufferElementData
{
    public Entity ItemEntityPrefab;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemInventoryAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public bool IsStartingKit = false;

    public List<GameObject> ItemPrefabs = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (IsStartingKit)
        {
            dstManager.AddComponentData(entity, new ItemKitTag());
        }

        if (ItemPrefabs.Count <= 0)
            return;

        DynamicBuffer<InventoryItemPrefabReference> itemPrefabsInventory = dstManager.GetOrAddBuffer<InventoryItemPrefabReference>(entity);

        // Convert Prefabs to Entity Prefabs
        for (int i = 0; i < ItemPrefabs.Count; i++)
        {
            itemPrefabsInventory.Add(new InventoryItemPrefabReference() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(ItemPrefabs[i]) } );
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(ItemPrefabs);
    }
}

internal partial class CommonWrites
{
    // We empty the inventory / container after giving the items
    public static void MoveToEntityInventory(ISimWorldReadWriteAccessor accessor, Entity destinationEntity, DynamicBuffer<InventoryItemReference> sourceItems)
    {
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(destinationEntity);

        foreach (InventoryItemReference item in sourceItems)
        {
            if (!CommonReads.IsInventoryFull(accessor, destinationEntity) || !TryIncrementStackableItemInInventory(accessor, destinationEntity, item.ItemEntity))
            {
                inventory.Add(item);
            }
        }

        sourceItems.Clear();
    }

    public static void MoveToEntityInventory(ISimWorldReadWriteAccessor accessor, Entity destinationEntity, InventoryItemReference sourceItem)
    {
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(destinationEntity);
        if (!CommonReads.IsInventoryFull(accessor, destinationEntity) || !TryIncrementStackableItemInInventory(accessor, destinationEntity, sourceItem.ItemEntity))
        {
            inventory.Add(sourceItem);
        }
    }

    // We keep the item reference into the inventory / container even after transfering the items
    public static void InstantiateToEntityInventory(ISimWorldReadWriteAccessor accessor, Entity destinationEntity, DynamicBuffer<InventoryItemPrefabReference> sourceItemsBuffer)
    {
        if (sourceItemsBuffer.Length <= 0)
            return;

        NativeArray<InventoryItemPrefabReference> sourceItems = sourceItemsBuffer.ToNativeArray(Allocator.Temp);
        NativeArray<Entity> itemInstances = new NativeArray<Entity>(sourceItems.Length, Allocator.Temp);

        // Spawn items
        for (int i = 0; i < sourceItems.Length; i++)
        {
            itemInstances[i] = accessor.Instantiate(sourceItems[i].ItemEntityPrefab);
        }

        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(destinationEntity);
        foreach (Entity itemInstance in itemInstances)
        {
            if (!CommonReads.IsInventoryFull(accessor, destinationEntity) || !TryIncrementStackableItemInInventory(accessor, destinationEntity, itemInstance))
            {
                inventory.Add(new InventoryItemReference() { ItemEntity = itemInstance });
            }
        }
    }

    public static void InstantiateToEntityInventory(ISimWorldReadWriteAccessor accessor, Entity destinationEntity, InventoryItemPrefabReference sourceItem)
    {
        // Spawn item
        Entity itemInstance = accessor.Instantiate(sourceItem.ItemEntityPrefab);

        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(destinationEntity);
        if (!CommonReads.IsInventoryFull(accessor, destinationEntity) || !TryIncrementStackableItemInInventory(accessor, destinationEntity, itemInstance))
        {
            inventory.Add(new InventoryItemReference() { ItemEntity = itemInstance });
        }
        
    }

    public static bool TryIncrementStackableItemInInventory(ISimWorldReadWriteAccessor accessor, Entity InventoryEntity, Entity sourceItemEntity)
    {
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(InventoryEntity);
        SimAssetId sourceItemID = accessor.GetComponentData<SimAssetId>(sourceItemEntity);

        foreach (InventoryItemReference itemRef in inventory)
        {
            SimAssetId inventoryItemID = accessor.GetComponentData<SimAssetId>(itemRef.ItemEntity);

            // Found an identical item in destination inventory
            if (sourceItemID.Value == inventoryItemID.Value)
            {
                // Does the item present in destination inventory is stackable
                if (accessor.TryGetComponentData(itemRef.ItemEntity, out ItemStackableData stackableData))
                {
                    // Stack it, and notify caller transfer is not necesary
                    accessor.SetComponentData(itemRef.ItemEntity, new ItemStackableData() { Value = stackableData.Value + 1 });

                    return true;
                }
            }
        }

        return false;
    }

    public static void DecrementStackableItemInInventory(ISimWorldReadWriteAccessor accessor, Entity InventoryEntity, Entity sourceItemEntity)
    {
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(InventoryEntity);
        SimAssetId sourceItemID = accessor.GetComponentData<SimAssetId>(sourceItemEntity);

        for (int i = 0; i < inventory.Length; i++)
        {
            InventoryItemReference itemRef = inventory[i];
            SimAssetId inventoryItemID = accessor.GetComponentData<SimAssetId>(itemRef.ItemEntity);

            // Found an identical item in destination inventory
            if (sourceItemID.Value == inventoryItemID.Value)
            {
                // Does the item present in destination inventory is stackable
                if (accessor.TryGetComponentData(itemRef.ItemEntity, out ItemStackableData stackableData))
                {
                    if (stackableData.Value == 1)
                    {
                        inventory.RemoveAt(i);
                        accessor.DestroyEntity(itemRef.ItemEntity);
                    }
                    else
                    {
                        // Stack it, and notify caller transfer is not necesary
                        accessor.SetComponentData(sourceItemEntity, new ItemStackableData() { Value = stackableData.Value - 1 });
                    }

                    return;
                }
            }
        }
    }
}

public partial class CommonReads 
{
    public static bool IsInventoryFull(ISimWorldReadWriteAccessor accessor, Entity destinationEntity)
    {
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(destinationEntity);
        InventorySize inventorySize = accessor.GetComponentData<InventorySize>(destinationEntity);

        return inventory.Length >= inventorySize.Value;
    }
}