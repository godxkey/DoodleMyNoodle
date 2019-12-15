using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Inventory
{
    public List<InventoryItem> Items = new List<InventoryItem>();

    public int AddItem(InventoryItem item, int position = -1)
    {
        if (position < 0)
        {
            Items.Add(item);
            return Items.Count - 1;
        }
        else
        {
            Items[position] = item;
            return position;
        }
    }

    public InventoryItem RemoveItem(InventoryItem item)
    {
        InventoryItem itemRemoved = GetItem(item);
        Items.Remove(itemRemoved);
        return itemRemoved;
    }

    public InventoryItem RemoveItem(int position)
    {
        InventoryItem itemRemoved = GetItem(position);
        Items[position] = null;
        return itemRemoved;
    }

    public InventoryItem GetItem(InventoryItem item)
    {
        foreach (InventoryItem inventoryItem in Items)
        {
            if (item.Name == inventoryItem.Name)
            {
                return item;
            }
        }

        return null;
    }

    public InventoryItem GetItem(int position)
    {
        return Items[position];
    }

    // todo
    // could Add GetAllItem , to get all identical item we have in the inventory
    // sort items
    // stacking
    // inventory size
}
