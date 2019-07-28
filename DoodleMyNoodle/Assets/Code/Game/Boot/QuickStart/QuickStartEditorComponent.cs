using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuickStartEditorComponent : MonoBehaviour
{
    public bool startFromScratch;

    [HideIf("startFromScratch")]
    public bool overridePlayMode;
    [ShowIf("CanShowPlayMode", HideShowBaseAttribute.Type.Property)]
    public QuickStartSettings.PlayMode playMode;
    bool CanShowPlayMode => !startFromScratch && overridePlayMode;

    [HideIf("startFromScratch")]
    public bool overrideServerName;
    [ShowIf("CanShowServerName", HideShowBaseAttribute.Type.Property)]
    public string serverName;
    bool CanShowServerName => !startFromScratch && overrideServerName;

    [HideIf("startFromScratch")]
    public bool overrideLevel;
    [ShowIf("CanShowLevel", HideShowBaseAttribute.Type.Property)]
    public SceneInfo level;
    bool CanShowLevel => !startFromScratch && overrideLevel;

#if UNITY_EDITOR
    void Awake()
    {
        // Fred - maybe this is not necessary
        if (SceneService.totalSceneLoadCount == 0)
        {
            CoreServiceManager.AddInitializationCallback(OnServicesReady);
        }
    }

    void OnServicesReady()
    {
        if (QuickStart.hasEverQuickStarted == false)
        {
            if (startFromScratch)
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

                if (overridePlayMode)
                    settings.playMode = playMode;

                if (overrideServerName)
                    settings.serverName = serverName;

                if (overrideLevel)
                    settings.level = level ? level.SceneName : "";

                QuickStart.Start(settings);
            }
        }
    }
#endif
}
