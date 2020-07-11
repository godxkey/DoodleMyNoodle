using UnityEngine;
using System.Collections.Generic;
using System;
using GameConsoleInterals;
using UnityEngineX;
using System.Collections.Concurrent;
using System.Reflection;
using CCC.ConfigVarInterals;

public class GameConsole
{
    public enum LineColor
    {
        Normal,
        Command,
        Warning,
        Error
    }

    static IGameConsoleUI s_consoleUI;
    static GameConsoleDatabase s_database = new GameConsoleDatabase();
    static int s_historyIndex = 0;
    static ConcurrentQueue<(int channelId, string condition, string stackTrace, LogType logType)> s_queuedLogs = new ConcurrentQueue<(int channelId, string condition, string stackTrace, LogType logType)>();
    private static bool s_init;

    internal static void SetUI(IGameConsoleUI consoleUI)
    {
        s_consoleUI?.Shutdown();
        s_consoleUI = consoleUI;

        if (s_consoleUI != null)
        {
            Log.Internals.LogMessageReceivedThreaded += OnLogMessageReceivedThreaded;
            s_consoleUI.Init(s_database);
        }
        else
        {
            Log.Internals.LogMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        }

        InitIfNeeded();
    }

    public static void InitIfNeeded()
    {
        if (s_init)
            return;
        s_init = true;

        PopulateAllInvokables();

        Write("Console ready", LineColor.Normal);
    }

    private static void PopulateAllInvokables()
    {
        List<GameConsoleInvokable> potentialInvokable = new List<GameConsoleInvokable>();
        foreach (MethodInfo method in TypeUtility.GetStaticMethodsWithAttribute(typeof(ConsoleCommandAttribute)))
        {
            potentialInvokable.Add(new GameConsoleCommand(method));
        }

        foreach (FieldInfo field in TypeUtility.GetStaticFieldsWithAttribute(typeof(ConsoleVarAttribute)))
        {
            potentialInvokable.Add(new GameConsoleField(field));
        }

        foreach (PropertyInfo property in TypeUtility.GetStaticPropertiesWithAttribute(typeof(ConsoleVarAttribute)))
        {
            potentialInvokable.Add(new GameConsoleProperty(property));
        }

        foreach (var invokable in potentialInvokable)
        {
            Type unsupportedParameterType = invokable.GetFirstUnsupportedParameter();

            if (unsupportedParameterType != null)
            {
                if (invokable is GameConsoleCommand)
                    Log.Warning($"Ignoring ConsoleCommand '{invokable.DisplayName}' because it has an unsupported parameter type: {unsupportedParameterType.GetPrettyName()}");
                else
                    Log.Warning($"Ignoring ConsoleVar '{invokable.DisplayName}' because it is of an unsupported type: {unsupportedParameterType.GetPrettyName()}");
                continue;
            }

            if (conflictsWithAnyInvokable(invokable))
            {
                if (invokable is GameConsoleCommand)
                    Log.Warning($"Ignoring ConsoleCommand '{invokable.DisplayName}' because its signature conflicts with another ConsoleVar or ConsoleCommand.");
                else
                    Log.Warning($"Ignoring ConsoleVar '{invokable.DisplayName}' because its signature conflicts with another ConsoleVar or ConsoleCommand.");
                continue;
            }

            s_database.Invokables.Add(invokable);
            s_database.InvokablesMap.Add(invokable.Name, invokable);
        }


        foreach (GameConsoleInvokable invokable in s_database.Invokables)
        {
            invokable.Init();
        }


        bool conflictsWithAnyInvokable(GameConsoleInvokable inv)
        {
            foreach (GameConsoleInvokable invokable in s_database.Invokables)
            {
                if (inv.ConflictsWith(invokable))
                {
                    return true;
                }
            }
            return false;
        }
    }

    private static void OnLogMessageReceivedThreaded(int channelId, string condition, string stackTrace, LogType logType)
    {
        if (ThreadUtility.IsMainThread)
        {
            if (s_consoleUI != null)
                s_consoleUI?.OutputLog(channelId, condition, stackTrace, logType);
        }
        else
        {
            s_queuedLogs.Enqueue((channelId, condition, stackTrace, logType));
        }
    }

    static void OutputString(string message, LineColor lineColor)
    {
        if (s_consoleUI != null)
            s_consoleUI.OutputString(message, lineColor);
    }

    public static void Write(string msg, LineColor lineColor)
    {
        InitIfNeeded();
        OutputString(msg, lineColor);
    }

    public static bool IsOpen()
    {
        return s_consoleUI != null ? s_consoleUI.IsOpen() : false;
    }

    public static void SetOpen(bool open)
    {
        s_consoleUI?.SetOpen(open);
    }

    public static void ConsoleUpdate()
    {
        InitIfNeeded();
        ProcessQueuedLogs();
        s_consoleUI?.ConsoleUpdate();

        while (s_database.PendingInvokes.Count > 0)
        {
            // Remove before executing as we may hit an 'exec' command that wants to insert commands
            var cmd = s_database.PendingInvokes[0];
            s_database.PendingInvokes.RemoveAt(0);
            ExecuteCommand(cmd, LineColor.Command);
        }
    }

    public static void ConsoleLateUpdate()
    {
        InitIfNeeded();
        ProcessQueuedLogs();

        s_consoleUI?.ConsoleLateUpdate();
    }

    private static void ProcessQueuedLogs()
    {
        var strBuilder = StringBuilderPool.Take();

        while (s_queuedLogs.TryDequeue(out (int channelId, string condition, string stackTrace, LogType logType) result))
        {
            strBuilder.Clear();
            strBuilder.Append("[Deferred to main thread] ");
            strBuilder.Append(result.condition);
            s_consoleUI.OutputLog(result.channelId, strBuilder.ToString(), result.stackTrace, result.logType);
        }

        StringBuilderPool.Release(strBuilder);
    }

