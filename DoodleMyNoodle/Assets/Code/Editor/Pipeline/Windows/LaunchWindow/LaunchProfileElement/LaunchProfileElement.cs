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

    bool _isMarkedAsServer;
    bool _isMarkedAsEditor;
    public bool isMarkedAsServer { get => _isMarkedAsServer; set { _isMarkedAsServer = value; UpdateContent(); } }
    public bool isMarkedAsEditor { get => _isMarkedAsEditor; set { _isMarkedAsEditor = value; UpdateContent(); } }

    bool myStandaloneIsRunning
        => _standaloneProcessHandle != null
        && !_standaloneProcessHandle.HasExited;

    bool anyEditorIsRunning
        => Application.isPlaying;

    bool myEditorIsRunning
        => Application.isPlaying
        && PlayerProfileService.Instance != null
        && PlayerProfileService.Instance.currentProfile == playerProfile;

    readonly TextField _content_profileName;
    readonly Button _content_play;
    readonly Button _content_stop;
    readonly TextElement _content_tag_editor;
    readonly TextElement _content_tag_server;

    ProcessHandle _standaloneProcessHandle;

    bool _subscribedToUpdate = false;

    public LaunchProfileElement(PlayerProfile playerProfile)
    {
        this.playerProfile = playerProfile;

        RegisterCallback<DetachFromPanelEvent>(OnDetach, TrickleDown.TrickleDown);

        styleSheets.Add(AssetDatabase.LoadAssetAtPath<StyleSheet>(path + "LaunchProfileElementStyles.uss"));
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path + "LaunchProfileElementTree.uxml");


        visualTree.CloneTree(this);

        _content_profileName = this.Q<TextField>("profileName");
        _content_play = this.Q<Button>("play");
        _content_stop = this.Q<Button>("stop");
        _content_tag_editor = this.Q<TextElement>("editorTag");
        _content_tag_server = this.Q<TextElement>("serverTag");

        _content_profileName.SetEnabled(false);
        _content_profileName.RegisterValueChangedCallback(
            (s) =>
            {
                if (playerProfile != null)
                    playerProfile.playerName = s.newValue;
            });

        _content_play.clickable.clicked += Play;
        _content_stop.clickable.clicked += Stop;

        BindToStandaloneProcessFromProfile();
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
            _content_profileName.value = playerProfile.playerName;

        _content_tag_editor.style.visibility = Visible(isMarkedAsEditor);
        _content_tag_server.style.visibility = Visible(isMarkedAsServer && EditorLaunchData.playOnline);
        _content_play.style.visibility = Visible(!myEditorIsRunning && !myStandaloneIsRunning);
        _content_stop.style.visibility = Visible(myEditorIsRunning || myStandaloneIsRunning);

        ReceiveUpdates(anyEditorIsRunning || myStandaloneIsRunning);
    }


    void OnDetach(DetachFromPanelEvent evt)
    {
        ReceiveUpdates(false);

        if (PlayerProfileService.Instance != null)
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
        if (_subscribedToUpdate == value)
            return;
        _subscribedToUpdate = value;

        if (value)
            EditorApplication.update += OnUpdate;
        else
            EditorApplication.update -= OnUpdate;
    }

    StyleEnum<Visibility> Visible(bool value)
    {
        return value ? new StyleEnum<Visibility>(Visibility.Visible) : new StyleEnum<Visibility>(Visibility.Hidden);
    }

    void Play()
    {
        if (isMarkedAsEditor)
        {
            LaunchEditor();
        }
        else
        {
            LaunchStandalone();
        }
    }

    void Stop()
    {
        if (myStandaloneIsRunning)
        {
            StopMyStandalone();
        }

        if (myEditorIsRunning)
        {
            StopMyEditor();
        }
    }

    void LaunchEditor()
    {
        EditorLaunchData.profileLocalId = playerProfile.localId;
        EditorApplication.isPlaying = true;
    }

    void StopMyEditor()
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

        ProcessHandle processHandle = new ProcessHandle(process, customId: playerProfile.localId);
        BindToStandaloneProcess(processHandle);

        UpdateContent();
    }

    void OnStandaloneProcessExit()
    {
        _standaloneProcessHandle = null;
        UpdateContent();
    }

    void StopMyStandalone()
    {
        if (_standaloneProcessHandle != null)
        {
            _standaloneProcessHandle.Process.Kill();
        }
    }

    void BindToStandaloneProcessFromProfile()
    {
        if (playerProfile == null)
            return;

        foreach (ProcessHandle handle in ProcessHandle.ActiveHandles)
        {
            ProcessHandle standaloneProcessHandle = handle as ProcessHandle;
            if (standaloneProcessHandle != null && handle.CustomId == playerProfile.localId)
            {
                BindToStandaloneProcess(standaloneProcessHandle);
                return;
            }
        }
    }

    void BindToStandaloneProcess(ProcessHandle processHandle)
    {
        _standaloneProcessHandle = processHandle;
        _standaloneProcessHandle.OnExitAction = OnStandaloneProcessExit;
        UpdateContent();
    }

    QuickStartSettings.PlayMode GetPlayMode()
    {
        if (EditorLaunchData.playOnline)
        {
            return isMarkedAsServer ? QuickStartSettings.PlayMode.OnlineServer : QuickStartSettings.PlayMode.OnlineClient;
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
