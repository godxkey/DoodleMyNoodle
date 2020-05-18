using UnityEngine;
using UnityEngineX;

public class CommandLineBoot : MonoBehaviour
{
    void Awake()
    {
        CoreServiceManager.AddInitializationCallback(OnCoreServicesReady);
    }

    void OnCoreServicesReady()
    {
        // read the command line arguments
        DebugService.Log("CommandLine: " + CommandLine.CompleteCommandLine);

        QuickStartSettings quickStartSettings = new QuickStartSettings();

        int playModeValue;
        if (CommandLine.TryGetInt("-playmode", out playModeValue))
        {
            CommandLine.TryGetInt("-profileId", out quickStartSettings.localProfileId);
            CommandLine.TryGetString("-servername", out quickStartSettings.serverName);
            CommandLine.TryGetString("-level", out quickStartSettings.level);
            quickStartSettings.playMode = (QuickStartSettings.PlayMode)playModeValue;


            QuickStart.Start(quickStartSettings);
        }
        else
        {
            QuickStart.StartFromScratch();
        }
    }
}
