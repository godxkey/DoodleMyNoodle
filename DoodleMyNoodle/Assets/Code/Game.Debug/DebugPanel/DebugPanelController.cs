using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPanelController
{
    static bool s_panelVisible;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void StaticReset()
    {
        s_panelVisible = false;
    }

    [Updater.StaticUpdateMethod(UpdateType.Update)]
    static void OnUpdate()
    {
        if (Input.GetKeyDown(KeyCode.F2))
        {
            s_panelVisible = !s_panelVisible;
        }
    }

    [Updater.StaticUpdateMethod(UpdateType.GUI)]
    static void OnGUI()
    {
        if (!DebugPanelStyles.initialized)
        {
            DebugPanelStyles.Initialize();
        }

        if (s_panelVisible)
        {
            DebugPanelStyles.ApplyStyles();

            Rect screenRect = new Rect(Vector2.zero, new Vector2(Screen.width, Screen.height));

            GUI.Box(screenRect, "");

            var stdColor = GUI.color;

            GUILayout.BeginArea(new Rect(Vector2.zero, new Vector2(350, Screen.height)));

            DebugPanel[] panels = DebugPanelRegistry.RegisteredPanels;

            for (int i = 0; i < panels.Length; i++)
            {
                if (panels[i].CanBeDisplayed)
                {
                    if (!panels[i].IsDisplayed)
                    {
                        panels[i].IsDisplayed = true;
                        panels[i].OnStartDisplay();
                    }

                    GUILayout.Label(panels[i].Title, DebugPanelStyles.title);
                    panels[i].OnGUI();
                    GUILayout.Space(12);
                    GUI.color = stdColor;
                }
                else
                {
                    if (panels[i].IsDisplayed)
                    {
                        panels[i].IsDisplayed = false;
                        panels[i].OnStopDisplay();
                    }
                }
            }

            GUILayout.EndArea();

            DebugPanelStyles.RevertStyles();
        }
    }
}
