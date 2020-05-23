using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimPlayerHelpersOld
{
    public static SimPlayerComponent FindPlayerFromId(in SimPlayerId simPlayerId)
    {
        foreach (SimPlayerComponent playerComponent in Simulation.EntitiesWithComponent<SimPlayerComponent>())
        {
            if(playerComponent.SimPlayerId == simPlayerId)
            {
                return playerComponent;
            }
        }

        return null;
    }

    public static string GetPlayerName(SimPlayerComponent player)
    {
        if (player.TryGetComponent(out SimNameComponent nameComponent))
        {
            return nameComponent.Value;
        }

        return null;
    }
}
