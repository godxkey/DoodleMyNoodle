using System;

public static class SimModules
{
    internal static SimWorld world;

    internal static ISimModuleBlueprintBank blueprintBank;
    internal static SimModuleEntityManager entityManager;
    internal static SimModuleTicker ticker;
    internal static SimModuleWorldSearcher worldSearcher;
    internal static SimModuleSceneLoader sceneLoader;
    internal static SimModuleSerializer serializer;
}
