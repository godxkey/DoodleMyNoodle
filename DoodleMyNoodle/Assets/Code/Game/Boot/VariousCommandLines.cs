using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal static class VariousCommandLines
{
    static bool done = false;

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad() // Executed after scene is loaded and game is running
    {
        if (done == false)
        {
            done = true;
            RegisterCommands();
        }
    }

    static void RegisterCommands()
    {
        GameConsole.AddCommand("menu", Cmd_Menu, "Go to menu");
    }

    static void Cmd_Menu(string[] args)
    {
        QuickStart.StartFromScratch();
    }
}
