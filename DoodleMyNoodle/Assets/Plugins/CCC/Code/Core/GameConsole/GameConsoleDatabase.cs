using System.Collections.Generic;
using UnityEngineX;

public class GameConsoleDatabase
{
    public List<string> PendingCommands = new List<string>();
    public Dictionary<string, GameConsoleCommand> CommandsMap = new Dictionary<string, GameConsoleCommand>();
    public List<GameConsoleCommand> Commands = new List<GameConsoleCommand>();


    const int HISTORY_COUNT = 50;
    public List<string> History = new List<string>(HISTORY_COUNT);

    public void PushInHistory(string command)
    {
        while (History.Count >= HISTORY_COUNT)
            History.RemoveLast();
        History.Insert(0, command);
    }
}
