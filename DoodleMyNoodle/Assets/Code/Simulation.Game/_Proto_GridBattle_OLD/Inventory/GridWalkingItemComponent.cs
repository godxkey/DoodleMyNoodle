using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : CHANGE IT THAT INSTED OF ADDING GRID WALKING, THIS COMPONENT IS THE ONE MOVING US WHEN USE() IS CALLED
public class GridWalkingItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
{
    public int StartingSpeed = 4;

    public void OnEquip(SimInventoryComponent Inventory)
    {
        SimGridWalkerComponent newGridWalkerComponent = Inventory.GetComponent<SimComponentsLinker>().AddComponent<SimGridWalkerComponent>();
        newGridWalkerComponent.Speed = StartingSpeed;
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
        Inventory.GetComponent<SimComponentsLinker>().RemoveComponent<SimGridWalkerComponent>();
    }

    public void OnUse(SimPlayerActions PlayerActions, object[] Informations)
    {
        SimGridWalkerComponent simGridWalkerComponent = PlayerActions.GetComponent<SimGridWalkerComponent>();

        PlayerActions.IncreaseValue(-CalculateAmountOfActionToMoveThere(simGridWalkerComponent.TileId, (SimTileId_OLD)Informations[0]));

        simGridWalkerComponent.TryWalkTo((SimTileId_OLD)Informations[0]);
    }

    public void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady)
    {
        SimPlayerInputUseItem simPlayerInputUseItem = new SimPlayerInputUseItem();
        simPlayerInputUseItem.SimPlayerIdOld = simPlayerId;
        simPlayerInputUseItem.ItemIndex = itemIndex;

        SimPlayerActions simPlayerActions = PawnComponent.GetComponent<SimPlayerActions>();
        SimGridWalkerComponent simGridWalkerComponent = PawnComponent.GetComponent<SimGridWalkerComponent>();

        if (simPlayerActions.CanTakeAction(simPlayerActions.Value))
        {
            if (simGridWalkerComponent.WantsToWalk)
            {
                simGridWalkerComponent.OnCancelWalkRequest();
            }
            else
            {
                simGridWalkerComponent.OnRequestToWalk((SimTileId_OLD Destination)=> 
                {
                    object[] ItemUsageInfo = { Destination };

                    simPlayerInputUseItem.Informations = ItemUsageInfo;

                    OnContextReady.Invoke(simPlayerInputUseItem);
                });
            }
        }
    }

    private int CalculateAmountOfActionToMoveThere(SimTileId_OLD start, SimTileId_OLD end)
    {
        int up = Mathf.Abs(end.Y - start.Y);
        int right = Mathf.Abs(end.X - start.X);
        return up + right;
    }
}
