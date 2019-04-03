﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandLineBoot : MonoBehaviour
{
    void Awake()
    {
        CoreServiceManager.AddInitializationCallback(OnCoreServicesReady);
    }

    void OnCoreServicesReady()
    {
        // read the command line arguments

        QuickStartSettings quickStartSettings = new QuickStartSettings();

        int playModeValue;
        if (CommandLine.TryGetInt("-playmode", out playModeValue))
        {
            quickStartSettings.playMode = (QuickStartSettings.PlayMode)playModeValue;
        }
        else
        {
            quickStartSettings.playMode = QuickStartSettings.PlayMode.None;
        }

        CommandLine.TryGetString("-playername", out quickStartSettings.playerName);
        CommandLine.TryGetString("-servername", out quickStartSettings.serverName);
        CommandLine.TryGetString("-level", out quickStartSettings.level);


        QuickStart.Start(quickStartSettings);
    }
}