    private static void ExecuteCommand(string command, LineColor lineColor)
    {
        List<string> tokens = GameConsoleParser.Tokenize(command);
        if (tokens.Count < 1)
            return;

        OutputString('>' + command, lineColor);
        string invokableName = tokens[0].ToLower();

        if (s_database.InvokablesMap.TryGetValue(invokableName, out GameConsoleInvokable invokable))
        {
            if (invokable.Enabled)
            {
                invokable.Invoke(tokens.GetRange(1, tokens.Count - 1).ToArray());
            }
            else
            {
                Log.Warning($"'{invokableName}' is disabled.");
            }
        }
        else if (ConfigVarService.Instance.configVarRegistry.ConfigVars.TryGetValue(invokableName, out ConfigVarBase configVar))
        {
            if (tokens.Count == 2)
            {
                configVar.Value = tokens[1];
            }
            else if (tokens.Count == 1)
            {
                // Print value
                OutputString(string.Format("{0} = {1}", configVar.name, configVar.Value), LineColor.Normal);
            }
            else
            {
                OutputString("Too many arguments", LineColor.Warning);
            }
        }
        else
        {
            OutputString("Unknown command: " + tokens[0], LineColor.Warning);
        }
    }

    public static void EnqueueCommandNoHistory(string command)
    {
        InitIfNeeded();
        s_database.PendingInvokes.Add(command);
    }

    public static void EnqueueCommand(string command)
    {
        InitIfNeeded();
        s_database.PushInHistory(command);
        s_historyIndex = -1;

        EnqueueCommandNoHistory(command);
    }

    public static string TabComplete(string prefix)
    {
        InitIfNeeded();
        // Look for possible tab completions
        List<string> matches = new List<string>();

        foreach (var c in s_database.InvokablesMap)
        {
            if (!c.Value.Enabled)
                continue;

            var name = c.Key;
            if (!name.StartsWith(prefix, true, null))
                continue;

            matches.Add(name);
        }

        foreach (var v in ConfigVarService.Instance.configVarRegistry.ConfigVars)
        {
            var name = v.Key;
            if (!name.StartsWith(prefix, true, null))
                continue;
            matches.Add(name);
        }

        if (matches.Count == 0)
            return prefix;

        // Look for longest common prefix
        int lcp = matches[0].Length;
        for (var i = 0; i < matches.Count - 1; i++)
        {
            lcp = Mathf.Min(lcp, commonPrefix(matches[i], matches[i + 1]));
        }
        prefix += matches[0].Substring(prefix.Length, lcp - prefix.Length);
        if (matches.Count > 1)
        {
            // write list of possible completions
            for (var i = 0; i < matches.Count; i++)
                GameConsole.Write(" " + matches[i], LineColor.Normal);
        }
        else
        {
            prefix += " ";
        }
        return prefix;

        // Returns length of largest common prefix of two strings
        int commonPrefix(string a, string b)
        {
            int minl = Mathf.Min(a.Length, b.Length);
            for (int i = 1; i <= minl; i++)
            {
                if (!a.StartsWith(b.Substring(0, i), true, null))
                    return i - 1;
            }
            return minl;
        }
    }

    public static string HistoryUp()
    {
        InitIfNeeded();
        return HistoryMove(1);
    }

    public static string HistoryDown()
    {
        InitIfNeeded();
        return HistoryMove(-1);
    }

    public static string HistoryDownCompletely()
    {
        InitIfNeeded();
        return HistoryMove(-s_database.History.Count);
    }

    private static string HistoryMove(int move)
    {
        s_historyIndex = Mathf.Clamp(s_historyIndex + move, -1, s_database.History.Count - 1);

        if (s_historyIndex == -1)
            return string.Empty;
        if (s_database.History.Count == 0)
            return string.Empty;

        return s_database.History[s_historyIndex];
    }

    public static void SetCommandOrVarEnabled(string command, bool enabled)
    {
        InitIfNeeded();

        command = command.ToLower();

        if (s_database.InvokablesMap.TryGetValue(command, out GameConsoleInvokable c))
        {
            c.Enabled = enabled;
        }
        else
        {
            Log.Error($"Command '{command}' does not exist.");
        }
    }

    [ConsoleCommand("help", "Show available commands")]
    static void Help()
    {
        OutputString("Available commands:", LineColor.Normal);

        foreach (var c in s_database.Invokables)
        {
            if (c.Enabled)
                OutputString(c.DisplayName + ": " + c.Description, LineColor.Normal);
        }
    }

    [ConsoleCommand("cvars", "Show available config-vars")]
    static void CVars()
    {
        var varNames = new List<string>(ConfigVarService.Instance.configVarRegistry.ConfigVars.Keys);
        varNames.Sort();
        foreach (var v in varNames)
        {
            var cv = ConfigVarService.Instance.configVarRegistry.ConfigVars[v];
            OutputString($"{cv.name} = {cv.Value}    // {cv.description}", LineColor.Normal);
        }
    }

    [ConsoleCommand("exec", "Execute commands stored in a text file")]
    static void Exec(string fileName, bool silent = false)
    {
        try
        {
            var lines = System.IO.File.ReadAllLines(fileName);
            s_database.PendingInvokes.InsertRange(0, lines);
            if (s_database.PendingInvokes.Count > 128)
            {
                s_database.PendingInvokes.Clear();
                OutputString("Command overflow. Flushing pending commands!!!", LineColor.Warning);
            }
        }
        catch (Exception e)
        {
            if (!silent)
                OutputString("Exec failed: " + e.Message, LineColor.Error);
        }
    }
}
