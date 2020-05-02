using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AoeAttackItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
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
        SimCharacterAttackComponent simAttackComponent = PlayerActions.GetComponent<SimCharacterAttackComponent>();

        PlayerActions.IncreaseValue(-1 * ActionToConsume);

        fix3 PlayerPosition = (fix3)Informations[0];

        simAttackComponent.TryToAttack(new SimTileId_OLD((int)PlayerPosition.x + 1, (int)PlayerPosition.y));
        simAttackComponent.TryToAttack(new SimTileId_OLD((int)PlayerPosition.x - 1, (int)PlayerPosition.y));
        simAttackComponent.TryToAttack(new SimTileId_OLD((int)PlayerPosition.x, (int)PlayerPosition.y + 1));
        simAttackComponent.TryToAttack(new SimTileId_OLD((int)PlayerPosition.x, (int)PlayerPosition.y + 1));
    }

    public void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady)
    {
        SimPlayerInputUseItem simPlayerInputUseItem = new SimPlayerInputUseItem();
        simPlayerInputUseItem.SimPlayerIdOld = simPlayerId;
        simPlayerInputUseItem.ItemIndex = itemIndex;

        SimPlayerActions simPlayerActions = PawnComponent.GetComponent<SimPlayerActions>();

        if (simPlayerActions.CanTakeAction(ActionToConsume))
        {
            object[] ItemUsageInfo = { PawnComponent.SimTransform.WorldPosition };

            //simPlayerInputUseItem.Informations = ItemUsageInfo;

            OnContextReady.Invoke(simPlayerInputUseItem);
        }
    }
}
