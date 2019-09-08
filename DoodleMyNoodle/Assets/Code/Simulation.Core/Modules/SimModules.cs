using System;

internal static class SimModules
{
    internal static void Initialize(ISimModuleBlueprintBank iBlueprintBank)
    {
        // garder en horde alphabétique svp
        BlueprintBank = iBlueprintBank;
        EntityManager = new SimModuleEntityManager();
        InputProcessorManager = new SimModuleInputProcessorManager();
        Random = new SimModuleRandom();
        SceneLoader = new SimModuleSceneLoader();
        Serializer = new SimModuleSerializer();
        Ticker = new SimModuleTicker();
        World = new SimWorld();
        WorldSearcher = new SimModuleWorldSearcher();
    }

    internal static void Dispose()
    {
        IsDisposed = true;

        // garder en horde alphabétique svp
        BlueprintBank.Dispose();
        EntityManager.Dispose();
        InputProcessorManager.Dispose();
        Random.Dispose();
        SceneLoader.Dispose();
        Serializer.Dispose();
        Ticker.Dispose();
        World.Dispose();
        WorldSearcher.Dispose();

        // garder en horde alphabétique svp
        BlueprintBank = null;
        EntityManager = null;
        InputProcessorManager = null;
        Random = null;
        SceneLoader = null;
        Serializer = null;
        Ticker = null;
        World = null;
        WorldSearcher = null;
    }


    internal static bool IsDisposed;
    internal static SimWorld World;

    // garder en horde alphabétique svp
    internal static ISimModuleBlueprintBank BlueprintBank;
    internal static SimModuleEntityManager EntityManager;
    internal static SimModuleInputProcessorManager InputProcessorManager;
    internal static SimModuleRandom Random;
    internal static SimModuleSceneLoader SceneLoader;
    internal static SimModuleSerializer Serializer;
    internal static SimModuleTicker Ticker;
    internal static SimModuleWorldSearcher WorldSearcher;

    internal static bool IsInitialized => World != null;
}
