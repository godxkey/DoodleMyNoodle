using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEntityInDirectionItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
{
    [SerializeField] private SimVelocityComponent _projectilePrefab;
    [SerializeField] private fix _throwSpeed;

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

        fix2 Destination = new fix2(((SimTileId_OLD)Informations[1]).X, ((SimTileId_OLD)Informations[1]).Y);
        fix2 Direction = Destination - (fix2)Informations[0];

        var projectileEntity = Simulation.Instantiate(_projectilePrefab);
        projectileEntity.SimTransform.WorldPosition = (fix3)Destination + new fix3(0, 0, (fix)(-0.5f));

        if (projectileEntity.TryGetComponent(out SimVelocityComponent velocityComponent))
        {
            Direction.Normalize();

            velocityComponent.Value = Direction * _throwSpeed;
        }
    }

    public void TryGetUsageContext(SimPawnComponent PawnComponent, SimPlayerId simPlayerId, int itemIndex, Action<SimPlayerInputUseItem> OnContextReady)
    {
        SimPlayerInputUseItem simPlayerInputUseItem = new SimPlayerInputUseItem();
        simPlayerInputUseItem.SimPlayerIdOld = simPlayerId;
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
                simCharacterAttackComponent.OnRequestToShoot((SimTileId_OLD Destination) =>
                {
                    object[] ItemUsageInfo = { new fix2(PawnComponent.SimTransform.WorldPosition.x, PawnComponent.SimTransform.WorldPosition.y), Destination };

                    //simPlayerInputUseItem.Informations = ItemUsageInfo;

                    OnContextReady.Invoke(simPlayerInputUseItem);
                });
            }
        }
    }
}

