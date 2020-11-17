using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms.VisualStyles;
using UnityEngine;
using UnityEngineX;

internal static class VariousCommandLines
{
    [ConsoleCommand]
    private static void OpenSaveLocation() => OpenDirectory(Application.persistentDataPath);

    private static void OpenDirectory(string path)
    {
        path = path.Replace('/', '\\');
        if (Directory.Exists(path))
        {
            Process.Start("explorer.exe", path);
        }
        else
        {
            Log.Warning("Cannot open log file location(" + path + "). Directory was not found.");
        }
    }

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

    [ConsoleCommand]
    private static void SupportedResolutions()
    {
        foreach (Resolution resolution in Screen.resolutions)
        {
            Log.Info(resolution);
        }
    }

    [ConsoleCommand]
    private static void SetWindowed() => Screen.fullScreenMode = FullScreenMode.Windowed;

    [ConsoleCommand]
    private static void SetWindowedMaximized() => Screen.fullScreenMode = FullScreenMode.MaximizedWindow;

    [ConsoleCommand]
    private static void SetFullscreenWindowed() => Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

    [ConsoleCommand]
    private static void SetFullscreenExclusive() => Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;

    [ConsoleCommand]
    private static void CurrentResolution()
    {
        Log.Info(Screen.currentResolution);
    }

    [ConsoleVar(Description = "The screen's target refresh rate. Set to 0 for maximal fps. NB: Has no effect if vsync enabled.")]
    private static int TargetFrameRate
    {
        get => Application.targetFrameRate;
        set
        {
            Application.targetFrameRate = value;
            if (QualitySettings.vSyncCount > 0)
                Log.Warning("TargetFrameRate has no effect when vsync is on.");
        }
    }

    [ConsoleVar(Description = "The monitor's preferred refresh rate. Set to 0 for maximal fps.")]
    private static int MonitorRefreshRate
    {
        get => Screen.currentResolution.refreshRate;
        set
        {
            Screen.SetResolution(Screen.width, Screen.height, Screen.fullScreenMode, value);

            if (Screen.fullScreenMode != FullScreenMode.ExclusiveFullScreen)
            {
                Log.Warning($"{nameof(MonitorRefreshRate)} has no effect if fullscreenMode != ExlcusiveFullscreen.");
            }
        }
    }

    [ConsoleVar]
    private static bool VSync
    {
        get => QualitySettings.vSyncCount > 0;
        set => QualitySettings.vSyncCount = value ? 1 : 0;
    }

    [ConsoleCommand(Description = "Open the log file location")]
    static void OpenLog()
    {
        string path = Path.GetDirectoryName(Application.consoleLogPath);
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
