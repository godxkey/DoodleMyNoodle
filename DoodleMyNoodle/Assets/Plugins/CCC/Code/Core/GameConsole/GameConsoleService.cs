using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Internals.GameConsoleInterals
{
    public class GameConsoleService : MonoCoreService<GameConsoleService>
    {
        [SerializeField]
        bool openConsoleAtLaunch = true;
        [SerializeField]
        string consoleGUIResource;

        public override void Initialize(Action<ICoreService> onComplete)
        {
            var commandLineArgs = new List<string>(Environment.GetCommandLineArgs());

            bool isHeadless = commandLineArgs.Contains("-batchmode");
            bool consoleRestoreFocus = commandLineArgs.Contains("-consolerestorefocus");

            if (isHeadless)
            {
                string consoleTitle = Application.productName + " Console";
                consoleTitle += " [" + System.Diagnostics.Process.GetCurrentProcess().Id + "]";

                GameConsoleTextWin consoleUI = new GameConsoleTextWin(consoleTitle, consoleRestoreFocus);

                GameConsole.Init(consoleUI);
            }
            else
            {
                GameObject consoleGUIGameObject = Instantiate((GameObject)Resources.Load(consoleGUIResource));
                DontDestroyOnLoad(consoleGUIGameObject);

                GameConsoleGUI consoleGUI = consoleGUIGameObject.GetComponent<GameConsoleGUI>();
                GameConsole.Init(consoleGUI);
                GameConsole.SetOpen(openConsoleAtLaunch);
            }

            GameConsole.AddCommand("quit", CmdQuit, "Quits");

            onComplete(this);
        }

        void Update()
        {
            GameConsole.ConsoleUpdate();
        }

        void LateUpdate()
        {
            GameConsole.ConsoleLateUpdate();
        }

        void CmdQuit(string[] arguments)
        {
            Application.Quit();
        }

        protected override void OnDestroy()
        {
            GameConsole.Shutdown();
            base.OnDestroy();
        }
    }
}