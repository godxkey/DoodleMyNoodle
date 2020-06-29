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

        DynamicBuffer<NewInventoryItem> newItems = dstManager.AddBuffer<NewInventoryItem>(entity);

        foreach (GameObject itemPrefab in ItemPrefabs)
        {
            newItems.Add(new NewInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemPrefab) });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(ItemPrefabs);
    }
}

internal partial class CommonWrites
{
    public static void EquipItemBundle(ISimWorldReadWriteAccessor accessor, Entity pawn, DynamicBuffer<NewInventoryItem> newInventoryItems)
    {
        if (newInventoryItems.Length <= 0)
            return;

        NativeArray<NewInventoryItem> itemsBuffer = newInventoryItems.ToNativeArray(Allocator.Temp);
        NativeArray<Entity> itemInstances = new NativeArray<Entity>(itemsBuffer.Length, Allocator.Temp);

        // Spawn items
        for (int i = 0; i < itemsBuffer.Length; i++)
        {
            itemInstances[i] = accessor.Instantiate(itemsBuffer[i].ItemEntityPrefab);
        }

        // Add item references into inventory
        DynamicBuffer<InventoryItemReference> inventory = accessor.GetBuffer<InventoryItemReference>(pawn);
        foreach (Entity itemInstance in itemInstances)
        {
            inventory.Add(new InventoryItemReference() { ItemEntity = itemInstance });
        }
    }
}