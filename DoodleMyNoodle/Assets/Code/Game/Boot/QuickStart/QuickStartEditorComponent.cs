using UnityEngine;
using CCC.InspectorDisplay;
using Unity.Entities;
using UnityEngineX;
using System.Linq;
using System;
using UnityEngine.Serialization;

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
    public QuickStartSettings.EPlayMode PlayMode;
    bool CanShowPlayMode => !StartFromScratch && OverridePlayMode;

    [HideIf(nameof(StartFromScratch))]
    [FormerlySerializedAs("overrideServerName")]
    public bool OverrideServerName;
    
    [ShowIf(nameof(CanShowServerName))]
    [FormerlySerializedAs("serverName")]
    public string ServerName;
    bool CanShowServerName => !StartFromScratch && OverrideServerName;

    [HideIf(nameof(StartFromScratch))]
    [FormerlySerializedAs("OverrideLevel")]
    public bool OverrideMap;

    [ShowIf(nameof(CanShowMap))]
    [FormerlySerializedAs("Level")]
    public Map Map;
    bool CanShowMap => !StartFromScratch && OverrideMap;

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
            if (StartFromScratch || EditorLaunchData.PlayFromScratch)
            {
                QuickStart.StartFromScratch(EditorLaunchData.ProfileLocalId);
            }
            else
            {
                QuickStartSettings settings = new QuickStartSettings()
                {
                    LocalProfileId = EditorLaunchData.ProfileLocalId,
                    Map = EditorLaunchData.Map,
                    ServerName = EditorLaunchData.ServerName
                };

                if (EditorLaunchData.PlayOnline)
                {
                    settings.PlayMode = EditorLaunchData.WhoIsServerId == EditorLaunchData.ProfileLocalId ?
                        QuickStartSettings.EPlayMode.OnlineServer :
                        QuickStartSettings.EPlayMode.OnlineClient;
                }
                else
                {
                    settings.PlayMode = QuickStartSettings.EPlayMode.Local;
                }

                if (OverridePlayMode)
                    settings.PlayMode = PlayMode;

                if (OverrideServerName)
                    settings.ServerName = ServerName;

                if (OverrideMap)
                    settings.Map = Map ? Map.name : "";

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
