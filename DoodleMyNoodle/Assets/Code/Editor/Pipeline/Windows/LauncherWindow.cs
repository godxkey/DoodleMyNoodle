using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class LauncherWindow : EditorWindow
{
    LauncherWindowContent content = new LauncherWindowContent();

    void OnGUI()
    {
        content.OnGUI();
    }

    // Open the window
    [MenuItem("Tools/Pipeline/Launcher #&l")]
    public static void ShowWindow()
    {
        GetWindow<LauncherWindow>(false, "Launcher", true);
    }
}


public class LauncherWindowContent
{
    ////////////////////////////////////////////////////////////////////////////////////////
    //      THIS WINDOW IS SUPER SIMPLIFIED FOR THE MOMENT!!!                                 
    ////////////////////////////////////////////////////////////////////////////////////////

    public void OnGUI()
    {
        if (GUILayout.Button("Launch Server"))
        {
            RunBuild("-batchmode -nographics");
        }

        if (GUILayout.Button("Launch Client"))
        {
            RunBuild("");
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