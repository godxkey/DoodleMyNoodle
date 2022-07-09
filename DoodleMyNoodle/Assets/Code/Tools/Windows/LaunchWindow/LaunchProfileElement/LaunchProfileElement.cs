using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class LaunchProfileElement : ToolsVisualElementBase
{
    public PlayerProfile PlayerProfile { get; private set; }

    bool _isMarkedAsServer;
    bool _isMarkedAsEditor;
    public bool IsMarkedAsServer { get => _isMarkedAsServer; set { _isMarkedAsServer = value; UpdateContent(); } }
    public bool IsMarkedAsEditor { get => _isMarkedAsEditor; set { _isMarkedAsEditor = value; UpdateContent(); } }

    bool MyStandaloneIsRunning
        => _standaloneProcessHandle != null
        && !_standaloneProcessHandle.HasExited;

    bool AnyEditorIsRunning
        => Application.isPlaying;

    bool MyEditorIsRunning
        => Application.isPlaying
        && PlayerProfileService.Instance != null
        && PlayerProfileService.Instance.CurrentProfile == PlayerProfile;

    public override string UxmlGuid => "04ab12ab6d1ce71409dfafb6e7178471";
    public override string UssGuid => "ca3898a92df42194d969b1f0347a0c6c";

    readonly TextField _content_profileName;
    readonly Button _content_play;
    readonly Button _content_stop;
    readonly TextElement _content_tag_editor;
    readonly TextElement _content_tag_server;

    ProcessHandle _standaloneProcessHandle;

    bool _subscribedToUpdate = false;

    public LaunchProfileElement(PlayerProfile playerProfile)
        : base()
    {
        this.PlayerProfile = playerProfile;

        RegisterCallback<DetachFromPanelEvent>(OnDetach, TrickleDown.TrickleDown);

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
        if (CoreServiceManager.Instance == null)
            return;

        UpdateContent();

        if (PlayerProfileService.Instance != null)
        {
            PlayerProfileService.Instance.OnChangeProfile += UpdateContent;
        }
    }

    public void UpdateContent()
    {
        if (PlayerProfile != null)
            _content_profileName.value = PlayerProfile.playerName;

        _content_tag_editor.style.visibility = Visible(IsMarkedAsEditor);
        _content_tag_server.style.visibility = Visible(IsMarkedAsServer && EditorLaunchData.PlayOnline && !EditorLaunchData.PlayFromScratch);
        _content_play.style.visibility = Visible(!MyEditorIsRunning && !MyStandaloneIsRunning);
        _content_stop.style.visibility = Visible(MyEditorIsRunning || MyStandaloneIsRunning);

        ReceiveUpdates(AnyEditorIsRunning || MyStandaloneIsRunning);
    }


    void OnDetach(DetachFromPanelEvent evt)
    {
        ReceiveUpdates(false);

        if (PlayerProfileService.Instance != null)
        {
            PlayerProfileService.Instance.OnChangeProfile -= UpdateContent;
        }
    }

    public void OnUpdate()
    {
        if (!AnyEditorIsRunning && !MyStandaloneIsRunning)
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
        if (IsMarkedAsEditor)
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
        if (MyStandaloneIsRunning)
        {
            StopMyStandalone();
        }

        if (MyEditorIsRunning)
        {
            StopMyEditor();
        }
    }

    void LaunchEditor()
    {
        EditorLaunchData.ProfileLocalId = PlayerProfile.localId;
        EditorApplication.isPlaying = true;
    }

    void StopMyEditor()
    {
        EditorApplication.isPlaying = false;
    }

    void LaunchStandalone()
    {
        string args = GetFinalLaunchArguments().ToString();

        var lastBuildArtifact = BuildTools.GetLastBuildFile();
        if (lastBuildArtifact == null)
        {
            EditorUtility.DisplayDialog("No Build", "No windows build artifact found.", "Ok");
            return;
        }

        var process = new Process();

        process.StartInfo.UseShellExecute = args.Contains("-batchmode");
        process.StartInfo.FileName = lastBuildArtifact.FullName;
        process.StartInfo.Arguments = args;
        process.StartInfo.WorkingDirectory = lastBuildArtifact.Directory.FullName;
        process.Start();

        ProcessHandle processHandle = new ProcessHandle(process, customId: PlayerProfile.localId);
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
        if (PlayerProfile == null)
            return;

        foreach (ProcessHandle handle in ProcessHandle.ActiveHandles)
        {
            ProcessHandle standaloneProcessHandle = handle as ProcessHandle;
            if (standaloneProcessHandle != null && handle.CustomId == PlayerProfile.localId)
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

    QuickStartSettings.EPlayMode GetPlayMode()
    {
        if (EditorLaunchData.PlayOnline)
        {
            return IsMarkedAsServer ? QuickStartSettings.EPlayMode.OnlineServer : QuickStartSettings.EPlayMode.OnlineClient;
        }
        else
        {
            return QuickStartSettings.EPlayMode.Local;
        }
    }

    string GetFinalLaunchArguments()
    {
        StringBuilder finalArguments = new StringBuilder();

        bool headless = false;
        if (!EditorLaunchData.PlayFromScratch)
        {
            QuickStartSettings.EPlayMode playmode = GetPlayMode();
            headless = EditorLaunchData.ServerIsHeadless && (playmode == QuickStartSettings.EPlayMode.OnlineServer);

            finalArguments.Append("-playmode ");
            finalArguments.Append((int)playmode);
            finalArguments.Append(' ');

            string map = EditorLaunchData.Map;
            if (string.IsNullOrEmpty(map) == false)
            {
                finalArguments.Append("-map ");
                finalArguments.Append(map);
                finalArguments.Append(' ');
            }

            string serverName = EditorLaunchData.ServerName;
            if (string.IsNullOrEmpty(serverName) == false)
            {
                finalArguments.Append("-servername ");
                finalArguments.Append('\"' + serverName + '\"'); // we need to wrap the string with quotes "" to allow for spaces
                finalArguments.Append(' ');
            }
        }

        finalArguments.Append("-profileId " + PlayerProfile.localId);
        finalArguments.Append(' ');

        if (headless)
        {
            finalArguments.Append("-batchmode -nographics ");
        }
        else if (EditorLaunchData.LaunchOverrideScreen)
        {
            finalArguments.Append($"-screen-fullscreen {(EditorLaunchData.LaunchFullscreen ? 1 : 0)} ");
            finalArguments.Append($"-screen-height {EditorLaunchData.LaunchScreenHeight} ");
            finalArguments.Append($"-screen-width {EditorLaunchData.LaunchScreenWidth} ");
        }

        finalArguments.Append(EditorLaunchData.ExtraArguments);
        finalArguments.Append(' ');

        return finalArguments.ToString();
    }
}
