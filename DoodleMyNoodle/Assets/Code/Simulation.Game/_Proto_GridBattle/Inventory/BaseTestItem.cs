using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTestItem : SimComponent, IItemOnEquip, IItemOnUnequip
{
    public void OnEquip()
    {
        Debug.Log("Equip Base Test Item");
    }

    public void OnUnequip()
    {
        Debug.Log("Unequip Base Test Item");
    }
}
