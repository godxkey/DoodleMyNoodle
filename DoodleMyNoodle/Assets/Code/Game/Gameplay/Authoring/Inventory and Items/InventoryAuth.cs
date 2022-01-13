using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;
using CCC.InspectorDisplay;
using System;

[DisallowMultipleComponent]
public class InventoryAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    private const int DEFAULT_CAPACITY = 9999;

    [Serializable]
    public class ItemAndQuantity
    {
        public GameObject Item;

        [MinMaxSlider(0, 10)]
        public Vector2Int Quantity = new Vector2Int(1, 1);
    }

    public List<ItemAndQuantity> StartingItems = new List<ItemAndQuantity>();

    public int MaxItemsPerTurn = 1;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InventoryCapacity() { Value = DEFAULT_CAPACITY });
        dstManager.AddBuffer<InventoryItemReference>(entity);

        DynamicBuffer<StartingInventoryItem> startingInventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

        foreach (ItemAndQuantity itemAndQuantity in StartingItems)
        {
            if (startingInventory.Length >= DEFAULT_CAPACITY)
                break;

            if (!itemAndQuantity.Item)
                continue;

            if (!itemAndQuantity.Item.TryGetComponent(out SimAsset simAsset))
                continue;

            startingInventory.Add(new StartingInventoryItem()
            {
                ItemAssetId = simAsset.GetSimAssetId(),
                StacksMin = Mathf.Min(itemAndQuantity.Quantity.x, itemAndQuantity.Quantity.y),
                StacksMax = Mathf.Max(itemAndQuantity.Quantity.x, itemAndQuantity.Quantity.y),
            });
        }

        dstManager.AddComponentData(entity, new ItemUsedThisTurn() { Value = 0 });
        dstManager.AddComponentData(entity, new MaxItemUsesPerTurn() { Value = MaxItemsPerTurn });
    }
}