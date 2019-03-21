using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateDebugPanel
{
    static bool panelVisible;

    [RuntimeInitializeOnLoadMethod]
    static void OnRuntimeMethodLoad() // Executed after scene is loaded and game is running
    {
        CoreServiceManager.AddInitializationCallback(() =>
        {
            UpdaterService.AddGUICallback(OnGUI);
            UpdaterService.AddUpdateCallback(OnUpdate);
        });
    }

    static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            panelVisible = !panelVisible;
        }
    }

    static void OnGUI()
    {
        if (panelVisible)
        {
            GUI.Box(new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height)), "");

            GUILayout.Label("GameState: " + GameStateManager.currentGameState);
            GameStateManager.currentGameState?.OnDebugPanelGUI();
        }
    }
}
