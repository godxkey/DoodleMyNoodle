using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTestItem : SimComponent, IItemOnEquip, IItemOnUnEquip
{
    public void OnEquip()
    {
        Debug.Log("Equip Base Test Item");
    }

    public void OnUnEquip()
    {
        Debug.Log("Unequip Base Test Item");
    }
}
