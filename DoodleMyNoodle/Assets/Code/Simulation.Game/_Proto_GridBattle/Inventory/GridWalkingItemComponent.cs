using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO : CHANGE IT THAT INSTED OF ADDING GRID WALKING, THIS COMPONENT IS THE ONE MOVING US WHEN USE() IS CALLED
public class GridWalkingItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse
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

    public void OnUse(SimPlayerActions PlayerActions)
    {
        if (PlayerActions.CanTakeAction())
        {
            if (PlayerActions.GetComponent<SimGridWalkerComponent>().WantsToWalk)
            {
                PlayerActions.GetComponent<SimGridWalkerComponent>().WantsToWalk = false;
                PlayerActions.GetComponent<SimGridWalkerComponent>().ChoiceMade = true;
            }
            else
            {
                PlayerActions.GetComponent<SimGridWalkerComponent>().ChoiceMade = false;
                PlayerActions.GetComponent<SimGridWalkerComponent>().WantsToWalk = true;
            }
        }
    }
}
