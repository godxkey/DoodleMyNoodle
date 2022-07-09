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

        CommandLine.TryGetInt("-profileId", out quickStartSettings.LocalProfileId);

        if (CommandLine.TryGetInt("-playmode", out int playModeValue))
        {
            CommandLine.TryGetString("-servername", out quickStartSettings.ServerName);
            CommandLine.TryGetString("-map", out quickStartSettings.Map);
            quickStartSettings.PlayMode = (QuickStartSettings.EPlayMode)playModeValue;


            QuickStart.Start(quickStartSettings);
        }
        else
        {
            QuickStart.StartFromScratch(quickStartSettings.LocalProfileId);
        }
    }
}
