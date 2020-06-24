using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

internal static class VariousCommandLines
{
    [Command(Description = "Go to menu")]
    static void Menu()
    {
        GameStateManager.TransitionToState(QuickStartAssets.instance.rootMenu);
    }
}
