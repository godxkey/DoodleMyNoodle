using CCC.InspectorDisplay;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCharacterAttackComponent : SimEventComponent
{
    [System.Serializable]
    struct SerializedData
    {
        public int AttackDamage;
    }

    public bool WantsToAttack = false;
    public bool AttackChoiceMade = false;

    public bool WantsToShootProjectile = false;
    public bool ShootProjectileChoiceMade = false;

    private Action<SimTileId_OLD> _currentAttackDestinationFoundCallback = null;
    private Action<SimTileId_OLD> _currentProjectileDestinationFoundCallback = null;


    public int AttackDamage { get => _data.AttackDamage; set => _data.AttackDamage = value; }

    public bool TryToAttack(SimTileId_OLD simTileId)
    {
        SimEntity targetEntity = SimTileHelpers.GetPawnOnTile(simTileId);
        if(targetEntity != null)
        {
            SimHealthStatComponent targetHealth = targetEntity.GetComponent<SimHealthStatComponent>();
            SimTeamMemberComponent myTeamComponent = GetComponent<SimTeamMemberComponent>();
            SimTeamMemberComponent targetTeamComponent = targetEntity.GetComponent<SimTeamMemberComponent>();
            if (targetHealth != null && myTeamComponent != null && targetTeamComponent != null)
            {
                if(myTeamComponent.Team != targetTeamComponent.Team)
                {
                    targetHealth.DecreaseValue(AttackDamage);
                    return true;
                }
            }
        }

        return false;
    }

    public void OnRequestToAttack(Action<SimTileId_OLD> OnDestinationFound)
    {
        _currentAttackDestinationFoundCallback = OnDestinationFound;
        AttackChoiceMade = false;
        WantsToAttack = true;
    }

    public void OnCancelAttackRequest()
    {
        _currentAttackDestinationFoundCallback = null;
        WantsToAttack = false;
        AttackChoiceMade = true;
    }

    public void OnAttackDestinationChoosen(SimTileId_OLD simTileId)
    {
        if (WantsToAttack)
        {
            _currentAttackDestinationFoundCallback?.Invoke(simTileId);
            OnCancelAttackRequest();
        }
    }

    public void OnRequestToShoot(Action<SimTileId_OLD> OnDestinationFound)
    {
        _currentProjectileDestinationFoundCallback = OnDestinationFound;
        ShootProjectileChoiceMade = false;
        WantsToShootProjectile = true;
    }

    public void OnCancelShootRequest()
    {
        _currentProjectileDestinationFoundCallback = null;
        WantsToShootProjectile = false;
        ShootProjectileChoiceMade = true;
    }

    public void OnShootDestinationChoosen(SimTileId_OLD simTileId)
    {
        if (WantsToShootProjectile)
        {
            _currentProjectileDestinationFoundCallback?.Invoke(simTileId);
            OnCancelShootRequest();
        }
    }

    #region Serialized Data Methods
    [UnityEngine.SerializeField]
    [AlwaysExpand]
    SerializedData _data = new SerializedData()
    {
        // define default values here
    };

    public override void PushToDataStack(SimComponentDataStack dataStack)
    {
        base.PushToDataStack(dataStack);
        dataStack.Push(_data);
    }

    public override void PopFromDataStack(SimComponentDataStack dataStack)
    {
        _data = (SerializedData)dataStack.Pop();
        base.PopFromDataStack(dataStack);
    }
    #endregion
}
