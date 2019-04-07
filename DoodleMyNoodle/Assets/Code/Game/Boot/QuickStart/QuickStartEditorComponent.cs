using UnityEngine;

public class QuickStartEditorComponent : MonoBehaviour
{
    public bool startFromScratch;

    [HideIf("startFromScratch")]
    public bool overridePlayMode;
    [ShowIf("CanShowPlayMode", HideShowBaseAttribute.Type.Property)]
    public QuickStartSettings.PlayMode playMode;
    bool CanShowPlayMode => !startFromScratch && overridePlayMode;

    [HideIf("startFromScratch")]
    public bool overridePlayerName;
    [ShowIf("CanShowPlayerName", HideShowBaseAttribute.Type.Property)]
    public string playerName;
    bool CanShowPlayerName => !startFromScratch && overridePlayerName;

    [HideIf("startFromScratch")]
    public bool overrideServerName;
    [ShowIf("CanShowServerName", HideShowBaseAttribute.Type.Property)]
    public string serverName;
    bool CanShowServerName => !startFromScratch && overrideServerName;

    [HideIf("startFromScratch")]
    public bool overrideLevel;
    [ShowIf("CanShowLevel", HideShowBaseAttribute.Type.Property)]
    public string level;
    bool CanShowLevel => !startFromScratch && overrideLevel;

#if UNITY_EDITOR
    void Awake()
    {
        // Fred - maybe this is not necessary
        if(SceneService.totalSceneLoadCount == 0)
        {
            CoreServiceManager.AddInitializationCallback(OnServicesReady);
        }
    }

    void OnServicesReady()
    {
        if(QuickStart.hasEverQuickStarted == false)
        {
            if (startFromScratch)
            {
                QuickStart.StartFromScratch();
            }
            else
            {
                QuickStartSettings settings = QuickStartAssets.instance.defaultSettings;

                if (overridePlayMode)
                    settings.playMode = playMode;

                if (overridePlayerName)
                    settings.playerName = playerName;

                if (overrideServerName)
                    settings.serverName = serverName;

                if (overrideLevel)
                    settings.level = level;

                QuickStart.Start(settings);
            }
        }
    }
#endif
}
