using System;
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
            CommandLine.TryGetString("-playername", out quickStartSettings.playerName);
            CommandLine.TryGetString("-servername", out quickStartSettings.serverName);
            CommandLine.TryGetString("-level", out quickStartSettings.level);
            quickStartSettings.playMode = (QuickStartSettings.PlayMode)playModeValue;


            QuickStart.Start(quickStartSettings);
        }
        else
        {
            QuickStart.StartFromScratch();
        }
    }
}
