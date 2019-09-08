using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LaunchWindow : EditorWindow
{
    const string path = "Assets/Code/Editor/Pipeline/Windows/LaunchWindow/";

    [MenuItem("Tools/Launch Window")]
    public static void ShowExample()
    {
        LaunchWindow wnd = GetWindow<LaunchWindow>();
        wnd.titleContent = new GUIContent("LaunchWindow");
    }

    public void OnEnable()
    {
        Rebuild();
    }

    List<PlayerProfile> _localPlayerProfiles;
    List<LaunchProfileElement> _profileElements = new List<LaunchProfileElement>();
    string[] _localPlayerProfileNames;

    void Rebuild()
    {
        _localPlayerProfiles = PlayerProfileService.LoadProfilesOnDisk();

        _localPlayerProfileNames = new string[_localPlayerProfiles.Count + 1];
        for (int i = 0; i < _localPlayerProfiles.Count; i++)
        {
            _localPlayerProfileNames[i] = _localPlayerProfiles[i].playerName;
        }
        _localPlayerProfileNames[_localPlayerProfiles.Count] = "- none -";


        VisualElement root = rootVisualElement;
        root.Clear();
        _profileElements.Clear();

        StyleSheet styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "LaunchWindowStyles.uss");
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "LaunchWindowTree.uxml");
        SerializedObject serializedQuickStartAssets = new SerializedObject(QuickStartAssets.instance);

        visualTree.CloneTree(root);
        root.styleSheets.Add(styleSheet);

        root.Q<Button>(name: "refreshButton").clickable.clicked += Rebuild;

        {
            var element = root.Q<Label>(name: "buildTarget");
            element.text = EditorUserBuildSettings.activeBuildTarget.ToString();
        }

        {
            var element = root.Q<Toggle>(name: "developmentBuild");
            element.value = EditorLaunchData.developmentBuild;
            element.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.developmentBuild = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<Toggle>(name: "allowDebugging");
            element.value = EditorLaunchData.allowDebugging;
            element.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.allowDebugging = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<Button>(name: "buildGame");
            element.clickable.clicked += OnClick_BuildGame;
        }

        {
            var element = root.Q<Button>(name: "buildScripts");
            element.clickable.clicked += OnClick_BuildScripts;
        }

        {
            var element = root.Q<Button>(name: "openBuildFolder");
            element.clickable.clicked += OnClick_OpenBuildFolder;
        }

        {
            var element = root.Q<TextField>(name: "level");
            element.value = EditorLaunchData.level;
            element.RegisterValueChangedCallback(
                (ChangeEvent<string> changeEvent) =>
                {
                    EditorLaunchData.level = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<TextField>(name: "serverName");
            element.value = EditorLaunchData.serverName;
            element.RegisterValueChangedCallback(
                (ChangeEvent<string> changeEvent) =>
                {
                    EditorLaunchData.serverName = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<Toggle>(name: "online");
            element.value = EditorLaunchData.playOnline;
            element.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.playOnline = changeEvent.newValue;

                    for (int i = 0; i < _profileElements.Count; i++)
                        _profileElements[i].UpdateContent();
                });
        }

        {
            var element = root.Q<Foldout>(name: "advFoldout");
            element.value = false;
            element.text = "Advanced";
        }

        {
            var element = root.Q<Toggle>(name: "headless");
            element.value = EditorLaunchData.serverIsHeadless;
            element.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.serverIsHeadless = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<TextField>(name: "extraArgs");
            element.value = EditorLaunchData.extraArguments;
            element.RegisterValueChangedCallback(
                (ChangeEvent<string> changeEvent) =>
                {
                    EditorLaunchData.extraArguments = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<VisualElement>(name: "profilesContainer");

            int whoIsServer = EditorLaunchData.whoIsServerId;
            int whoIsEditor = EditorLaunchData.whoIsEditorId;
            for (int i = 0; i < _localPlayerProfiles.Count; i++)
            {
                PlayerProfile profile = _localPlayerProfiles[i];
                LaunchProfileElement newElement = new LaunchProfileElement(profile)
                {
                    isMarkedAsServer = (whoIsServer == profile.localId),
                    isMarkedAsEditor = (whoIsEditor == profile.localId),
                };

                newElement.focusable = true;
                newElement.pickingMode = PickingMode.Position;
                newElement.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {
                    evt.menu.AppendAction("Set Server", (DropdownMenuAction action) =>
                    {
                        if (newElement.isMarkedAsServer)
                        {
                            SetAsServer(null);
                        }
                        else
                        {
                            SetAsServer(newElement);
                        }
                    }, status: newElement.isMarkedAsServer ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
                    evt.menu.AppendAction("Set Editor", (DropdownMenuAction action) =>
                    {
                        if (newElement.isMarkedAsEditor)
                        {
                            SetAsEditor(null);
                        }
                        else
                        {
                            SetAsEditor(newElement);
                        }
                    }, status: newElement.isMarkedAsEditor ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
                }));

                _profileElements.Add(newElement);
                element.Add(newElement);
            }
        }
    }

    void SetAsServer(LaunchProfileElement newServer)
    {
        int newId = newServer != null ? newServer.playerProfile.localId : -1;
        EditorLaunchData.whoIsServerId = newId;
        foreach (LaunchProfileElement launchProfileElement in _profileElements)
        {
            launchProfileElement.isMarkedAsServer = (launchProfileElement.playerProfile.localId == newId);
        }
    }

    void SetAsEditor(LaunchProfileElement newEditor)
    {
        int newId = newEditor != null ? newEditor.playerProfile.localId : -1;
        EditorLaunchData.whoIsEditorId = newId;
        foreach (LaunchProfileElement launchProfileElement in _profileElements)
        {
            launchProfileElement.isMarkedAsEditor = (launchProfileElement.playerProfile.localId == newId);
        }
    }


    private struct BuildOptionsData
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

    void OnClick_BuildGame()
    {
        BuildGame(scriptsOnly: false);
    }

    void OnClick_BuildScripts()
    {
        BuildGame(scriptsOnly: true);
    }

    void BuildGame(bool scriptsOnly)
    {
        KillProcessesAndStopEditor();

        BuildOptionsData buildData = new BuildOptionsData()
        {
            buildScriptsOnly = scriptsOnly,
            developmentBuild = EditorLaunchData.developmentBuild,
            allowDebugging = EditorLaunchData.allowDebugging
        };

        BuildTools.BuildGame(
            PipelineSettings.AutoBuildPath,                // path
            PipelineSettings.AutoBuildExecutableName,      // exec name
            buildData.ProduceBuildOptions(),  // build options
            "AutoBuild",                                // build id
            GetAllEnabledScenes(),                      // scenes
            EditorUserBuildSettings.activeBuildTarget); // build target
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

    void OnClick_OpenBuildFolder()
    {
        string path = Application.dataPath.BeforeLast("Assets") + PipelineSettings.AutoBuildPath;
        string windowsPath = path.Replace("/", "\\");
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