using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEntityInDirectionItemComponent : SimComponent, IItemOnEquip, IItemOnUnequip, IItemOnUse, IItemTryGetUsageContext
{
    [SerializeField] private SimVelocityComponent _projectilePrefab;
    [SerializeField] private Fix64 _throwSpeed;

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

        FixVector2 Destination = new FixVector2(((SimTileId)Informations[1]).X, ((SimTileId)Informations[1]).Y);
        FixVector2 Direction = Destination - (FixVector2)Informations[0];

        var projectileEntity = Simulation.Instantiate(_projectilePrefab);
        projectileEntity.SimTransform.WorldPosition = (FixVector3)Destination + new FixVector3(0, 0, (Fix64)(-0.5f));

        if (projectileEntity.TryGetComponent(out SimVelocityComponent velocityComponent))
        {
            Direction.Normalize();

            velocityComponent.Value = Direction * _throwSpeed;
        }
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
                    object[] ItemUsageInfo = { new FixVector2(PawnComponent.SimTransform.WorldPosition.x, PawnComponent.SimTransform.WorldPosition.y), Destination };

                    simPlayerInputUseItem.Informations = ItemUsageInfo;

                    OnContextReady.Invoke(simPlayerInputUseItem);
                });
            }
        }
    }
}

