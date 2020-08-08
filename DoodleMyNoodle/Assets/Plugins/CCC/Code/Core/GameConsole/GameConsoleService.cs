using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GameConsoleInterals
{
    internal class GameConsoleService : MonoCoreService<GameConsoleService>
    {
        [SerializeField] private bool _openConsoleAtLaunch = false;
        [SerializeField] private GameObject _consoleGUIPrefab;


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
                GameObject consoleGUIGameObject = Instantiate(_consoleGUIPrefab);
                DontDestroyOnLoad(consoleGUIGameObject);

                GameConsoleGUI consoleGUI = consoleGUIGameObject.GetComponent<GameConsoleGUI>();
                GameConsole.SetUI(consoleGUI);
                GameConsole.SetOpen(_openConsoleAtLaunch);
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