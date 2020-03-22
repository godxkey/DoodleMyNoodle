using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimCharacterHealComponent : SimEventComponent
{
    public bool WantsToHeal = false;
    public bool ChoiceMade = false;

    private Action<SimTileId_OLD> _currentHealDestinationFoundCallback = null;

    public bool TryToHeal(SimTileId_OLD simTileId, int Amount)
    {
        SimEntity targetEntity = SimTileHelpers.GetPawnOnTile(simTileId);
        if (targetEntity != null)
        {
            SimHealthStatComponent targetHealth = targetEntity.GetComponent<SimHealthStatComponent>();
            SimTeamMemberComponent myTeamComponent = GetComponent<SimTeamMemberComponent>();
            SimTeamMemberComponent targetTeamComponent = targetEntity.GetComponent<SimTeamMemberComponent>();
            if (targetHealth != null && myTeamComponent != null && targetTeamComponent != null)
            {
                if (myTeamComponent.Team == targetTeamComponent.Team)
                {
                    targetHealth.IncreaseValue(Amount);
                    return true;
                }
            }
        }

        return false;
    }

    public void OnRequestToHeal(Action<SimTileId_OLD> OnDestinationFound)
    {
        _currentHealDestinationFoundCallback = OnDestinationFound;
        ChoiceMade = false;
        WantsToHeal = true;
    }

    public void OnCancelHealRequest()
    {
        _currentHealDestinationFoundCallback = null;
        WantsToHeal = false;
        ChoiceMade = true;
    }

    public void OnHealDestinationChoosen(SimTileId_OLD simTileId)
    {
        if (WantsToHeal)
        {
            _currentHealDestinationFoundCallback?.Invoke(simTileId);
            OnCancelHealRequest();
        }
    }
}
