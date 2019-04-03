using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BuildWindow : EditorWindow
{
    BuildWindowContent content = new BuildWindowContent();

    void OnGUI()
    {
        content.OnGUI();
    }

    // Open the window
    [MenuItem("Tools/Pipeline/Extra/Build Window")]
    public static void ShowWindow()
    {
        GetWindow<BuildWindow>(false, "Build", true);
    }
}


public class BuildWindowContent
{
    BuildOptionsGUIData buildOptionsGUIData = new BuildOptionsGUIData()
    {
        buildScriptsOnly = false,
        developmentBuild = true,
        allowDebugging = true
    };

    public void OnGUI()
    {
        DrawBuildTarget();
        DrawBuildOptions();
        DrawBuildButtons();
        DrawOpenBuildFolder();
    }

    void DrawBuildOptions()
    {
        buildOptionsGUIData.developmentBuild = EditorGUILayout.Toggle("Development build", buildOptionsGUIData.developmentBuild);
        buildOptionsGUIData.allowDebugging = EditorGUILayout.Toggle("Allow debugging", buildOptionsGUIData.allowDebugging);
    }

    void DrawBuildTarget()
    {
        GUILayout.Label("Building for: " + EditorUserBuildSettings.activeBuildTarget.ToString(), GUILayout.ExpandWidth(false));
    }

    void DrawBuildButtons()
    {
        GUILayout.BeginHorizontal();
        bool buildGame = false;
        if (GUILayout.Button("Build game"))
        {
            buildGame = true;
            buildOptionsGUIData.buildScriptsOnly = false;
        }
        if (GUILayout.Button("Build ONLY scripts"))
        {
            buildGame = true;
            buildOptionsGUIData.buildScriptsOnly = true;
        }
        GUILayout.EndHorizontal();

        if (buildGame)
        {
            KillProcessesAndStopEditor();
            string[] scenes = GetAllEnabledScenes();

            BuildTools.BuildGame(
                PipelineSettings.AutoBuildPath,                // path
                PipelineSettings.AutoBuildExecutableName,      // exec name
                buildOptionsGUIData.ProduceBuildOptions(),  // build options
                "AutoBuild",                                // build id
                GetAllEnabledScenes(),                      // scenes
                EditorUserBuildSettings.activeBuildTarget); // build target
        }
    }

    void DrawOpenBuildFolder()
    {
        string path = Application.dataPath.BeforeLast("Assets") + PipelineSettings.AutoBuildPath;
        string windowsPath = path.Replace("/", "\\");
        if (GUILayout.Button("Open build folder"))
        {
            if (Directory.Exists(windowsPath))
            {
                var p = new Process
                {
                    StartInfo = new ProcessStartInfo("explorer.exe", windowsPath)
                };
                p.Start();
            }
            else
            {
                EditorUtility.DisplayDialog("Folder missing", string.Format("Folder {0} doesn't exist yet", windowsPath), "Ok");
            }
        }
    }

    static string[] GetAllEnabledScenes()
    {
        EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
        List<string> result = new List<string>(scenes.Length);

        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].enabled)
                result.Add(scenes[i].path);
        }

        return result.ToArray();
    }

    static void KillProcessesAndStopEditor()
    {
        BuildTools.KillAllProcesses(PipelineSettings.AutoBuildExecutableName);
        EditorApplication.isPlaying = false;
    }

    [System.Serializable]
    private struct BuildOptionsGUIData
    {
        public bool buildScriptsOnly;
        public bool developmentBuild;
        public bool allowDebugging;

        public BuildOptions ProduceBuildOptions()
        {
            BuildOptions result = BuildOptions.None;

            if (buildScriptsOnly)
                result |= BuildOptions.BuildScriptsOnly;

            if (developmentBuild)
                result |= BuildOptions.Development;

            if (allowDebugging)
                result |= BuildOptions.AllowDebugging;

            return result;
        }
    }
}
