using System;
using UnityEngine;

//
// Logging of messages
//
// There are three different types of messages:
//
// Debug.Log/Warn/Error coming from unity (or code, e.g. packages, not using GameDebug)
//    These get caught here and sent onto the console and into our log file
// GameDebug.Log/Warn/Error coming from game
//    These gets sent onto the console and into our log file
//    *IF* we are in editor, they are also sent to Debug.* so they show up in editor Console window
// Console.Write
//    Only used for things that should not be logged. Typically reponses to user commands. Only shown on Console.
//

public static class CDebug
{
    static bool forwardToNativeUnityDebug = true;
    static System.IO.StreamWriter logFile = null;

    public static void Init(string logfilePath, string logBaseName)
    {
        forwardToNativeUnityDebug = Application.isEditor;
        Application.logMessageReceived += LogCallback;

        // Try creating logName; attempt a number of suffixxes
        string name = "";
        for (var i = 0; i < 10; i++)
        {
            name = logBaseName + (i == 0 ? "" : "_" + i) + ".log";
            try
            {
                logFile = System.IO.File.CreateText(logfilePath + "/" + name);
                logFile.AutoFlush = true;
                break;
            }
            catch
            {
                name = "<none>";
            }
        }
        CDebug.Log("GameDebug initialized. Logging to " + logfilePath + "/" + name);
    }

    public static void Shutdown()
    {
        Application.logMessageReceived -= LogCallback;
        if (logFile != null)
            logFile.Close();
        logFile = null;
    }

    static void LogCallback(string message, string stack, LogType logtype)
    {
        switch (logtype)
        {
            default:
            case LogType.Log:
                CDebug._Log(message);
                break;
            case LogType.Warning:
                CDebug._LogWarning(message);
                break;
            case LogType.Error:
                CDebug._LogError(message);
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
