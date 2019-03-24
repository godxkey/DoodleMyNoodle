
namespace Internals.GameConsoleInterals
{
    public interface IGameConsoleUI
    {
        void Init();
        void Shutdown();
        void OutputString(string message, GameConsole.LineColor lineColor);
        bool IsOpen();
        void SetOpen(bool open);
        void ConsoleUpdate();
        void ConsoleLateUpdate();
    }
}