
using UnityEngine;

namespace GameConsoleInterals
{
    internal interface IGameConsoleUI
    {
        void Init(GameConsoleDatabase database);
        void Shutdown();
        void OutputLog(int channelId, string condition, string stackTrace, LogType logType);
        void OutputString(string message, GameConsole.LineColor lineColor);
        bool IsOpen();
        void SetOpen(bool open);
        void ConsoleUpdate();
        void ConsoleLateUpdate();
    }
}