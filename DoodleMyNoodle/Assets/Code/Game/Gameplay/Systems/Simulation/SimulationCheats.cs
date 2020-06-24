using System;
using UnityEngine;
using UnityEngineX;


public static class SimulationCheats
{
    [Command(Description = "Kill the local player pawn")]
    public static void CheatSuicide()
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        GameMonoBehaviourHelpers.SubmitInput(new SimInputCheatKillPlayerPawn()
        {
            PlayerId = localPlayerInfo.SimPlayerId
        });
    }
}