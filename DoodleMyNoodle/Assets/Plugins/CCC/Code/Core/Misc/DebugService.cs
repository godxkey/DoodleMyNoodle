using System;
using System.Collections.Generic;
using UnityEngine;

public class DebugService : MonoCoreService<DebugService>
{
    static bool forwardToNativeUnityDebug = true;
    static System.IO.StreamWriter logFile = null;

    public override void Initialize(Action<ICoreService> onComplete)
    {
        forwardToNativeUnityDebug = Application.isEditor;
        Application.logMessageReceived += LogCallback;

        InitLogFile();

        onComplete(this);
    }

    void InitLogFile()
    {
        List<string> commandLineArgs = new List<string>(Environment.GetCommandLineArgs());

#if UNITY_STANDALONE_LINUX
        bool isHeadless = true;
#else
        bool isHeadless = commandLineArgs.Contains("-batchmode");
#endif
        string logBaseName = isHeadless ? "serverLog_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff") : "clientLog";

        // If -logfile was passed, we try to put our own logs next to the engine's logfile
        string engineLogFileLocation = ".";
        int logfileArgIdx = commandLineArgs.IndexOf("-logfile");
        if (logfileArgIdx >= 0 && commandLineArgs.Count >= logfileArgIdx)
        {
            engineLogFileLocation = System.IO.Path.GetDirectoryName(commandLineArgs[logfileArgIdx + 1]);
        }

        // Try creating logName; attempt a number of suffixes
        string name = "";
        for (var i = 0; i < 10; i++)
        {
            name = logBaseName + (i == 0 ? "" : "_" + i) + ".log";
            try
            {
                logFile = System.IO.File.CreateText(engineLogFileLocation + "/" + name);
                logFile.AutoFlush = true;
                break;
            }
            catch
            {
                name = "<none>";
            }
        }
        DebugService.Log("GameDebug initialized. Logging to " + engineLogFileLocation + "/" + name);
    }

    protected override void OnDestroy()
    {
        Application.logMessageReceived -= LogCallback;
        if (logFile != null)
            logFile.Close();
        logFile = null;

        base.OnDestroy();
    }

    static void LogCallback(string message, string stack, LogType logtype)
    {
        switch (logtype)
        {
            default:
            case LogType.Log:
                DebugService._Log(message);
                break;
            case LogType.Warning:
                DebugService._LogWarning(message);
                break;
            case LogType.Error:
                DebugService._LogError(message);
                break;
        }
    }

    public static void Log(string message)
    {
        if (forwardToNativeUnityDebug)
            Debug.Log(message);
        else
            _Log(message);
    }

    static void _Log(string message)
    {
        Console.Write(FrameService.FrameCount + ": " + message);
        if (logFile != null)
            logFile.WriteLine(FrameService.FrameCount + ": " + message + "\n");
    }

    public static void LogError(string message)
    {
        if (forwardToNativeUnityDebug)
            Debug.LogError(message);
        else
            _LogError(message);
    }

    static void _LogError(string message)
    {
        Console.Write(FrameService.FrameCount + ": [ERR] " + message);
        if (logFile != null)
            logFile.WriteLine("[ERR] " + message + "\n");
    }

    public static void LogWarning(string message)
    {
        if (forwardToNativeUnityDebug)
            Debug.LogWarning(message);
        else
            _LogWarning(message);
    }

    static void _LogWarning(string message)
    {
        Console.Write(FrameService.FrameCount + ": [WARN] " + message);
        if (logFile != null)
            logFile.WriteLine("[WARN] " + message + "\n");
    }
}
