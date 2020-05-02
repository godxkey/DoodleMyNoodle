using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimShieldItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
{
    public int ActionToConsume = 1;

    public void OnEquip(SimInventoryComponent Inventory)
    {
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
    }

    public void OnUse(SimPlayerActions PlayerActions, object[] Informations)
    {
        SimHealthStatComponent simHealthStatComponent = PlayerActions.GetComponent<SimHealthStatComponent>();

        PlayerActions.IncreaseValue(-1 * ActionToConsume);

        simHealthStatComponent.Invincible = true;
    }

    public void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady)
    {
        SimPlayerInputUseItem simPlayerInputUseItem = new SimPlayerInputUseItem();
        simPlayerInputUseItem.SimPlayerIdOld = simPlayerId;
        simPlayerInputUseItem.ItemIndex = itemIndex;

        SimPlayerActions simPlayerActions = PawnComponent.GetComponent<SimPlayerActions>();

        if (simPlayerActions.CanTakeAction(ActionToConsume))
        {
            object[] ItemUsageInfo = {  };

            //simPlayerInputUseItem.Informations = ItemUsageInfo;

            OnContextReady.Invoke(simPlayerInputUseItem);
        }
    }
}
