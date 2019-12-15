using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBank : SimSingleton<ItemBank>
{
    private List<InventoryItem> _availableItems = new List<InventoryItem>();

    public override void OnSimStart()
    {
        base.OnSimStart();

        foreach (ItemInfo item in GetComponentsInChildren<ItemInfo>())
        {
            InventoryItem inventoryItem = new InventoryItem();
            inventoryItem.ItemInstance = item.gameObject;
            inventoryItem.Name = item.Name;
            _availableItems.Add(inventoryItem);
        }
    }

    public InventoryItem GetItem(string Name)
    {
        foreach (InventoryItem item in _availableItems)
        {
            if(item.Name == Name)
            {
                return item;
            }
        }

        return null;
    }
}
