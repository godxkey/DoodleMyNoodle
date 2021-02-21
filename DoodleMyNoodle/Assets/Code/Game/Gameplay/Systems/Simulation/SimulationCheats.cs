using System;
using UnityEngine;
using UnityEngineX;


public static class SimulationCheats
{
    [Updater.StaticUpdateMethod(UpdateType.Update)]
    public static void Update()
    {
        if (Input.GetKey(KeyCode.C) && !GameConsole.IsOpen())
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                CheatTeleportAtMouse();
            }
        }
    }


    [ConsoleCommand(Description = "Kill the local player pawn")]
    public static void CheatSuicide()
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        PresentationHelpers.SubmitInput(new SimInputCheatKillPlayerPawn()
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

        PresentationHelpers.SubmitInput(new SimInputCheatToggleInvincible()
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

        PresentationHelpers.SubmitInput(new SimInputCheatDamagePlayer()
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

        PresentationHelpers.SubmitInput(new SimInputCheatDamagePlayer()
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

        PresentationHelpers.SubmitInput(new SimInputCheatAddAllItems()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
        });
    }

    [ConsoleCommand(Description = "Next Turn")]
    public static void CheatNextTurn()
    {
        PresentationHelpers.SubmitInput(new SimInputCheatNextTurn());
    }

    [ConsoleCommand(Description = "Give your pawn infinit action points")]
    public static void CheatInfinitAP()
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        PresentationHelpers.SubmitInput(new SimInputCheatInfiniteAP()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
        });
    }

    [ConsoleCommand(Description = "Teleport your character at the current mouse location. Shortcut: C + T")]
    public static void CheatTeleportAtMouse()
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        Vector3 destination = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        PresentationHelpers.SubmitInput(new SimInputCheatTeleport()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
            Destination = new fix2((fix)destination.x, (fix)destination.y)
        });
    }
}