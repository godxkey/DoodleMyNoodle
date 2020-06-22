using UnityEngine;
using System.Collections.Generic;
using System;
using Internals.GameConsoleInterals;
using UnityEngineX;
using System.Collections.Concurrent;
using System.Reflection;

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
    static ConcurrentQueue<(int channelId, string condition, string stackTrace, LogType logType)> s_queuedLogs = new ConcurrentQueue<(int channelId, string condition, string stackTrace, LogType logType)>();

    public static void Init(IGameConsoleUI consoleUI)
    {
        if (s_consoleUI != null)
        {
            Log.Error("Initializing the Console for a second time.");
            return;
        }

        Log.Internals.LogMessageReceivedThreaded += OnLogMessageReceivedThreaded;

        s_consoleUI = consoleUI;
        s_consoleUI.Init();

        PopulateAllCommands();

        AddCommand("help", Cmd_Help, "Show available commands");
        AddCommand("vars", Cmd_Vars, "Show available variables");
        AddCommand("exec", Cmd_Exec, "Executes commands from file");
        Write("Console ready", LineColor.Normal);
    }

    private static void PopulateAllCommands()
    {
        var commandMethods = TypeUtility.GetStaticMethodsWithAttribute(typeof(CommandAttribute));

        foreach (MethodInfo method in commandMethods)
        {
            if (method.IsAsync())
            {
                Log.Warning($"Ignoring command {method.Name} because it is async.");
                continue;
            }

            GameConsoleCommand command = new GameConsoleCommand(method);

            bool conflict = false;
            foreach (GameConsoleCommand otherCommand in s_commands)
            {
                if (command.ConflictsWith(otherCommand))
                {
                    conflict = true;
                    break;
                }
            }

            if (conflict)
            {
                Log.Warning($"Ignoring command {method.Name} because it conflicts with an already existing command.");
                continue;
            }


            Log.Info($"Adding command {command.Name}");
            s_commands.Add(command);
        }
    }

    public static void Shutdown()
    {
        Log.Internals.LogMessageReceivedThreaded -= OnLogMessageReceivedThreaded;
        s_consoleUI.Shutdown();
    }

    private static void OnLogMessageReceivedThreaded(int channelId, string condition, string stackTrace, LogType logType)
    {
        if (ThreadUtility.IsMainThread)
        {
            if (s_consoleUI != null)
                s_consoleUI.OutputLog(channelId, condition, stackTrace, logType);
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
        OutputString(msg, lineColor);
    }

    /// <summary>
    /// Name Guideline: Separate terms by . Separate words of a term by _
    /// <para/>
    /// E.g: inventory.drop_item
    /// </summary>
    public static void AddCommand(string name, Action<string[]> method, string description)
    {
        name = name.ToLower();
        if (s_commandsMap.ContainsKey(name))
        {
            OutputString("Cannot add command " + name + " twice", LineColor.Error);
            return;
        }
        s_commandsMap.Add(name, new ConsoleCommand(name, method, description, 0));
    }

    public static bool RemoveCommand(string name)
    {
        return s_commandsMap.Remove(name.ToLower());
    }

    public static void RemoveCommandsWithTag(int tag)
    {
        var removals = new List<string>();
        foreach (var c in s_commandsMap)
        {
            if (c.Value.tag == tag)
                removals.Add(c.Key);
        }
        foreach (var c in removals)
            RemoveCommand(c);
    }

    public static void ProcessCommandLineArguments(string[] arguments)
    {
        // Process arguments that have '+' prefix as console commands. Ignore all other arguments

        OutputString("ProcessCommandLineArguments: " + string.Join(" ", arguments), LineColor.Normal);

        var commands = new List<string>();

        foreach (var argument in arguments)
        {
            var newCommandStarting = argument.StartsWith("+") || argument.StartsWith("-");

            // Skip leading arguments before we have seen '-' or '+'
            if (commands.Count == 0 && !newCommandStarting)
                continue;

            if (newCommandStarting)
                commands.Add(argument);
            else
                commands[commands.Count - 1] += " " + argument;
        }

        foreach (var command in commands)
        {
            if (command.StartsWith("+"))
                EnqueueCommandNoHistory(command.Substring(1));
        }
    }

    public static bool IsOpen()
    {
        return s_consoleUI.IsOpen();
    }

    public static void SetOpen(bool open)
    {
        s_consoleUI.SetOpen(open);
    }

    public static void ConsoleUpdate()
    {
        ProcessQueuedLogs();
        s_consoleUI.ConsoleUpdate();

        while (s_pendingCommands.Count > 0)
        {
            // Remove before executing as we may hit an 'exec' command that wants to insert commands
            var cmd = s_pendingCommands[0];
            s_pendingCommands.RemoveAt(0);
            ExecuteCommand(cmd, LineColor.Command);
        }
    }

    public static void ConsoleLateUpdate()
    {
        ProcessQueuedLogs();

        s_consoleUI.ConsoleLateUpdate();
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

    public static void ExecuteCommand(string command, LineColor lineColor)
    {
        List<string> tokens = GameConsoleParser.Tokenize(command);
        if (tokens.Count < 1)
            return;

        OutputString('>' + command, lineColor);
        string commandName = tokens[0].ToLower();

        ConsoleCommand consoleCommand;
        CCC.ConfigVarInterals.ConfigVarBase configVar;

        if (s_commandsMap.TryGetValue(commandName, out consoleCommand))
        {
            var arguments = tokens.GetRange(1, tokens.Count - 1).ToArray();
            consoleCommand.method(arguments);
            Log.Info($"cmd: {command}");
        }
        else if (ConfigVarService.Instance.configVarRegistry.ConfigVars.TryGetValue(commandName, out configVar))
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

    [Command]
    static void Cmd_Help(string[] arguments)
    {
        OutputString("Available commands:", LineColor.Normal);

        foreach (var c in s_commandsMap)
            OutputString(c.Value.name + ": " + c.Value.description, LineColor.Normal);
    }

    [Command]
    static void Cmd_Vars(string[] arguments)
    {
        var varNames = new List<string>(ConfigVarService.Instance.configVarRegistry.ConfigVars.Keys);
        varNames.Sort();
        foreach (var v in varNames)
        {
            var cv = ConfigVarService.Instance.configVarRegistry.ConfigVars[v];
            OutputString($"{cv.name} = {cv.Value}    // {cv.description}", LineColor.Normal);
        }
    }

    [Command]
    static void Cmd_Exec(string[] arguments)
    {
        bool silent = false;
        string filename = "";
        if (arguments.Length == 1)
        {
            filename = arguments[0];
        }
        else if (arguments.Length == 2 && arguments[0] == "-s")
        {
            silent = true;
            filename = arguments[1];
        }
        else
        {
            OutputString("Usage: exec [-s] <filename>", LineColor.Normal);
            return;
        }

        try
        {
            var lines = System.IO.File.ReadAllLines(filename);
            s_pendingCommands.InsertRange(0, lines);
            if (s_pendingCommands.Count > 128)
            {
                s_pendingCommands.Clear();
                OutputString("Command overflow. Flushing pending commands!!!", LineColor.Warning);
            }
        }
        catch (Exception e)
        {
            if (!silent)
                OutputString("Exec failed: " + e.Message, LineColor.Error);
        }
    }

    public static void EnqueueCommandNoHistory(string command)
    {
        s_pendingCommands.Add(command);
    }

    public static void EnqueueCommand(string command)
    {
        s_History[s_HistoryNextIndex % k_HistoryCount] = command;
        s_HistoryNextIndex++;
        s_HistoryIndex = s_HistoryNextIndex;

        EnqueueCommandNoHistory(command);
    }


    public static string TabComplete(string prefix)
    {
        // Look for possible tab completions
        List<string> matches = new List<string>();

        foreach (var c in s_commandsMap)
        {
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
            lcp = Mathf.Min(lcp, CommonPrefix(matches[i], matches[i + 1]));
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
    }

    public static string HistoryUp(string current)
    {
        if (s_HistoryIndex == 0 || s_HistoryNextIndex - s_HistoryIndex >= k_HistoryCount - 1)
            return "";

        if (s_HistoryIndex == s_HistoryNextIndex)
        {
            s_History[s_HistoryIndex % k_HistoryCount] = current;
        }

        s_HistoryIndex--;

        return s_History[s_HistoryIndex % k_HistoryCount];
    }

    public static string HistoryDown()
    {
        if (s_HistoryIndex == s_HistoryNextIndex)
            return "";

        s_HistoryIndex++;

        return s_History[s_HistoryIndex % k_HistoryCount];
    }

    // Returns length of largest common prefix of two strings
    static int CommonPrefix(string a, string b)
    {
        int minl = Mathf.Min(a.Length, b.Length);
        for (int i = 1; i <= minl; i++)
        {
            if (!a.StartsWith(b.Substring(0, i), true, null))
                return i - 1;
        }
        return minl;
    }

    class ConsoleCommand
    {
        public string name;
        public Action<string[]> method;
        public string description;
        public int tag;

        public ConsoleCommand(string name, Action<string[]> method, string description, int tag)
        {
            this.name = name;
            this.method = method;
            this.description = description;
            this.tag = tag;
        }
    }

    static List<string> s_pendingCommands = new List<string>();
    static Dictionary<string, ConsoleCommand> s_commandsMap = new Dictionary<string, ConsoleCommand>();
    static List<GameConsoleCommand> s_commands = new List<GameConsoleCommand>();
    const int k_HistoryCount = 50;
    static string[] s_History = new string[k_HistoryCount];
    static int s_HistoryNextIndex = 0;
    static int s_HistoryIndex = 0;
}
