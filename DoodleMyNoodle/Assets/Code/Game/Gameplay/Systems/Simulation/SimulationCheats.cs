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
    /// These cheats can only be used when the player is created
    /// </summary>
    public const string LOCAL_PLAYER_GROUP = "sim-cheats-local-player";

    /// <summary>
    /// These cheats can be used anytime
    /// </summary>
    public const string GLOBAL_GROUP = "sim-cheats-global";

    private static DirtyValue<bool> s_globalGroupEnabled;
    private static DirtyValue<bool> s_localPawnGroupEnabled;
    private static DirtyValue<bool> s_localPlayerGroupEnabled;

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

        s_globalGroupEnabled.Set(cache?.SimWorld != null && cache.SimWorld.HasSingleton<GameStartedTag>());
        s_localPawnGroupEnabled.Set(s_globalGroupEnabled && cache.SimWorld.Exists(GamePresentationCache.Instance.LocalPawn));
        s_localPlayerGroupEnabled.Set(s_globalGroupEnabled && GamePresentationCache.Instance.LocalControllerExists);

        if (s_globalGroupEnabled.ClearDirty())
        {
            GameConsole.SetGroupEnabled(GLOBAL_GROUP, s_globalGroupEnabled);
        }

        if (s_localPawnGroupEnabled.ClearDirty())
        {
            GameConsole.SetGroupEnabled(LOCAL_PAWN_GROUP, s_localPawnGroupEnabled);
        }

        if (s_localPlayerGroupEnabled.ClearDirty())
        {
            GameConsole.SetGroupEnabled(LOCAL_PLAYER_GROUP, s_localPlayerGroupEnabled);
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

    [ConsoleCommand(Description = "Possess a specific player and prevent the others from attacking.", EnableGroup = LOCAL_PLAYER_GROUP)]
    public static void CheatSoloPlay(int playerIndex)
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        PresentationHelpers.SubmitInput(new SimInputCheatSoloPlay()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
            PawnIndex = playerIndex
        });

    }
    [ConsoleCommand(Description = "Set the player group speed", EnableGroup = LOCAL_PLAYER_GROUP)]
    public static void CheatPlayerSpeed(float playerSpeed)
    {
        PresentationHelpers.SubmitInput(new SimInputCheatPlayerSpeed()
        {
            PlayerGroupSpeed = (fix)playerSpeed
        });
    }

    [ConsoleCommand(Description = "Fred's test", EnableGroup = LOCAL_PLAYER_GROUP)]
    public static void CheatTestFred()
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }

        PresentationHelpers.SubmitInput(new SimInputCheatTestFred()
        {
            PlayerId = localPlayerInfo.SimPlayerId,
        });
    }


    [ConsoleCommand(Description = "Enable or disable the ability for player characters to auto-attack.", EnableGroup = LOCAL_PLAYER_GROUP)]
    public static void CheatPlayersAutoAttackEnabled(bool enabled)
    {
        PresentationHelpers.SubmitInput(new SimInputCheatPlayerAutoAttackEnabled()
        {
            Enabled = enabled
        });
    }

    [ConsoleCommand(Description = "Enable or disable the ability for player characters to auto-attack.", EnableGroup = LOCAL_PAWN_GROUP)]
    public static void CheatPossessPawn(int index)
    {
        var localPlayerInfo = PlayerHelpers.GetLocalPlayerInfo();

        if (localPlayerInfo == null)
        {
            Log.Warning("No local player found");
            return;
        }
        PresentationHelpers.SubmitInput(new SimInputCheatPossessPawn()
        {
            PawnIndex = index,
            PlayerId = localPlayerInfo.SimPlayerId,
        });
    }

    [ConsoleCommand(Description = "Enable or disable the ability for player characters to auto-attack.", EnableGroup = GLOBAL_GROUP)]
    public static void CheatMultiplyModHP(float multiplier)
    {
        PresentationHelpers.SubmitInput(new SimInputCheatMultiplyMobHP()
        {
            Multiplier = (fix)multiplier
        });
    }

    [ConsoleCommand(Description = "Change the level to the next", EnableGroup = GLOBAL_GROUP)]
    public static void CheatNextLevel()
    {
        PresentationHelpers.SubmitInput(new SimInputCheatNextLevel()
        {
        });
    }
}