using System;

internal static class SimModules
{
    internal static void Initialize(ISimModuleBlueprintBank iBlueprintBank)
    {
        // garder en horde alphabétique svp
        blueprintBank = iBlueprintBank;
        entityManager = new SimModuleEntityManager();
        random = new SimModuleRandom();
        sceneLoader = new SimModuleSceneLoader();
        serializer = new SimModuleSerializer();
        ticker = new SimModuleTicker();
        world = new SimWorld();
        worldSearcher = new SimModuleWorldSearcher();
    }

    internal static void Shutdown()
    {
        // garder en horde alphabétique svp
        blueprintBank = null;
        entityManager = null;
        random = null;
        sceneLoader = null;
        serializer = null;
        ticker = null;
        world = null;
        worldSearcher = null;
    }


    internal static SimWorld world;

    // garder en horde alphabétique svp
    internal static ISimModuleBlueprintBank blueprintBank;
    internal static SimModuleEntityManager  entityManager;
    internal static SimModuleRandom         random;
    internal static SimModuleSceneLoader    sceneLoader;
    internal static SimModuleSerializer     serializer;
    internal static SimModuleTicker         ticker;
    internal static SimModuleWorldSearcher  worldSearcher;

    internal static bool isInitialized => world != null;
}
