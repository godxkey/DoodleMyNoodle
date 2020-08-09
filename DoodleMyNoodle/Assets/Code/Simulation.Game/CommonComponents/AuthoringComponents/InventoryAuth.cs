using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InventoryAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int Size = 6;
    public bool IsRandom = false;

    public List<GameObject> InitialItems = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InventorySize() { Value = Size });

        dstManager.AddBuffer<InventoryItemReference>(entity);

        DynamicBuffer<StartingInventoryItem> startingInventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

        if (IsRandom)
            InitialItems.Shuffle();

        foreach (GameObject itemGO in InitialItems)
        {
            if (startingInventory.Length >= Size)
                break;

            startingInventory.Add(new StartingInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemGO) });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(InitialItems);
    }
}


internal partial class CommonWrites
{
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
            if (!CommonReads.IsInventoryFull(accessor, destinationEntity) || !TryIncrementStackableItemInInventory(accessor, destinationEntity, itemInstance, inventory))
            {
                inventory.Add(new InventoryItemReference() { ItemEntity = itemInstance });
            }
        }
    }

    public static bool TryIncrementStackableItemInInventory(ISimWorldReadWriteAccessor accessor, Entity InventoryEntity, Entity sourceItemEntity, DynamicBuffer<InventoryItemReference> inventory)
    {
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
        if (accessor.TryGetComponentData(sourceItemEntity, out ItemStackableData stackableData))
        {
            if (stackableData.Value == 1)
            {
                DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(InventoryEntity);

                for (int i = 0; i < inventory.Length; i++)
                {
                    // Found an identical item in destination inventory
                    if (sourceItemEntity == inventory[i].ItemEntity)
                    {
                        inventory.RemoveAt(i);
                        accessor.DestroyEntity(sourceItemEntity);
                        break;
                    }
                }
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

public partial class CommonReads
{
    public static bool IsInventoryFull(ISimWorldReadAccessor accessor, Entity destinationEntity)
    {
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBufferReadOnly<InventoryItemReference>(destinationEntity);
        InventorySize inventorySize = accessor.GetComponentData<InventorySize>(destinationEntity);

        return inventory.Length >= inventorySize.Value;
    }
}