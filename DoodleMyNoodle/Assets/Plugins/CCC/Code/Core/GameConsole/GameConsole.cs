using UnityEngine;
using System.Collections.Generic;
using System;
using Internals.GameConsoleInterals;

public class GameConsole
{
    public enum LineColor
    {
        Normal,
        Command,
        Warning,
        Error
    }

    static IGameConsoleUI s_ConsoleUI;

    public static void Init(IGameConsoleUI consoleUI)
    {
        if (s_ConsoleUI != null)
        {
            DebugService.LogError("Initializing the Console for a second time.");
            return;
        }

        s_ConsoleUI = consoleUI;
        s_ConsoleUI.Init();
        AddCommand("help", Cmd_Help, "Show available commands");
        AddCommand("vars", Cmd_Vars, "Show available variables");
        AddCommand("exec", Cmd_Exec, "Executes commands from file");
        Write("Console ready", LineColor.Normal);
    }

    public static void Shutdown()
    {
        s_ConsoleUI.Shutdown();
    }

    static void OutputString(string message, LineColor lineColor)
    {
        if (s_ConsoleUI != null)
            s_ConsoleUI.OutputString(message, lineColor);
    }

    static string lastMsg = "";
    static double timeLastMsg;
    public static void Write(string msg, LineColor lineColor)
    {
        // Have to condition on cvar being null as this may run before cvar system is initialized
        if (consoleShowLastLine != null && consoleShowLastLine.IntValue > 0)
        {
            lastMsg = msg;
            timeLastMsg = Time.time; // Game.frameTime;
        }
        OutputString(msg, lineColor);
    }

    public static void AddCommand(string name, Action<string[]> method, string description, int tag = 0)
    {
        name = name.ToLower();
        if (s_Commands.ContainsKey(name))
        {
            OutputString("Cannot add command " + name + " twice", LineColor.Error);
            return;
        }
        s_Commands.Add(name, new ConsoleCommand(name, method, description, tag));
    }

    public static bool RemoveCommand(string name)
    {
        return s_Commands.Remove(name.ToLower());
    }

    public static void RemoveCommandsWithTag(int tag)
    {
        var removals = new List<string>();
        foreach (var c in s_Commands)
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
        return s_ConsoleUI.IsOpen();
    }

    public static void SetOpen(bool open)
    {
        s_ConsoleUI.SetOpen(open);
    }

    public static void ConsoleUpdate()
    {
        //var lastMsgTime = Time.time - timeLastMsg;
        //if (lastMsgTime < 1.0)
        //    DebugOverlay.Write(0, 0, lastMsg);

        s_ConsoleUI.ConsoleUpdate();

        while (s_PendingCommands.Count > 0)
        {
            // Remove before executing as we may hit an 'exec' command that wants to insert commands
            var cmd = s_PendingCommands[0];
            s_PendingCommands.RemoveAt(0);
            ExecuteCommand(cmd, LineColor.Command);
        }
    }

    public static void ConsoleLateUpdate()
    {
        s_ConsoleUI.ConsoleLateUpdate();
    }

    static void SkipWhite(string input, ref int pos)
    {
        while (pos < input.Length && " \t".IndexOf(input[pos]) > -1)
        {
            pos++;
        }
    }

    static string ParseQuoted(string input, ref int pos)
    {
        pos++;
        int startPos = pos;
        while (pos < input.Length)
        {
            if (input[pos] == '"' && input[pos - 1] != '\\')
            {
                pos++;
                return input.Substring(startPos, pos - startPos - 1);
            }
            pos++;
        }
        return input.Substring(startPos);
    }

    static string Parse(string input, ref int pos)
    {
        int startPos = pos;
        while (pos < input.Length)
        {
            if (" \t".IndexOf(input[pos]) > -1)
            {
                return input.Substring(startPos, pos - startPos);
            }
            pos++;
        }
        return input.Substring(startPos);
    }

    static List<string> Tokenize(string input)
    {
        var pos = 0;
        var res = new List<string>();
        var c = 0;
        while (pos < input.Length && c++ < 10000)
        {
            SkipWhite(input, ref pos);
            if (pos == input.Length)
                break;

            if (input[pos] == '"' && (pos == 0 || input[pos - 1] != '\\'))
            {
                res.Add(ParseQuoted(input, ref pos));
            }
            else
                res.Add(Parse(input, ref pos));
        }
        return res;
    }

    public static void ExecuteCommand(string command, LineColor lineColor)
    {
        var tokens = Tokenize(command);
        if (tokens.Count < 1)
            return;

        OutputString('>' + command, lineColor);
        var commandName = tokens[0].ToLower();

        ConsoleCommand consoleCommand;
        CCC.ConfigVarInterals.ConfigVarBase configVar;

        if (s_Commands.TryGetValue(commandName, out consoleCommand))
        {
            var arguments = tokens.GetRange(1, tokens.Count - 1).ToArray();
            consoleCommand.method(arguments);
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

    static void Cmd_Help(string[] arguments)
    {
        OutputString("Available commands:", LineColor.Normal);

        foreach (var c in s_Commands)
            OutputString(c.Value.name + ": " + c.Value.description, LineColor.Normal);
    }

    static void Cmd_Vars(string[] arguments)
    {
        var varNames = new List<string>(ConfigVarService.Instance.configVarRegistry.ConfigVars.Keys);
        varNames.Sort();
        foreach (var v in varNames)
        {
            var cv = ConfigVarService.Instance.configVarRegistry.ConfigVars[v];
            OutputString(string.Format("{0} = {1}", cv.name, cv.Value), LineColor.Normal);
        }
    }

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
            s_PendingCommands.InsertRange(0, lines);
            if (s_PendingCommands.Count > 128)
            {
                s_PendingCommands.Clear();
                OutputString("Command overflow. Flushing pending commands!!!", LineColor.Warning);
            }
        }
        catch (Exception e)
        {
            if(!silent)
                OutputString("Exec failed: " + e.Message, LineColor.Error);
        }
    }

    public static void EnqueueCommandNoHistory(string command)
    {
        //CDebug.Log("cmd: " + command); // useless log that seems to fills the console for nothing
        s_PendingCommands.Add(command);
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

        foreach (var c in s_Commands)
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

    [ConfigVar(name: "config.showlastline", defaultValue: "0", description: "Show last logged line briefly at top of screen")]
    static ConfigVar consoleShowLastLine;

    static List<string> s_PendingCommands = new List<string>();
    static Dictionary<string, ConsoleCommand> s_Commands = new Dictionary<string, ConsoleCommand>();
    const int k_HistoryCount = 50;
    static string[] s_History = new string[k_HistoryCount];
    static int s_HistoryNextIndex = 0;
    static int s_HistoryIndex = 0;
}
