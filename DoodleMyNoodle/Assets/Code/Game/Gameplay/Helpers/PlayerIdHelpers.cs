using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerIdHelpers
{
    public static PlayerInfo GetPlayerFromSimPlayer(in ISimPlayerInfo simPlayerId)
    {
        return GetPlayerFromSimPlayer(simPlayerId.SimPlayerId);
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

    public static ISimPlayerInfo GetSimPlayerFromPlayer(in PlayerId playerId)
    {
        if (PlayerRepertoireSystem.Instance == null)
            return null;

        PlayerInfo playerInfo = PlayerRepertoireSystem.Instance.GetPlayerInfo(playerId);

        return GetSimPlayerFromPlayer(playerInfo);
    }

    public static ISimPlayerInfo GetSimPlayerFromPlayer(PlayerInfo playerInfo)
    {
        if (playerInfo == null || SimPlayerManager.Instance == null)
            return null;

        foreach (ISimPlayerInfo simPlayer in SimPlayerManager.Instance.Players)
        {
            if(simPlayer.SimPlayerId == playerInfo.SimPlayerId)
            {
                return simPlayer;
            }
        }

        return null;
    }
}