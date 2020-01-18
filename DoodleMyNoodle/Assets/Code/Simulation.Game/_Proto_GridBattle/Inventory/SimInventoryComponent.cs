using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimInventoryComponent : SimComponent
{
    public List<SimItem> StartInventory = new List<SimItem>();

    public int InventorySize = 1;

    private List<SimItem> _inventory = new List<SimItem>();

    public override void OnSimAwake() 
    {
        base.OnSimAwake();

        TakeItem(ItemBank.Instance.GetItemWithSameName("Backpack"));

        foreach (SimItem item in StartInventory)
        {
            TakeItem(item);
        }
    }

    private int AddItem(SimItem item, int position = -1)
    {
        if (_inventory.Count >= InventorySize)
            return -1;

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

    private bool RemoveItem(SimItem item)
    {
        return _inventory.Remove(item);
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
        if (position >= _inventory.Count)
            return null;

        return _inventory[position];
    }

    public void TakeItem(SimItem item)
    {
        if(AddItem(item) >= 0)
        {
            item.OnEquip(this);
        }
    }

    public void DropItem(SimItem item)
    {
        if (RemoveItem(item))
        {
            item.OnUnequip(this);
        }
    }
}
