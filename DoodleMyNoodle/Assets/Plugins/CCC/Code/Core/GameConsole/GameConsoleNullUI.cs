
using UnityEngine;

namespace GameConsoleInterals
{
    internal class GameConsoleNullUI : IGameConsoleUI
    {
        public void ConsoleUpdate()
        {
        }

        public void ConsoleLateUpdate()
        {
        }

        public void Init(GameConsoleDatabase database)
        {
        }

        public void Shutdown()
        {
        }

        public bool IsOpen()
        {
            return false;
        }

        public void OutputString(string message, GameConsole.LineColor lineColor)
        {
        }

        public void SetOpen(bool open)
        {
        }

        public void OutputLog(int channelId, string condition, string stackTrace, LogType logType)
        {
        }
    }
}