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
        Log.Info("CommandLine: " + CommandLine.CompleteCommandLine);

        QuickStartSettings quickStartSettings = new QuickStartSettings();

        CommandLine.TryGetInt("-profileId", out quickStartSettings.localProfileId);

        if (CommandLine.TryGetInt("-playmode", out int playModeValue))
        {
            CommandLine.TryGetString("-servername", out quickStartSettings.serverName);
            CommandLine.TryGetString("-level", out quickStartSettings.level);
            quickStartSettings.playMode = (QuickStartSettings.PlayMode)playModeValue;


            QuickStart.Start(quickStartSettings);
        }
        else
        {
            QuickStart.StartFromScratch(quickStartSettings.localProfileId);
        }
    }
}
