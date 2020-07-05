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
public class ItemBundleAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
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
    // We empty the bundle / container after giving the items
    public static void MoveToEntityInventory(ISimWorldReadWriteAccessor accessor, Entity pawn, DynamicBuffer<InventoryItemReference> sourceItems)
    {
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(pawn);
        foreach (InventoryItemReference item in sourceItems)
        {
            inventory.Add(item);
        }

        sourceItems.Clear();
    }

    // We keep the item reference into the bundle / container even after transfering the items
    public static void CopyToEntityInventory(ISimWorldReadWriteAccessor accessor, Entity pawn, DynamicBuffer<InventoryItemPrefabReference> sourceItems)
    {
        if (sourceItems.Length <= 0)
            return;

        NativeArray<Entity> itemInstances = new NativeArray<Entity>(sourceItems.Length, Allocator.Temp);

        // Spawn items
        for (int i = 0; i < sourceItems.Length; i++)
        {
            itemInstances[i] = accessor.Instantiate(sourceItems[i].ItemEntityPrefab);
        }

        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(pawn);
        foreach (Entity itemInstance in itemInstances)
        {
            inventory.Add(new InventoryItemReference() { ItemEntity = itemInstance });
        }
    }
}