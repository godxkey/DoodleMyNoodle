using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAbilityItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse
{
    public int StartingAttackDamage = 1;

    public void OnEquip(SimInventoryComponent Inventory)
    {
        SimCharacterAttackComponent newCharacterAttackComponent = Inventory.GetComponent<SimComponentsLinker>().AddComponent<SimCharacterAttackComponent>();
        newCharacterAttackComponent.AttackDamage = StartingAttackDamage;
    }

    public void OnUnequip(SimInventoryComponent Inventory)
    {
        Inventory.GetComponent<SimComponentsLinker>().RemoveComponent<SimCharacterAttackComponent>();
    }

    public void OnUse(SimPlayerActions PlayerActions)
    {
        if (PlayerActions.CanTakeAction())
        {
            SimCharacterAttackComponent characterAttackComponent = PlayerActions.GetComponent<SimCharacterAttackComponent>();
            if (characterAttackComponent.WantsToAttack)
            {
                characterAttackComponent.WantsToAttack = false;
                characterAttackComponent.ChoiceMade = true;
            }
            else
            {
                characterAttackComponent.ChoiceMade = false;
                characterAttackComponent.WantsToAttack = true;
            }
        }
    }
}
