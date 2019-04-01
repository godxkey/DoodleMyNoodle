using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuickStartDefaultsWindow : EditorWindow
{
    QuickStartDefaultsContent content = new QuickStartDefaultsContent();

    void OnGUI()
    {
        content.OnGUI();
    }

    // Open the window
    [MenuItem("Tools/Pipeline/QuickStart Defaults")]
    public static void ShowWindow()
    {
        GetWindow<QuickStartDefaultsWindow>(false, "QuickStart Defaults", true);
    }
}


public class QuickStartDefaultsContent
{
    QuickStartAssets assets;
    const string assetPath = "Assets/Resources/QuickStartAssets.asset";

    public void OnGUI()
    {
        if (assets == null)
        {
            assets = AssetDatabase.LoadAssetAtPath<QuickStartAssets>(assetPath);
        }

        if (assets == null)
        {
            GUILayout.Label("Coult not load QuickStartAsset at " + assetPath);
            return;
        }


        // _________________________________________ Play Mode _________________________________________ //
        bool dirty = false;
        var playMode = (QuickStartSettings.PlayMode)EditorGUILayout.EnumPopup("Play Mode", assets.defaultSettings.playMode);
        if(playMode != assets.defaultSettings.playMode)
        {
            assets.defaultSettings.playMode = playMode;
            dirty = true;
        }

        // _________________________________________ Player Name _________________________________________ //
        string playerName = EditorGUILayout.TextField("Player Name", assets.defaultSettings.playerName);
        if (playerName != assets.defaultSettings.playerName)
        {
            assets.defaultSettings.playerName = playerName;
            dirty = true;
        }


        // _________________________________________ Server Name _________________________________________ //
        string serverName = EditorGUILayout.TextField("Server Name", assets.defaultSettings.serverName);
        if (serverName != assets.defaultSettings.serverName)
        {
            assets.defaultSettings.serverName = serverName;
            dirty = true;
        }


        // _________________________________________ Level _________________________________________ //
        string level = EditorGUILayout.TextField("Level", assets.defaultSettings.level);
        if(level != assets.defaultSettings.level)
        {
            assets.defaultSettings.level = level;
            dirty = true;
        }

        if (dirty)
        {
            EditorUtility.SetDirty(assets);
        }
    }
}