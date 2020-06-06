public static class GameBoltConfig
{
    public static BoltConfig GetConfig()
    {
        BoltConfig newConfig = BoltRuntimeSettings.instance.GetConfigCopy();

        newConfig.disableAutoSceneLoading = true;

        return newConfig;
    }
}
