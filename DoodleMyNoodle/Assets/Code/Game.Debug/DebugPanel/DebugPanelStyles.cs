using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugPanelStyles
{
    public static bool initialized { get; private set; }
    public static void Initialize()
    {
        initialized = true;

        title = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Bold,
            fontSize = 18
        };

        text = new GUIStyle(GUI.skin.label)
        {
            fontStyle = FontStyle.Normal,
            fontSize = 12
        };
        text.margin.bottom = 0;
        text.margin.top = 0;
        text.padding.bottom = 0;
        text.padding.top = 0;

        boldText = new GUIStyle(text)
        {
            fontStyle = FontStyle.Bold
        };
    }


    public static void ApplyStyles()
    {
        // save previous
        prev_label = GUI.skin.label;

        // apply new
        GUI.skin.label = text;
    }

    public static void RevertStyles()
    {
        // restore previous
        GUI.skin.label = prev_label;
    }

    public static GUIStyle title;
    public static GUIStyle boldText;
    public static GUIStyle text;


    static GUIStyle prev_label;
}
