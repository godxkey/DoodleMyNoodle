using System.Collections.Generic;
using UnityEngineX;

internal class GameConsoleDatabase
{
    public Dictionary<string, GameConsoleInvokable> InvokablesMap = new Dictionary<string, GameConsoleInvokable>();
    public List<GameConsoleInvokable> Invokables = new List<GameConsoleInvokable>();
    
    public List<string> EnabledGroups = new List<string>();
    public List<string> PendingInvokes = new List<string>();
    public List<(GameConsoleInvokable invokable, string invoke)> BufferedInvokables = new List<(GameConsoleInvokable, string)>();


    const int HISTORY_COUNT = 50;
    public List<string> History = new List<string>(HISTORY_COUNT);

    public void PushInHistory(string command)
    {
        while (History.Count >= HISTORY_COUNT)
            History.RemoveLast();
        History.Insert(0, command);
    }
}
