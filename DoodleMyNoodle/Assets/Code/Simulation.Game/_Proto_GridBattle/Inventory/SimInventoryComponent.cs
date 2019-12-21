using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimInventoryComponent : SimComponent
{
    private List<SimItem> _inventory = new List<SimItem>();

    private int AddItem(SimItem item, int position = -1)
    {
        if (position < 0)
        {
            _inventory.Add(item);
            return _inventory.Count - 1;
        }
        else
        {
            _inventory[position] = item;
            return position;
        }
    }

    private SimItem RemoveItem(SimItem item)
    {
        SimItem itemRemoved = GetItem(item);
        _inventory.Remove(itemRemoved);
        return itemRemoved;
    }

    private SimItem RemoveItem(int position)
    {
        SimItem itemRemoved = GetItem(position);
        _inventory[position] = null;
        return itemRemoved;
    }

    public SimItem GetItem(SimItem item)
    {
        foreach (SimItem inventoryItem in _inventory)
        {
            if (item == inventoryItem)
            {
                return item;
            }
        }

        return null;
    }

    public SimItem GetItem(int position)
    {
        return _inventory[position];
    }

    public void TakeItem(SimItem item)
    {
        AddItem(item);
        item.OnEquip();
    }

    public void DropItem(SimItem item)
    {
        item.OnUnequip();
        RemoveItem(item);
    }
}
