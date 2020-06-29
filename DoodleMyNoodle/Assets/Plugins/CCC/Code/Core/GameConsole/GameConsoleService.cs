using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameConsoleInterals
{
    internal class GameConsoleService : MonoCoreService<GameConsoleService>
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

                GameConsole.SetUI(consoleUI);
            }
            else
            {
                GameObject consoleGUIGameObject = Instantiate((GameObject)Resources.Load(consoleGUIResource));
                DontDestroyOnLoad(consoleGUIGameObject);

                GameConsoleGUI consoleGUI = consoleGUIGameObject.GetComponent<GameConsoleGUI>();
                GameConsole.SetUI(consoleGUI);
                GameConsole.SetOpen(openConsoleAtLaunch);
            }

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

        protected override void OnDestroy()
        {
            GameConsole.SetUI(null);
            base.OnDestroy();
        }
    }
}