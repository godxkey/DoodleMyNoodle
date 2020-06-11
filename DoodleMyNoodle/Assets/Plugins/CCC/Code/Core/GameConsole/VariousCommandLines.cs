using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal static class VariousCommandLines
{
    static bool done = false;

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad() // Executed after scene is loaded and game is running
    {
        if (done == false)
        {
            done = true;
            RegisterCommands();
        }
    }

    static void RegisterCommands()
    {
        GameConsole.AddCommand("openlog", Cmd_OpenLog, "Open the log file location");
        GameConsole.AddCommand("set_resolution", Cmd_SetResolution, "set screen resolution");
    }

    private static void Cmd_SetResolution(string[] args)
    {
        if (args.Length == 3)
            Screen.SetResolution(int.Parse(args[0]), int.Parse(args[1]), bool.Parse(args[2]));
        else
            Screen.SetResolution(int.Parse(args[0]), int.Parse(args[1]), Screen.fullScreenMode);
    }

    static void Cmd_OpenLog(string[] args)
    {
        string path = Application.persistentDataPath;
        path = path.Replace('/', '\\');
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        else
        {
            DebugService.LogWarning("Cannot open log file location(" + path + "). Directory was not found.");
        }
    }
}
