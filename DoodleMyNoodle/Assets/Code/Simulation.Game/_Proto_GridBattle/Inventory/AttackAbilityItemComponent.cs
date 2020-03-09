using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbilityItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
{
    public int StartingAttackDamage = 1;
    public int ActionToConsume = 1;

    public void OnEquip(SimInventoryComponent Inventory)
    {
        SimCharacterAttackComponent newCharacterAttackComponent = Inventory.GetComponent<SimComponentsLinker>().AddComponent<SimCharacterAttackComponent>();
        newCharacterAttackComponent.AttackDamage = StartingAttackDamage;
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
        Inventory.GetComponent<SimComponentsLinker>().RemoveComponent<SimCharacterAttackComponent>();
    }

    public void OnUse(SimPlayerActions PlayerActions, object[] Informations)
    {
        SimCharacterAttackComponent simAttackComponent = PlayerActions.GetComponent<SimCharacterAttackComponent>();

        PlayerActions.IncreaseValue(-1 * ActionToConsume);

        simAttackComponent.TryToAttack((SimTileId)Informations[0]);
    }

    public void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady)
    {
        SimPlayerInputUseItem simPlayerInputUseItem = new SimPlayerInputUseItem();
        simPlayerInputUseItem.SimPlayerId = simPlayerId;
        simPlayerInputUseItem.ItemIndex = itemIndex;

        SimPlayerActions simPlayerActions = PawnComponent.GetComponent<SimPlayerActions>();
        SimCharacterAttackComponent simCharacterAttackComponent = PawnComponent.GetComponent<SimCharacterAttackComponent>();

        if (simPlayerActions.CanTakeAction(ActionToConsume))
        {
            if (simCharacterAttackComponent.WantsToAttack)
            {
                simCharacterAttackComponent.OnCancelAttackRequest();
            }
            else
            {
                simCharacterAttackComponent.OnRequestToAttack((SimTileId Destination) =>
                {
                    object[] ItemUsageInfo = { Destination };

                    simPlayerInputUseItem.Informations = ItemUsageInfo;

                    OnContextReady.Invoke(simPlayerInputUseItem);
                });
            }
        }
    }
}
