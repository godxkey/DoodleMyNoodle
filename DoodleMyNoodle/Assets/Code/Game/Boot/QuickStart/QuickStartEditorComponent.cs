using UnityEngine;
using CCC.InspectorDisplay;
using Unity.Entities;
using UnityEngineX;
using System.Linq;
using System;
using UnityEngine.Serialization;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuickStartEditorComponent : MonoBehaviour
{
    public string ExtraCommands;
    
    [FormerlySerializedAs("startFromScratch")]
    public bool StartFromScratch;

    [HideIf(nameof(StartFromScratch))]
    [FormerlySerializedAs("overridePlayMode")]
    public bool OverridePlayMode;
    
    [ShowIf(nameof(CanShowPlayMode))]
    [FormerlySerializedAs("playMode")]
    public QuickStartSettings.PlayMode PlayMode;
    bool CanShowPlayMode => !StartFromScratch && OverridePlayMode;

    [HideIf(nameof(StartFromScratch))]
    [FormerlySerializedAs("overrideServerName")]
    public bool OverrideServerName;
    
    [ShowIf(nameof(CanShowServerName))]
    [FormerlySerializedAs("serverName")]
    public string ServerName;
    bool CanShowServerName => !StartFromScratch && OverrideServerName;

    [HideIf(nameof(StartFromScratch))]
    [FormerlySerializedAs("overrideLevel")]
    public bool OverrideLevel;

    [ShowIf(nameof(CanShowLevel))]
    [FormerlySerializedAs("level")]
    public Level Level;
    bool CanShowLevel => !StartFromScratch && OverrideLevel;

#if UNITY_EDITOR
    void Awake()
    {
        // Fred - maybe this is not necessary
        if (SceneService.TotalSceneLoadCount == 0)
        {
            CoreServiceManager.AddInitializationCallback(OnServicesReady);
        }
    }

    //private void Start()
    //{
    //    DisableSimulationEntitiesWarning();
    //}

    void OnServicesReady()
    {
        if (!string.IsNullOrEmpty(ExtraCommands))
        {
            GameConsole.ExecuteCommandLineStyleInvokables(ExtraCommands);
        }

        if (QuickStart.HasEverQuickStarted == false)
        {
            if (StartFromScratch || EditorLaunchData.playFromScratch)
            {
                QuickStart.StartFromScratch(EditorLaunchData.profileLocalId);
            }
            else
            {
                QuickStartSettings settings = new QuickStartSettings()
                {
                    localProfileId = EditorLaunchData.profileLocalId,
                    level = EditorLaunchData.level,
                    serverName = EditorLaunchData.serverName
                };

                if (EditorLaunchData.playOnline)
                {
                    settings.playMode = EditorLaunchData.whoIsServerId == EditorLaunchData.profileLocalId ?
                        QuickStartSettings.PlayMode.OnlineServer :
                        QuickStartSettings.PlayMode.OnlineClient;
                }
                else
                {
                    settings.playMode = QuickStartSettings.PlayMode.Local;
                }

                if (OverridePlayMode)
                    settings.playMode = PlayMode;

                if (OverrideServerName)
                    settings.serverName = ServerName;

                if (OverrideLevel)
                    settings.level = Level ? Level.name : "";

                QuickStart.Start(settings);
            }
        }
    }

    //private void DisableSimulationEntitiesWarning()
    //{
    //    SimulationWorldSystem simWorldSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<SimulationWorldSystem>();
    //    if(simWorldSystem != null)
    //    {
    //        simWorldSystem.DisableEntityInjectionWarningUntilNextClear(maxExpectedDuration: 10);
    //    }
    //}
#endif
}
