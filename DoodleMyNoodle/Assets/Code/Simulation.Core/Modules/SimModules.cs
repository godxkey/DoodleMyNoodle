using System;

public static class SimModules
{
    internal static SimWorld world;

    // garder en horde alphabétique svp
    internal static ISimModuleBlueprintBank blueprintBank;
    internal static SimModuleEntityManager  entityManager;
    internal static SimModuleRandom         random;
    internal static SimModuleSceneLoader    sceneLoader;
    internal static SimModuleSerializer     serializer;
    internal static SimModuleTicker         ticker;
    internal static SimModuleWorldSearcher  worldSearcher;
}
