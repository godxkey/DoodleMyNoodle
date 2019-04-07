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
        DebugService.Log("DebugService initialized. Logging to " + engineLogFileLocation + "/" + name);
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
                _Log(message);
                break;
            case LogType.Warning:
                _LogWarning(message);
                break;
            case LogType.Assert:
            case LogType.Exception:
            case LogType.Error:
                _LogError(message);
                _LogError(stack);
                break;
        }
    }

    public static void Log(string message, bool displayOnScreen = false)
    {
        if (forwardToNativeUnityDebug)
            Debug.Log(message);
        else
            _Log(message);

        if (displayOnScreen)
        {
            DebugScreenMessage.DisplayMessage(message);
        }
    }

    public static void LogError(string message, bool displayOnScreen = false)
    {
        if (forwardToNativeUnityDebug)
            Debug.LogError(message);
        else
            _LogError(message);

        if (displayOnScreen)
        {
            DebugScreenMessage.DisplayMessage(message);
        }
    }

    public static void LogWarning(string message, bool displayOnScreen = false)
    {
        if (forwardToNativeUnityDebug)
            Debug.LogWarning(message);
        else
            _LogWarning(message);

        if (displayOnScreen)
        {
            DebugScreenMessage.DisplayMessage(message);
        }
    }

    static void _Log(string message)
    {
        string result = FrameService.FrameCount + ": " + message;

        GameConsole.Write(result, GameConsole.LineColor.Normal); // console GUI 

        if (logFile != null)
            logFile.WriteLine(result + "\n");
    }

    static void _LogError(string message, bool stackTrace = true)
    {
        string result = FrameService.FrameCount + ": [ERR] " + message;
        if (stackTrace)
        {
            result += '\n' + StackTraceUtility.ExtractStackTrace();
        }

        GameConsole.Write(result, GameConsole.LineColor.Error);
        if (logFile != null)
            logFile.WriteLine(result + "\n");
    }

    static void _LogWarning(string message)
    {
        string result = FrameService.FrameCount + ": [WARN] " + message;

        GameConsole.Write(result, GameConsole.LineColor.Warning);

        if (logFile != null)
            logFile.WriteLine(result + "\n");
    }
}
