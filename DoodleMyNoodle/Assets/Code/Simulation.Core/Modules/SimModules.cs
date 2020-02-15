using System;

internal static class SimModules
{
    internal static void Initialize(SimulationCoreSettings settings)
    {
        // garder en horde alphabétique svp
        _BlueprintManager      = new SimModuleBlueprintManager();
        _EntityManager         = new SimModuleEntityManager();
        _InputProcessorManager = new SimModuleInputProcessorManager();
        _PresentationSceneManager = new SimModulePresentationSceneManager();
        _Random                = new SimModuleRandom();
        _SceneLoader           = new SimModuleSceneLoader();
        _Serializer            = new SimModuleSerializer();
        _Ticker                = new SimModuleTicker();
        _WorldSearcher         = new SimModuleWorldSearcher();

        _Modules = new SimModuleBase[]
        {
            _BlueprintManager     ,
            _EntityManager        ,
            _InputProcessorManager,
            _PresentationSceneManager,
            _Random               ,
            _SceneLoader          ,
            _Serializer           ,
            _Ticker               ,
            _WorldSearcher        ,
        };

        // not a module - contains no logic
        _World                 = new SimWorld();


        for (int i = 0; i < _Modules.Length; i++)
        {
            _Modules[i].Initialize(settings);
        }
    }

    internal static void Dispose()
    {
        _IsDisposed = true;

        for (int i = 0; i < _Modules.Length; i++)
        {
            _Modules[i].Dispose();
        }

        // garder en horde alphabétique svp
        _BlueprintManager = null;
        _EntityManager = null;
        _InputProcessorManager = null;
        _PresentationSceneManager = null;
        _Random = null;
        _SceneLoader = null;
        _Serializer = null;
        _Ticker = null;
        _WorldSearcher = null;

        _World = null;
    }


    internal static bool _IsDisposed;
    internal static SimWorld _World;

    // garder en horde alphabétique svp
    internal static SimModuleBlueprintManager _BlueprintManager;
    internal static SimModuleEntityManager _EntityManager;
    internal static SimModuleInputProcessorManager _InputProcessorManager;
    internal static SimModulePresentationSceneManager _PresentationSceneManager;
    internal static SimModuleRandom _Random;
    internal static SimModuleSceneLoader _SceneLoader;
    internal static SimModuleSerializer _Serializer;
    internal static SimModuleTicker _Ticker;
    internal static SimModuleWorldSearcher _WorldSearcher;

    internal static SimModuleBase[] _Modules;

    internal static bool IsInitialized => _World != null;
}
