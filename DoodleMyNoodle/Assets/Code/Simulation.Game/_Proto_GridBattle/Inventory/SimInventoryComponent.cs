using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimInventoryComponent : SimComponent
{
    public List<SimItem> StartInventory = new List<SimItem>();

    private List<SimItem> _inventory = new List<SimItem>();

    public override void OnSimStart() 
    {
        base.OnSimStart();

        _inventory.AddRange(StartInventory);
    }

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

    private void RemoveItem(SimItem item)
    {
        _inventory.Remove(item);
    }

    private SimItem RemoveItem(int position)
    {
        SimItem itemRemoved = GetItem(position);
        _inventory.Remove(itemRemoved);
        return itemRemoved;
    }

    public bool HasItem(SimItem item)
    {
        foreach (SimItem inventoryItem in _inventory)
        {
            if (item == inventoryItem)
            {
                return true;
            }
        }

        return false;
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
