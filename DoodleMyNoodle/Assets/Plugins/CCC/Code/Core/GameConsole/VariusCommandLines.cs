using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class VariousCommandLines
{
    public static void RegisterCommands()
    {
        GameConsole.AddCommand("openlog", Cmd_OpenLog, "Open the log file location");
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
