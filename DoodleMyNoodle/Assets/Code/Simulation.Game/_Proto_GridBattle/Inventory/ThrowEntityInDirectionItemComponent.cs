using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowEntityInDirectionItemComponent : SimItemComponent, IItemOnUse
{
    [SerializeField] private SimVelocityComponent _projectilePrefab;
    [SerializeField] private Fix64 _throwSpeed;
    [SerializeField] private int _apCost;

    public void OnUse(SimPlayerActions PlayerActions, object[] Informations)
    {
        PlayerActions.IncreaseValue(-_apCost);
        var projectileEntity = Simulation.Instantiate(_projectilePrefab);
        
        if(projectileEntity.TryGetComponent(out SimVelocityComponent velocityComponent))
        {
            var direction = (FixVector2)Informations[0];

            direction.Normalize();

            velocityComponent.Value = direction * _throwSpeed;
        }
    }
}

