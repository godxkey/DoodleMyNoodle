using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Inventory
{
    public List<InventoryItem> Items = new List<InventoryItem>();
    public UnityEvent OnInventoryChanged = new UnityEvent();

    public int AddItem(InventoryItem item, int position = -1)
    {
        if (position >= 0)
        {
            Items.Add(item);
            OnInventoryChanged?.Invoke();
            return Items.Count - 1;
        }
        else
        {
            Items[position] = item;
            OnInventoryChanged?.Invoke();
            return position;
        }
    }

    public InventoryItem RemoveItem(int position)
    {
        InventoryItem itemRemoved = Items[position];
        Items[position] = null;
        OnInventoryChanged?.Invoke();
        return itemRemoved;
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
