using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngineX;

internal static class VariousCommandLines
{
    [ConsoleCommand]
    private static void SetResolution(int width, int height, bool fullScreen)
    {
        if (Application.isEditor)
        {
            Log.Warning("Setting resolution in editor has no effect");
        }
        else
        {
            Screen.SetResolution(width, height, fullScreen);
        }
    }

    [ConsoleCommand(Description = "Open the log file location")]
    static void OpenLog()
    {
        string path = Application.persistentDataPath;
        path = path.Replace('/', '\\');
        if (Directory.Exists(path))
        {
            System.Diagnostics.Process.Start("explorer.exe", path);
        }
        else
        {
            Log.Warning("Cannot open log file location(" + path + "). Directory was not found.");
        }
    }

    [ConsoleCommand(Description = "Exit the application")]
    static void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
