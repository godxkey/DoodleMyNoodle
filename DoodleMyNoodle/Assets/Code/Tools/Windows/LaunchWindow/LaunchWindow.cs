using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEditorX;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngineX;

public class LaunchWindow : ToolsWindowBase
{
    private List<PlayerProfile> _localPlayerProfiles;
    private List<LaunchProfileElement> _profileElements = new List<LaunchProfileElement>();
    private string[] _localPlayerProfileNames;
    private PopupField<string> _elementLevel;

    protected override string UssGuid => "d13a97c21d8810b44af913ddbff4af18";
    protected override string UxmlGuid => "56f189421eb1e05429a4f0281d7300c9";

    protected override void OnEnable()
    {
        base.OnEnable();
        EditorSceneManager.sceneOpened += OnSceneOpened;
    }

    private void OnDisable()
    {
        EditorSceneManager.sceneOpened -= OnSceneOpened;
    }


    protected override void Rebuild(VisualElement root)
    {
        _localPlayerProfiles = PlayerProfileService.LoadProfilesOnDisk();

        _localPlayerProfileNames = new string[_localPlayerProfiles.Count + 1];
        for (int i = 0; i < _localPlayerProfiles.Count; i++)
        {
            _localPlayerProfileNames[i] = _localPlayerProfiles[i].playerName;
        }
        _localPlayerProfileNames[_localPlayerProfiles.Count] = "- none -";

        _profileElements.Clear();

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Build Target
        ////////////////////////////////////////////////////////////////////////////////////////
        {
            var element = root.Q<Label>(name: "buildTarget");
            element.text = EditorUserBuildSettings.activeBuildTarget.ToString();
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Symbols Profile
        ////////////////////////////////////////////////////////////////////////////////////////
        /*{
            List<string> choices = new List<string>(ScriptDefineSymbolManager.Profiles.ToArray().Select(p => p.Name));

            if (choices.Count == 0)
            {
                choices.Add("");
            }

            int currentChoice = choices.IndexOf(EditorLaunchData.symbolsProfile);

            if (currentChoice == -1)
            {
                EditorLaunchData.symbolsProfile = choices[0];
                currentChoice = 0;
            }

            PopupField<string> popupField = new PopupField<string>("Symbols Profile", choices, currentChoice);
            popupField.RegisterValueChangedCallback(
                (ChangeEvent<string> changeEvent) =>
                {
                    EditorLaunchData.symbolsProfile = changeEvent.newValue;
                });

            var element = root.Q<VisualElement>(name: "symbolsProfile");
            element.Add(popupField);
        }*/

        ////////////////////////////////////////////////////////////////////////////////////////
        //      Developement Build
        ////////////////////////////////////////////////////////////////////////////////////////

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
            var element = root.Q<Toggle>(name: "fromScratch");
            var childrenContainer = root.Q<VisualElement>(name: "startingPointContainer");

            element.value = EditorLaunchData.playFromScratch;
            childrenContainer.EnableInClassList("hidden", element.value);

            element.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.playFromScratch = changeEvent.newValue;
                    childrenContainer.EnableInClassList("hidden", changeEvent.newValue);
                });
        }

        {
            var container = root.Q<VisualElement>(name: "startingPointContainer");
            var levelBank = AssetDatabaseX.LoadAssetsOfType<LevelBank>().FirstOrDefault();

            List<string> levels = new List<string>();
            levels.Add(""); // the 'none' option

            if (levelBank != null)
            {
                foreach (var item in levelBank.Levels)
                {
                    levels.Add(item.name);
                }

                levels.Sort();
            }

            string levelToDisplayName(string level)
            {
                if (string.IsNullOrEmpty(level))
                    return "- None -";
                return level;
            }

            _elementLevel = new PopupField<string>("Level", levels, 0,
                formatSelectedValueCallback: levelToDisplayName,
                formatListItemCallback: levelToDisplayName);

            container.Insert(0, _elementLevel);

            if (levels.Contains(EditorLaunchData.level))
            {
                _elementLevel.value = EditorLaunchData.level;
            }

            _elementLevel.RegisterValueChangedCallback(
                (ChangeEvent<string> changeEvent) =>
                {
                    EditorLaunchData.level = changeEvent.newValue;
                });
        }

        {
            var elementPlayOnline = root.Q<Toggle>(name: "online");
            var elementServerName = root.Q<TextField>(name: "serverName");

            elementServerName.value = EditorLaunchData.serverName;
            elementServerName.RegisterValueChangedCallback(
                (ChangeEvent<string> changeEvent) =>
                {
                    EditorLaunchData.serverName = changeEvent.newValue;
                });

            elementPlayOnline.value = EditorLaunchData.playOnline;
            elementPlayOnline.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.playOnline = changeEvent.newValue;

                    for (int i = 0; i < _profileElements.Count; i++)
                        _profileElements[i].UpdateContent();

                    elementServerName.EnableInClassList("hidden", !changeEvent.newValue);
                });

            elementServerName.EnableInClassList("hidden", !elementPlayOnline.value);
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
                    GameConsole.EditorPlayCommands = CommandLine.SplitCommandLine(changeEvent.newValue).ToArray();
                    EditorLaunchData.extraArguments = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<Toggle>(name: "overrideScreen");
            var childrenContainer = root.Q<VisualElement>(name: "overrideScreenContainer");

            element.value = EditorLaunchData.launchOverrideScreen;
            childrenContainer.EnableInClassList("hidden", !element.value);

            element.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.launchOverrideScreen = changeEvent.newValue;
                    childrenContainer.EnableInClassList("hidden", !changeEvent.newValue);
                });
        }

        {
            var element = root.Q<Toggle>(name: "fullscreen");
            element.value = EditorLaunchData.launchFullscreen;
            element.RegisterValueChangedCallback(
                (ChangeEvent<bool> changeEvent) =>
                {
                    EditorLaunchData.launchFullscreen = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<IntegerField>(name: "screenWidth");
            element.value = EditorLaunchData.launchScreenWidth;
            element.RegisterValueChangedCallback(
                (ChangeEvent<int> changeEvent) =>
                {
                    EditorLaunchData.launchScreenWidth = changeEvent.newValue;
                });
        }

        {
            var element = root.Q<IntegerField>(name: "screenHeight");
            element.value = EditorLaunchData.launchScreenHeight;
            element.RegisterValueChangedCallback(
                (ChangeEvent<int> changeEvent) =>
                {
                    EditorLaunchData.launchScreenHeight = changeEvent.newValue;
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
                    IsMarkedAsServer = (whoIsServer == profile.localId),
                    IsMarkedAsEditor = (whoIsEditor == profile.localId),
                };

                newElement.focusable = true;
                newElement.pickingMode = PickingMode.Position;
                newElement.AddManipulator(new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
                {
                    evt.menu.AppendAction("Set Server", (DropdownMenuAction action) =>
                    {
                        if (newElement.IsMarkedAsServer)
                        {
                            SetAsServer(null);
                        }
                        else
                        {
                            SetAsServer(newElement);
                        }
                    }, status: newElement.IsMarkedAsServer ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
                    evt.menu.AppendAction("Set Editor", (DropdownMenuAction action) =>
                    {
                        if (newElement.IsMarkedAsEditor)
                        {
                            SetAsEditor(null);
                        }
                        else
                        {
                            SetAsEditor(newElement);
                        }
                    }, status: newElement.IsMarkedAsEditor ? DropdownMenuAction.Status.Checked : DropdownMenuAction.Status.Normal);
                }));

                _profileElements.Add(newElement);
                element.Add(newElement);
            }
        }

        UpdateFromScenes();
    }

    private void OnSceneOpened(Scene scene, OpenSceneMode mode)
    {
        UpdateFromScenes();
    }

    private void UpdateFromScenes()
    {
        QuickStartEditorComponent quickStart = null;
        for (int i = 0; i < EditorSceneManager.sceneCount; i++)
        {
            foreach (GameObject rooGO in EditorSceneManager.GetSceneAt(i).GetRootGameObjects())
            {
                if (rooGO.TryGetComponent(out quickStart))
                    break;
            }
            if (quickStart != null)
                break;
        }

        if (quickStart != null && quickStart.overrideLevel)
        {
            _elementLevel.SetEnabled(!quickStart.overrideLevel);
            _elementLevel.label = "Level (set by scene)";
        }
        else
        {
            _elementLevel.SetEnabled(true);
            _elementLevel.label = "Level";
        }
    }

    void SetAsServer(LaunchProfileElement newServer)
    {
        int newId = newServer != null ? newServer.PlayerProfile.localId : -1;
        EditorLaunchData.whoIsServerId = newId;
        foreach (LaunchProfileElement launchProfileElement in _profileElements)
        {
            launchProfileElement.IsMarkedAsServer = (launchProfileElement.PlayerProfile.localId == newId);
        }
    }

    void SetAsEditor(LaunchProfileElement newEditor)
    {
        int newId = newEditor != null ? newEditor.PlayerProfile.localId : -1;
        EditorLaunchData.whoIsEditorId = newId;
        foreach (LaunchProfileElement launchProfileElement in _profileElements)
        {
            launchProfileElement.IsMarkedAsEditor = (launchProfileElement.PlayerProfile.localId == newId);
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

        var scriptSymbolsProfile = ScriptDefineSymbolManager.GetProfile(EditorLaunchData.symbolsProfile);
        string[] scriptingSymbols = scriptSymbolsProfile != null ? scriptSymbolsProfile.DefinedSymbols.ToArray() : new string[] { };

        BuildTools.BuildGame(
            PipelineSettings.AutoBuildPath,             // path
            PipelineSettings.AutoBuildExecutableName,   // exec name
            buildData.ProduceBuildOptions(),            // build options
            "AutoBuild",                                // build id
            GetAllEnabledScenes(),                      // scenes
            EditorUserBuildSettings.activeBuildTarget/*,  // build target
            scriptingSymbols*/);                          // scripting symbols
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

    [MenuItem("Tools/Launch Window _F11", priority = 100)]
    public static void ShowWindow()
    {
        LaunchWindow wnd = GetWindow<LaunchWindow>();
        wnd.titleContent = new GUIContent("LaunchWindow");
    }
}