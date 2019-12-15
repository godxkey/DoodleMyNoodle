using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimInventoryContainer : SimComponent
{
    public Inventory Inventory;

    // multiple bags or multiple inventory here

    public void TakeItem(InventoryItem item)
    {
        Inventory.AddItem(item);
        foreach (IItemOnEquip itemOnEquip in item.ItemInstance.GetComponents<IItemOnEquip>())
        {
            itemOnEquip?.OnEquip();
        }
    }

    public void DropItem(InventoryItem item)
    {
        foreach (IItemOnUnEquip itemOnEquip in item.ItemInstance.GetComponents<IItemOnUnEquip>())
        {
            itemOnEquip?.OnUnEquip();
        }
        Inventory.RemoveItem(item);
    }
}
