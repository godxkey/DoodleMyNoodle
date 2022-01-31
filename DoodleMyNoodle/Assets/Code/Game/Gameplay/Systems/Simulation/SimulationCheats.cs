using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;


public static class SimulationCheats
{
    /// <summary>
    /// These cheats can only be used when the player has a local pawn.
    /// </summary>
    public const string LOCAL_PAWN_GROUP = "sim-cheats-local-pawn";

    /// <summary>
    /// These cheats can be used anytime
    /// </summary>
    public const string GLOBAL_GROUP = "sim-cheats-global";

    private static DirtyValue<bool> s_globalGroupEnabled;
    private static DirtyValue<bool> s_localPawnGroupEnabled;

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

        var cache = GamePresentationCache.Instance;

        s_globalGroupEnabled.Set(cache?.SimWorld != null);
        s_localPawnGroupEnabled.Set(s_globalGroupEnabled && cache.SimWorld.Exists(GamePresentationCache.Instance.LocalPawn));

        if (s_globalGroupEnabled.ClearDirty())
        {
            GameConsole.SetGroupEnabled(GLOBAL_GROUP, s_globalGroupEnabled);
        }

        if (s_localPawnGroupEnabled.ClearDirty())
        {
            GameConsole.SetGroupEnabled(LOCAL_PAWN_GROUP, s_localPawnGroupEnabled);
        }
    }

    [ConsoleCommand(Description = "Render your local pawn immune to damage", EnableGroup = LOCAL_PAWN_GROUP)]
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

    [ConsoleCommand(Description = "Damage your local pawn by the given amount", EnableGroup = LOCAL_PAWN_GROUP)]
    public static void CheatDamageSelf(int amount)
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        PresentationHelpers.SubmitInput(new SimInputCheatDamageSelf()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
            Damage = amount
        });
    }

    [ConsoleCommand(Description = "Heal your local pawn by the given amount", EnableGroup = LOCAL_PAWN_GROUP)]
    public static void CheatHealSelf(int amount)
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        PresentationHelpers.SubmitInput(new SimInputCheatDamageSelf()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
            Damage = -amount
        });
    }

    [ConsoleCommand(Description = "Give your pawn all the game items", EnableGroup = LOCAL_PAWN_GROUP)]
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

    [ConsoleCommand(Description = "Teleport your character at the current mouse location. Shortcut: C + T", EnableGroup = LOCAL_PAWN_GROUP)]
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

    [ConsoleCommand(Description = "Remove all cooldowns", EnableGroup = LOCAL_PAWN_GROUP)]
    public static void CheatRemoveAllCooldowns()
    {
        PresentationHelpers.SubmitInput(new SimInputCheatRemoveAllCooldowns());
    }

    [ConsoleCommand(Description = "Add impulse to your local pawn", EnableGroup = LOCAL_PAWN_GROUP)]
    public static void CheatImpulseSelf(float x, float y)
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        PresentationHelpers.SubmitInput(new SimInputCheatImpulseSelf()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
            ImpulseValue = new fix2((fix)x, (fix)y)
        });
    }
}