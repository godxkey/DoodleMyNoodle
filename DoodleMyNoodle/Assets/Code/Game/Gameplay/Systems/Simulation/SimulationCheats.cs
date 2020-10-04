using System;
using UnityEngine;
using UnityEngineX;


public static class SimulationCheats
{
    [ConsoleCommand(Description = "Kill the local player pawn")]
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

    [ConsoleCommand(Description = "Render your local pawn immune to damage")]
    public static void CheatInvicible()
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        GameMonoBehaviourHelpers.SubmitInput(new SimInputCheatToggleInvincible()
        {
            PlayerId = localPlayerInfo.SimPlayerId
        });
    }

    [ConsoleCommand(Description = "Damage your local pawn by the given amount")]
    public static void CheatDamageSelf(int amount)
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        GameMonoBehaviourHelpers.SubmitInput(new SimInputCheatDamagePlayer()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
            Damage = amount
        });
    }

    [ConsoleCommand(Description = "Heal your local pawn by the given amount")]
    public static void CheatHealSelf(int amount)
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        GameMonoBehaviourHelpers.SubmitInput(new SimInputCheatDamagePlayer()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
            Damage = -amount
        });
    }

    [ConsoleCommand(Description = "Give your pawn all the game items")]
    public static void CheatGiveAllItems()
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        GameMonoBehaviourHelpers.SubmitInput(new SimInputCheatAddAllItems()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
        });
    }
}