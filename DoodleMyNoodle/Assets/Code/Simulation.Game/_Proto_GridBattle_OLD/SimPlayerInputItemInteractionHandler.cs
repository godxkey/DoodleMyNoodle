using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SimPlayerComponent))]
[RequireComponent(typeof(SimPawnControllerComponent))]
public class SimPlayerInputItemInteractionHandler : SimComponent, ISimPlayerInputHandler
{
    public bool HandleInput(SimPlayerInput input)
    {
        if(input is SimPlayerInputUseItem simPlayerInputUseItem)
        {
            SimPawnControllerComponent PawnController = GetComponent<SimPawnControllerComponent>();

            SimInventoryComponent Inventory = PawnController.TargetPawn.GetComponent<SimInventoryComponent>();
            SimPlayerActions simPlayerActions = PawnController.TargetPawn.GetComponent<SimPlayerActions>();

            SimItem ItemUsed = Inventory?.GetItem(simPlayerInputUseItem.ItemIndex);

            //ItemUsed?.OnUse(simPlayerActions, simPlayerInputUseItem.Informations);
        }

        return false;
    }
}
