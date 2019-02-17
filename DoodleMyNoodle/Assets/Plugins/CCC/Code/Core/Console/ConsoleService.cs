using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConsoleService : MonoCoreService<ConsoleService>
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

            ConsoleTextWin consoleUI = new ConsoleTextWin(consoleTitle, consoleRestoreFocus);

            Console.Init(consoleUI);
        }
        else
        {
            GameObject consoleGUIGameObject = Instantiate((GameObject)Resources.Load(consoleGUIResource));
            DontDestroyOnLoad(consoleGUIGameObject);

            ConsoleGUI consoleGUI = consoleGUIGameObject.GetComponent<ConsoleGUI>();
            Console.Init(consoleGUI);
            Console.SetOpen(openConsoleAtLaunch);
        }

        Console.AddCommand("quit", CmdQuit, "Quits");

        onComplete(this);
    }

    void Update()
    {
        Console.ConsoleUpdate();
    }

    void LateUpdate()
    {
        Console.ConsoleLateUpdate();
    }

    void CmdQuit(string[] arguments)
    {
        Application.Quit();
    }

    protected override void OnDestroy()
    {
        Console.Shutdown();
        base.OnDestroy();
    }
}
