using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootProjectileItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
{
    public int ProjectileDamage = 1;
    public int ActionToConsume = 1;

    public void OnEquip(SimInventoryComponent Inventory)
    {
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
    }

    public void OnUse(SimPlayerActions PlayerActions, object[] Informations)
    {
        PlayerActions.IncreaseValue(-1 * ActionToConsume);

        FixVector3 Destination = new FixVector3(((SimTileId)Informations[1]).X, ((SimTileId)Informations[1]).Y, 0);
        FixVector3 Direction = (FixVector3)Informations[0] - Destination;

        // PROJECTILE HERE
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
            if (simCharacterAttackComponent.WantsToShootProjectile)
            {
                simCharacterAttackComponent.OnCancelShootRequest();
            }
            else
            {
                simCharacterAttackComponent.OnRequestToShoot((SimTileId Destination) =>
                {
                    object[] ItemUsageInfo = { PawnComponent.SimTransform.WorldPosition, Destination };

                    simPlayerInputUseItem.Informations = ItemUsageInfo;

                    OnContextReady.Invoke(simPlayerInputUseItem);
                });
            }
        }
    }
}

