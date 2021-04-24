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
[RequiresEntityConversion]
public class InventoryAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [Serializable]
    public class ItemAndQuantity
    {
        public GameObject Item;

        [MinMaxSlider(0, 10)]
        public Vector2Int Quantity = new Vector2Int(1, 1);
    }

    public int Capacity = 9999;
    public List<GameObject> InitialItems = new List<GameObject>();
    public List<ItemAndQuantity> StartingItems = new List<ItemAndQuantity>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InventoryCapacity() { Value = Capacity });
        dstManager.AddBuffer<InventoryItemReference>(entity);

        DynamicBuffer<StartingInventoryItem> startingInventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

        foreach (ItemAndQuantity itemAndQuantity in StartingItems)
        {
            if (startingInventory.Length >= Capacity)
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
    }
}