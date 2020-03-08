using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pointer of Item, we can put shortcuts functions to get anything on the prefab of the item here !

[System.Serializable]
public class SimItem : SimEntity, IItemOnEquip, IItemOnUnequip, IItemOnConsume, IItemOnUse, IItemTryGetUsageContext
{
    // GETTERS

    public string GetName() { return GetComponent<SimItemInfoComponent>()?.Name; }
    public string GetDescription() { return GetComponent<SimItemInfoComponent>()?.Description; }
    
    public SimItemInfoComponent GetInfo() { return GetComponent<SimItemInfoComponent>(); }

    // INTERFACE SHORTCUT CALLS

    public void OnConsume(SimPlayerActions PlayerActions)
    {
        foreach (IItemOnConsume itemOnConsume in GetComponents<IItemOnConsume>())
        {
            if (itemOnConsume != (IItemOnConsume)this)
            {
                itemOnConsume.OnConsume(PlayerActions);
            } 
        }
    }

    public void OnEquip(SimInventoryComponent Inventory)
    {
        foreach (IItemOnEquip itemOnEquip in GetComponents<IItemOnEquip>())
        {
            if(itemOnEquip != (IItemOnEquip)this)
            {
                itemOnEquip.OnEquip(Inventory);
            }
        }
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
        foreach (IItemOnUnequip itemOnUnequip in GetComponents<IItemOnUnequip>())
        {
            if (itemOnUnequip != (IItemOnUnequip)this)
            {
                itemOnUnequip.OnUnequip(Inventory);
            }
        }
    }

    public void OnUse(SimPlayerActions PlayerActions, object[] Informations)
    {
        foreach (IItemOnUse itemOnUse in GetComponents<IItemOnUse>())
        {
            if (itemOnUse != (IItemOnUse)this)
            {
                itemOnUse.OnUse(PlayerActions, Informations);
            }
        }
    }

    public void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady)
    {
        foreach (IItemTryGetUsageContext usageContext in GetComponents<IItemTryGetUsageContext>())
        {
            if (usageContext != (IItemOnUse)this)
            {
                usageContext.TryGetUsageContext(PawnComponent, simPlayerId, itemIndex, OnContextReady);
                return;
            }
        }
    }
}
