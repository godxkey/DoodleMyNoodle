using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealAbilityItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
{
    public int HealAmount = 10;
    public int ActionToConsume = 1;

    public void OnEquip(SimInventoryComponent Inventory)
    {
        Inventory.GetComponent<SimComponentsLinker>().AddComponent<SimCharacterHealComponent>();
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
        Inventory.GetComponent<SimComponentsLinker>().RemoveComponent<SimCharacterHealComponent>();
    }

    public void OnUse(SimPlayerActions PlayerActions, object[] Informations)
    {
        SimCharacterHealComponent simHealComponent = PlayerActions.GetComponent<SimCharacterHealComponent>();

        PlayerActions.IncreaseValue(-1 * ActionToConsume);

        simHealComponent.TryToHeal((SimTileId_OLD)Informations[0], HealAmount);
    }

    public void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady)
    {
        SimPlayerInputUseItem simPlayerInputUseItem = new SimPlayerInputUseItem();
        simPlayerInputUseItem.SimPlayerIdOld = simPlayerId;
        simPlayerInputUseItem.ItemIndex = itemIndex;

        SimPlayerActions simPlayerActions = PawnComponent.GetComponent<SimPlayerActions>();
        SimCharacterHealComponent simCharacterHealComponent = PawnComponent.GetComponent<SimCharacterHealComponent>();

        if (simPlayerActions.CanTakeAction(ActionToConsume))
        {
            if (simCharacterHealComponent.WantsToHeal)
            {
                simCharacterHealComponent.OnCancelHealRequest();
            }
            else
            {
                simCharacterHealComponent.OnRequestToHeal((SimTileId_OLD Destination) =>
                {
                    object[] ItemUsageInfo = { Destination };

                    //simPlayerInputUseItem.Informations = ItemUsageInfo;

                    OnContextReady.Invoke(simPlayerInputUseItem);
                });
            }
        }
    }
}

