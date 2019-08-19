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

            DebugPanel[] panels = DebugPanelRegistry.registeredPanels;

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].canBeDisplayed)
                {
                    if (!panels[i].isDisplayed)
                    {
                        panels[i].isDisplayed = true;
                        panels[i].OnStartDisplay();
                    }

                    GUILayout.Label(panels[i].title, DebugPanelStyles.title);
                    panels[i].OnGUI();
                    GUILayout.Space(12);
                    GUI.color = stdColor;
                }
                else
                {
                    if (panels[i].isDisplayed)
                    {
                        panels[i].isDisplayed = false;
                        panels[i].OnStopDisplay();
                    }
                }
            }

            GUILayout.EndArea();

            DebugPanelStyles.RevertStyles();
        }
    }
}
