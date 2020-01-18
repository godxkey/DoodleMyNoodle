using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerIdHelpers
{
    public static PlayerId GetLocalPlayerID()
    {
        return PlayerRepertoireSystem.Instance.GetLocalPlayerInfo().PlayerId;
    }

    public static PlayerInfo GetLocalPlayerInfo()
    {
        return PlayerRepertoireSystem.Instance.GetLocalPlayerInfo();
    }

    public static SimPlayerComponent GetLocalSimPlayerComponent()
    {
        return PlayerIdHelpers.GetSimPlayerFromPlayer(PlayerRepertoireSystem.Instance.GetLocalPlayerInfo());
    }

    public static PlayerInfo GetPlayerFromSimPlayer(SimPlayerComponent simPlayer)
    {
        return GetPlayerFromSimPlayer(simPlayer.SimPlayerId);
    }

    public static PlayerInfo GetPlayerFromSimPlayer(in SimPlayerId simPlayerId)
    {
        foreach (PlayerInfo player in PlayerRepertoireSystem.Instance.Players)
        {
            if (player.SimPlayerId == simPlayerId)
            {
                return player;
            }
        }

        return null;
    }

    public static SimPlayerComponent GetSimPlayerFromPlayer(in PlayerId playerId)
    {
        if (PlayerRepertoireSystem.Instance == null)
            return null;

        PlayerInfo playerInfo = PlayerRepertoireSystem.Instance.GetPlayerInfo(playerId);

        return GetSimPlayerFromPlayer(playerInfo);
    }

    public static SimPlayerComponent GetSimPlayerFromPlayer(PlayerInfo playerInfo)
    {
        if (playerInfo == null || SimPlayerManager.Instance == null)
            return null;

        foreach (SimPlayerComponent simPlayer in SimulationView.EntitiesWithComponent<SimPlayerComponent>())
        {
            if(simPlayer.SimPlayerId == playerInfo.SimPlayerId)
            {
                return simPlayer;
            }
        }

        return null;
    }
}