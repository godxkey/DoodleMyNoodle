
namespace Internals.GameConsoleInterals
{
    public class GameConsoleNullUI : IGameConsoleUI
    {
        public void ConsoleUpdate()
        {
        }

        public void ConsoleLateUpdate()
        {
        }

        public void Init()
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
    }
}