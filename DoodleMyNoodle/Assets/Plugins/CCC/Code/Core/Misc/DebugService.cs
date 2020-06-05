using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugService : MonoCoreService<DebugService>
{
    const bool log = false;
    const string DELAYED_TO_MAIN_THREAD_SUFFIX = "   (Log was deferred to main thread)";

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
        if (log)
#pragma warning disable CS0162 // Unreachable code detected
            DebugService.Log("DebugService initialized. Logging to " + engineLogFileLocation + "/" + name);
#pragma warning restore CS0162 // Unreachable code detected
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
        StringBuilder stringBuilder = StringBuilderPool.Take();
        stringBuilder.AppendLine(message);
        stringBuilder.AppendLine(stack);

        string str = stringBuilder.ToString();

        StringBuilderPool.Release(stringBuilder);

        switch (logtype)
        {
            default:
            case LogType.Log:
                InternalLogInfo(str);
                break;
            case LogType.Warning:
                InternalLogWarning(str);
                break;

            case LogType.Assert:
            case LogType.Exception:
            case LogType.Error:
                InternalLogError(str);
                break;
        }
    }

    public static void Log(string message, bool displayOnScreen = false)
    {
        if (ThreadUtility.IsMainThread)
        {
            if (forwardToNativeUnityDebug)
                Debug.Log(message);
            else
                InternalLogInfo(message);

            if (displayOnScreen)
            {
                DebugScreenMessage.DisplayMessage(message);
            }
        }
        else
        {
            MainThreadService.AddMainThreadCallbackFromThread(() => Log(message + DELAYED_TO_MAIN_THREAD_SUFFIX, displayOnScreen));
        }
    }

    public static void LogError(string message, bool displayOnScreen = false)
    {
        if (ThreadUtility.IsMainThread)
        {
            if (forwardToNativeUnityDebug)
            {
                Debug.LogError(message);
            }
            else
            {
                string messageWithStack = CombineMessageAndStack(message, StackTraceUtility.ExtractStackTrace());
                InternalLogError(messageWithStack);
            }

            if (displayOnScreen)
            {
                DebugScreenMessage.DisplayMessage(message);
            }
        }
        else
        {
            MainThreadService.AddMainThreadCallbackFromThread(() => LogError(message + DELAYED_TO_MAIN_THREAD_SUFFIX, displayOnScreen));
        }
    }

    public static void LogWarning(string message, bool displayOnScreen = false)
    {
        if (ThreadUtility.IsMainThread)
        {
            if (forwardToNativeUnityDebug)
                Debug.LogWarning(message);
            else
                InternalLogWarning(message);

            if (displayOnScreen)
            {
                DebugScreenMessage.DisplayMessage(message);
            }
        }
        else
        {
            MainThreadService.AddMainThreadCallbackFromThread(() => Log(message + DELAYED_TO_MAIN_THREAD_SUFFIX, displayOnScreen));
        }
    }

    private static void InternalLogInfo(string message)
    {
        InternalLogX(message, "", GameConsole.LineColor.Normal);
    }

    private static void InternalLogError(string message)
    {
        InternalLogX(message, "[ERR]", GameConsole.LineColor.Error);
    }

    private static void InternalLogWarning(string message)
    {
        InternalLogX(message, "[WARN]", GameConsole.LineColor.Warning);
    }

    private static void InternalLogX(string message, string prefix, GameConsole.LineColor lineColor)
    {
        string result = ProcessMessage(message, prefix);

        GameConsole.Write(result, lineColor); // console GUI 

        if (logFile != null)
            logFile.WriteLine(result + "\n");
    }

    private static string CombineMessageAndStack(string message, string stack)
    {
        StringBuilder stringBuilder = StringBuilderPool.Take();
        stringBuilder.AppendLine(message);
        stringBuilder.AppendLine(stack);

        string str = stringBuilder.ToString();

        StringBuilderPool.Release(stringBuilder);

        return str;
    }

    private static string ProcessMessage(string message, string prefix = null)
    {
        StringBuilder stringBuilder = StringBuilderPool.Take();
        stringBuilder.Append('[');
        stringBuilder.Append(FrameService.FrameCount);
        stringBuilder.Append(']');

        if (!string.IsNullOrEmpty(prefix))
            stringBuilder.Append(prefix);

        stringBuilder.Append(message);

        string str = stringBuilder.ToString();

        StringBuilderPool.Release(stringBuilder);

        return str;
    }
}
