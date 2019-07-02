using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelController
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


    static DebugPanelGameState panel_gameState = new DebugPanelGameState();
    static DebugPanelPlayerRepertoire panel_playerRepertoire = new DebugPanelPlayerRepertoire();


    static DebugPanel[] panels = new DebugPanel[]
    {
        new DebugPanelGameState(),
        new DebugPanelPlayerRepertoire(),
        new DebugPanelClientSimController()
    };



    static void OnGUI()
    {
        if (!DebugPanelStyles.initialized)
        {
            DebugPanelStyles.Initialize();
        }

        if (panelVisible)
        {
            DebugPanelStyles.ApplyStyles();

            Rect screenRect = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));

            GUI.Box(screenRect, "");

            var stdColor = GUI.color;

            GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(350, Screen.height)));

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].CanBeDisplayed)
                {
                    GUILayout.Label(panels[i].Title, DebugPanelStyles.title);
                    panels[i].OnGUI();
                    GUILayout.Space(12);
                    GUI.color = stdColor;
                }
            }

            GUILayout.EndArea();

            DebugPanelStyles.RevertStyles();
        }
    }
}
