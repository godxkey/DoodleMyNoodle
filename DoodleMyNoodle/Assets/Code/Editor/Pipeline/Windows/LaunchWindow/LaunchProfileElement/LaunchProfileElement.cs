using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LaunchProfileElement : VisualElement
{
    const string path = "Assets/Code/Editor/Pipeline/Windows/LaunchWindow/LaunchProfileElement/";

    public PlayerProfile playerProfile { get; private set; }

    bool _isServer;
    public bool isServer { get => _isServer; set { _isServer = value; UpdateContent(); } }

    bool myStandaloneIsRunning
        => _standaloneProcessHandle != null
        && !_standaloneProcessHandle.hasExited;

    bool anyEditorIsRunning
        => Application.isPlaying;

    bool myEditorIsRunning
        => Application.isPlaying
        && PlayerProfileService.Instance != null
        && PlayerProfileService.Instance.currentProfile == playerProfile;

    TextField content_profileName;
    VisualElement content_container;
    Button content_playEditor;
    Button content_playStandalone;
    Button content_stopEditor;
    Button content_stopStandalone;

    ProcessHandle _standaloneProcessHandle;

    bool subscribedToUpdate = false;

    public LaunchProfileElement(PlayerProfile playerProfile)
    {
        this.playerProfile = playerProfile;
        BindToStandaloneProcessFromProfile();

        RegisterCallback<DetachFromPanelEvent>(OnDetach, TrickleDown.TrickleDown);

        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "LaunchProfileElementStyles.uss"));
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "LaunchProfileElementTree.uxml");


        visualTree.CloneTree(this);

        content_profileName = this.Q<TextField>("profileName");
        content_container = this.Q<VisualElement>("container");
        content_playEditor = this.Q<Button>("playEditor");
        content_stopEditor = this.Q<Button>("stopEditor");
        content_playStandalone = this.Q<Button>("playStandalone");
        content_stopStandalone = this.Q<Button>("stopStandalone");

        content_profileName.SetEnabled(false);
        content_profileName.RegisterValueChangedCallback(
            (s) =>
            {
                if (playerProfile != null)
                    playerProfile.playerName = s.newValue;
            });

        content_playEditor.clickable.clicked += LaunchEditor;
        content_stopEditor.clickable.clicked += StopEditor;
        content_playStandalone.clickable.clicked += LaunchStandalone;
        content_stopStandalone.clickable.clicked += StopStandalone;

        CoreServiceManager.AddInitializationCallback(OnEditorGameLaunch);
        UpdateContent();
    }

    void OnEditorGameLaunch()
    {
        if (CoreServiceManager.instance == null)
            return;

        UpdateContent();

        if (PlayerProfileService.Instance != null)
        {
            PlayerProfileService.Instance.onChangeProfile += UpdateContent;
        }
    }

    public void UpdateContent()
    {
        if (playerProfile != null)
            content_profileName.value = playerProfile.playerName;

        if (isServer)
        {
            content_container.AddToClassList("serverHighlight");
        }
        else
        {
            content_container.RemoveFromClassList("serverHighlight");
        }


        content_playEditor.style.visibility = Visible(!myEditorIsRunning);
        content_playEditor.SetEnabled(!anyEditorIsRunning);
        content_playStandalone.style.visibility = Visible(!myStandaloneIsRunning);
        content_playStandalone.SetEnabled(!myEditorIsRunning);
        content_stopEditor.style.visibility = Visible(myEditorIsRunning);
        content_stopStandalone.style.visibility = Visible(myStandaloneIsRunning);

        ReceiveUpdates(anyEditorIsRunning || myStandaloneIsRunning);
    }


    void OnDetach(DetachFromPanelEvent evt)
    {
        ReceiveUpdates(false);

        if(PlayerProfileService.Instance != null)
        {
            PlayerProfileService.Instance.onChangeProfile -= UpdateContent;
        }
    }

    public void OnUpdate()
    {
        if (!anyEditorIsRunning && !myStandaloneIsRunning)
        {
            UpdateContent();
        }
    }

    void ReceiveUpdates(bool value)
    {
        if (subscribedToUpdate == value)
            return;
        subscribedToUpdate = value;

        if (value)
            EditorApplication.update += OnUpdate;
        else
            EditorApplication.update -= OnUpdate;
    }

    StyleEnum<Visibility> Visible(bool value)
    {
        return value ? new StyleEnum<Visibility>(Visibility.Visible) : new StyleEnum<Visibility>(Visibility.Hidden);
    }

    void LaunchEditor()
    {
        EditorLaunchData.profileLocalId = playerProfile.localId;
        EditorApplication.isPlaying = true;
    }

    void StopEditor()
    {
        EditorApplication.isPlaying = false;
    }

    void LaunchStandalone()
    {
        string args = GetFinalLaunchArguments().ToString();

        var buildPath = PipelineSettings.AutoBuildPath;
        var buildExe = PipelineSettings.AutoBuildExecutableName;

        var process = new Process();

        process.StartInfo.UseShellExecute = args.Contains("-batchmode");
        process.StartInfo.FileName = Application.dataPath + "/../" + buildPath + "/" + buildExe;
        process.StartInfo.Arguments = args;
        process.StartInfo.WorkingDirectory = buildPath;
        process.Start();

        ProcessHandle processHandle = new ProcessHandle(process);
        processHandle.id = playerProfile.localId;
        BindToStandaloneProcess(processHandle);

        UpdateContent();
    }

    void OnStandaloneProcessExit()
    {
        _standaloneProcessHandle = null;
        UpdateContent();
    }

    void StopStandalone()
    {
        if (_standaloneProcessHandle != null)
        {
            _standaloneProcessHandle.process.Kill();
        }
    }

    void BindToStandaloneProcessFromProfile()
    {
        ProcessHandle.RegisterOnInitCallback(() =>
        {
            if (playerProfile == null)
                return;

            foreach (ProcessHandle handle in ProcessHandle.activeHandles)
            {
                ProcessHandle standaloneProcessHandle = handle as ProcessHandle;
                if (standaloneProcessHandle != null && handle.id == playerProfile.localId)
                {
                    BindToStandaloneProcess(standaloneProcessHandle);
                    return;
                }
            }
        });
    }

    void BindToStandaloneProcess(ProcessHandle processHandle)
    {
        _standaloneProcessHandle = processHandle;
        _standaloneProcessHandle.onExitAction = OnStandaloneProcessExit;
        UpdateContent();
    }

    QuickStartSettings.PlayMode GetPlayMode()
    {
        if (EditorLaunchData.playOnline)
        {
            return isServer ? QuickStartSettings.PlayMode.OnlineServer : QuickStartSettings.PlayMode.OnlineClient;
        }
        else
        {
            return QuickStartSettings.PlayMode.Local;
        }
    }

    string GetFinalLaunchArguments()
    {
        StringBuilder finalArguments = new StringBuilder();

        QuickStartSettings.PlayMode playmode = GetPlayMode();
        finalArguments.Append("-playmode ");
        finalArguments.Append((int)playmode);
        finalArguments.Append(' ');

        string level = EditorLaunchData.level;
        if (level.IsNullOrEmpty() == false)
        {
            finalArguments.Append("-level ");
            finalArguments.Append(level);
            finalArguments.Append(' ');
        }

        string serverName = EditorLaunchData.serverName;
        if (serverName.IsNullOrEmpty() == false)
        {
            finalArguments.Append("-servername ");
            finalArguments.Append('\"' + serverName + '\"'); // we need to wrap the string with quotes "" to allow for spaces
            finalArguments.Append(' ');
        }

        finalArguments.Append("-profileId " + playerProfile.localId);
        finalArguments.Append(' ');

        bool headless = EditorLaunchData.serverIsHeadless && (playmode == QuickStartSettings.PlayMode.OnlineServer);
        if (headless)
        {
            finalArguments.Append("-batchmode -nographics ");
        }

        finalArguments.Append(EditorLaunchData.extraArguments);
        finalArguments.Append(' ');

        return finalArguments.ToString();
    }
}
