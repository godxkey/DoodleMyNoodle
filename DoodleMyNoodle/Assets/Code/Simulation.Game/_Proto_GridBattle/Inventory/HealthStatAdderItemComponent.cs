using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthStatAdderItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip
{
    public int StartingHealth = 10;

    public void OnEquip(SimInventoryComponent Inventory)
    {
        SimHealthStatComponent newHealthComponent = Inventory.gameObject.AddComponent<SimHealthStatComponent>();
        newHealthComponent.MaxValue = StartingHealth;
        newHealthComponent.IncreaseValue(StartingHealth);
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
        Destroy(Inventory.gameObject.GetComponent<SimHealthStatComponent>());
    }
}
