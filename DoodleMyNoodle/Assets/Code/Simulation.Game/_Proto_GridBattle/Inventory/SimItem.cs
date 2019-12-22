using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pointer of Item, we can put shortcuts functions to get anything on the prefab of the item here !

[System.Serializable]
public class SimItem : SimEntity, IItemOnEquip, IItemOnUnequip, IItemOnConsume, IItemOnUse
{
    // GETTERS

    public string GetName() { return GetComponent<SimItemInfoComponent>()?.Name; }
    public SimItemInfoComponent GetInfo() { return GetComponent<SimItemInfoComponent>(); }

    // INTERFACE SHORTCUT CALLS

    // TODO: Change GetComponents for something more effeciant

    public void OnConsume()
    {
        foreach (IItemOnConsume itemOnConsume in GetComponents<IItemOnConsume>())
        {
            if (itemOnConsume != this)
            {
                itemOnConsume.OnConsume();
            } 
        }
    }

    public void OnEquip()
    {
        foreach (IItemOnEquip itemOnEquip in GetComponents<IItemOnEquip>())
        {
            if(itemOnEquip != this)
            {
                itemOnEquip.OnEquip();
            }
        }
    }

    public void OnUnequip()
    {
        foreach (IItemOnUnequip itemOnUnequip in GetComponents<IItemOnUnequip>())
        {
            if (itemOnUnequip != this)
            {
                itemOnUnequip.OnUnequip();
            }
        }
    }

    public void OnUse()
    {
        foreach (IItemOnUse itemOnUse in GetComponents<IItemOnUse>())
        {
            if (itemOnUse != this)
            {
                itemOnUse.OnUse();
            }
        }
    }
}
