using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryExtenderItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip
{
    public int InventorySizeUpgrade = 10;

    public void OnEquip(SimInventoryComponent Inventory)
    {
        Inventory.InventorySize += InventorySizeUpgrade;
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
        Inventory.InventorySize -= InventorySizeUpgrade;
        Inventory.OnInventorySizeDecrease();
    }
}
