using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class LauncherWindow : EditorWindow
{
    LauncherWindowContent _content = new LauncherWindowContent();

    void OnGUI()
    {
        _content.OnGUI();
    }

    // Open the window
    [MenuItem("Tools/Pipeline/Extra/Launcher Window")]
    public static void ShowWindow()
    {
        GetWindow<LauncherWindow>(false, "Launcher", true);
    }

    void OnEnable()
    {
        _content.Load();
    }

    void OnDisable()
    {
        _content.Save();
    }
}


public class LauncherWindowContent
{
    ////////////////////////////////////////////////////////////////////////////////////////
    //      THIS WINDOW IS SUPER SIMPLIFIED FOR THE MOMENT!!!                                 
    ////////////////////////////////////////////////////////////////////////////////////////

    bool _headless;
    bool _useDefaultQuickstartSettings;
    string _extraArguments;

    public void Load()
    {
        _headless = EditorPrefs.GetBool("Launcher-headless", false);
        _useDefaultQuickstartSettings = EditorPrefs.GetBool("Launcher-useQuickstart", true);
        _extraArguments = EditorPrefs.GetString("Launcher-extraArgs");
    }

    public void Save()
    {
        EditorPrefs.SetBool("Launcher-headless", _headless);
        EditorPrefs.SetBool("Launcher-useQuickstart", _useDefaultQuickstartSettings);
        EditorPrefs.SetString("Launcher-extraArgs", _extraArguments);
    }

    public void OnGUI()
    {
        StringBuilder finalArguments = new StringBuilder();

        _useDefaultQuickstartSettings = EditorGUILayout.Toggle("Use QuickStart", _useDefaultQuickstartSettings);
        if (_useDefaultQuickstartSettings)
        {
            QuickStartSettings quickStartSettings = QuickStartAssets.instance.defaultSettings;

            finalArguments.Append("-playmode ");
            finalArguments.Append((int)quickStartSettings.playMode);
            finalArguments.Append(' ');

            if (quickStartSettings.level.IsNullOrEmpty() == false)
            {
                finalArguments.Append("-level ");
                finalArguments.Append(quickStartSettings.level);
                finalArguments.Append(' ');
            }

            if (quickStartSettings.serverName.IsNullOrEmpty() == false)
            {
                finalArguments.Append("-servername ");
                finalArguments.Append(quickStartSettings.serverName);
                finalArguments.Append(' ');
            }

            if (quickStartSettings.playerName.IsNullOrEmpty() == false)
            {
                finalArguments.Append("-playername ");
                finalArguments.Append(quickStartSettings.playerName);
                finalArguments.Append(' ');
            }
        }

        _headless = EditorGUILayout.Toggle("Headless", _headless);
        if (_headless)
        {
            finalArguments.Append("-batchmode -nographics ");
        }

        _extraArguments = EditorGUILayout.TextField("Extra arguments", _extraArguments);
        finalArguments.Append(_extraArguments);
        finalArguments.Append(' ');



        bool previousWordWrap = EditorStyles.textField.wordWrap;

        string finaleAgsStr = finalArguments.ToString();
        EditorGUILayout.LabelField("Final arguments");

        GUI.enabled = false;
        EditorStyles.textField.wordWrap = true;
        EditorGUILayout.TextArea(finaleAgsStr, GUILayout.Height(30));
        GUI.enabled = true;

        EditorStyles.textField.wordWrap = previousWordWrap;


        if (GUILayout.Button("Launch"))
        {
            RunBuild(finaleAgsStr);
        }

        if (GUILayout.Button("Kill All Processes"))
        {
            BuildTools.KillAllProcesses(PipelineSettings.AutoBuildExecutableName);
        }
    }

    static void RunBuild(string args)
    {
        var buildPath = PipelineSettings.AutoBuildPath;
        var buildExe = PipelineSettings.AutoBuildExecutableName;

        UnityEngine.Debug.Log("Starting " + buildExe + " in " + buildPath);

        var process = new Process();

        process.StartInfo.UseShellExecute = args.Contains("-batchmode");
        process.StartInfo.FileName = Application.dataPath + "/../" + buildPath + "/" + buildExe;
        process.StartInfo.Arguments = args;
        process.StartInfo.WorkingDirectory = buildPath;

        process.Start();
    }
}