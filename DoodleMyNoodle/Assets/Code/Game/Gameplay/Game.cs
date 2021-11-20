
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Scenes;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineX;

public class Game : MonoBehaviour
{
    static class WorldBootstrap
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Initialize()
        {
            SimulationController.Initialize();
            DefaultWorldInitialization.Initialize("Default World", false);
            GameObjectSceneUtility.AddGameObjectSceneReferences();
        }
    }

    public static bool PlayingAsLocal { get; private set; }
    public static bool PlayingAsClient { get; private set; }
    public static bool PlayingAsServer { get; private set; }
    public static bool PlayingAsMaster => PlayingAsServer || PlayingAsLocal;

    public static bool Ready => s_instance && s_instance._ready;
    public static bool Started => s_instance && s_instance._started;
    public static bool CallingOnGameStart => s_instance && s_instance._callingOnGameStart;

    [SerializeField] GameSystemBank _systemBank;

    static Game s_instance;

    bool _ready;
    bool _started;
    bool _callingOnGameStart = false;

    int _lateStarterIterator;
    Scene _gameContentScene;
    Scene _gameSystemScene;

    List<GameMonoBehaviour> _lateStarters = new List<GameMonoBehaviour>();
    bool _fireOnGameAwake;
    bool _fireOnGameStart;

    void Awake()
    {
        s_instance = this;
    }

    private void Start()
    {
        StartCoroutine(Init());
    }

    void OnDestroy()
    {
        PlayingAsLocal = false;
        PlayingAsClient = false;
        PlayingAsServer = false;
        s_instance = null;
    }

    private IEnumerator Init()
    {
        bool roleSelected = false;
        while (!roleSelected)
        {
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameClient _:
                    Log.Info("Starting game as client");
                    PlayingAsClient = true;
                    roleSelected = true;
                    break;

                case GameStateInGameServer _:
                    Log.Info("Starting game as server");
                    PlayingAsServer = true;
                    roleSelected = true;
                    break;

                case GameStateInGameLocal _:
                    Log.Info("Starting game in local play");
                    PlayingAsLocal = true;
                    roleSelected = true;
                    break;

                default:
                {
                    yield return null;
                    break;
                }
            }
        }


        // Load scenes
        Log.Info("Loading dynamic game scenes...");
        var contentSceneLoad = SceneManager.LoadSceneAsync("Game Content (dynamic)", LoadSceneMode.Additive);
        var systemSceneLoad = SceneManager.LoadSceneAsync("Game Systems (dynamic)", LoadSceneMode.Additive);
        
        while (!contentSceneLoad.isDone || !systemSceneLoad.isDone)
        {
            yield return null;
        }

        _gameContentScene = SceneManager.GetSceneByName("Game Content (dynamic)");
        _gameSystemScene = SceneManager.GetSceneByName("Game Systems (dynamic)");


        // Instantiate game systems
        Log.Info("Instantiating game systems...");
        if (PlayingAsLocal)
            InstantiateCoreSystems(GameSystemOnlineFlags.Local);

        if (PlayingAsServer)
            InstantiateCoreSystems(GameSystemOnlineFlags.Server);

        if (PlayingAsClient)
            InstantiateCoreSystems(GameSystemOnlineFlags.Client);


        // wait for correct session configuration
        while (true)
        {
            bool ready = false;
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameClient clientState:
                    ready = clientState.SessionInterface != null;
                    break;

                case GameStateInGameServer serverState:
                    ready = serverState.SessionInterface != null;
                    break;

                case GameStateInGameLocal _:
                    ready = true;
                    break;
            }

            if (ready)
            {
                break;
            }
            else
            {
                yield return null;
            }
        }

        Log.Info("Waiting for game systems to be ready...");

        // Fire awake!
        _fireOnGameAwake = true;

        // Wait for all SystemReady
        while (true)
        {
            for (int i = 0; i < GameSystem.s_unreadySystems.Count; i++)
            {
                if (GameSystem.s_unreadySystems[i].SystemReady)
                {
                    GameSystem.s_unreadySystems.RemoveAt(i);
                    i--;
                }
            }

            if (GameSystem.s_unreadySystems.Count == 0)
            {
                break;
            }
            else
            {
                yield return null;
            }
        }

        Log.Info("Game starting!");
        _fireOnGameStart = true;
    }

    void Update()
    {
        if (_fireOnGameAwake)
        {
            _fireOnGameAwake = false;
            _ready = true;

            foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
            {
                b.OnGameAwake();
            }
        }

        if (_fireOnGameStart)
        {
            _fireOnGameStart = false;
            _started = true;
            _callingOnGameStart = true;

            foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
            {
                b.OnGameStart();
            }

            _callingOnGameStart = false;
        }

        if (_started)
        {
            ExecuteLateStarter();

            foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
            {
#if SAFETY
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameUpdate();
#if SAFETY
                }
                catch (Exception e)
                {
                    Log.Exception(e);
                }
#endif
            }
        }
    }

    private void FixedUpdate()
    {
        if (_started)
        {
            foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
            {
#if SAFETY
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameFixedUpdate();
#if SAFETY
                }
                catch (Exception e)
                {
                    Log.Error(e.Message + " - stack:\n " + e.StackTrace);
                }
#endif

            }
        }
    }

    private void LateUpdate()
    {
        if (_started)
        {
            foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
            {
#if SAFETY
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameLateUpdate();
#if SAFETY
                }
                catch (Exception e)
                {
                    Log.Error(e.Message + " - stack:\n " + e.StackTrace);
                }
#endif

            }
        }
    }

    public static void AddLateStarter(GameMonoBehaviour gameMonoBehaviour)
    {
        if (!s_instance)
            throw new Exception("Game instance is null");

        s_instance._lateStarters.Add(gameMonoBehaviour);
    }

    private void ExecuteLateStarter()
    {
        _lateStarterIterator = 0;

        for (; _lateStarterIterator < _lateStarters.Count; _lateStarterIterator++)
        {
            _lateStarters[_lateStarterIterator].OnGameStart();
        }

        _lateStarterIterator = -1;

        _lateStarters.Clear();
    }

    public static void RemoveLateStarter(GameMonoBehaviour gameMonoBehaviour)
    {
        if (!s_instance)
        {
            return;
        }

        int index = s_instance._lateStarters.IndexOf(gameMonoBehaviour);
        if (index != -1)
        {
            if (s_instance._lateStarterIterator >= index)
            {
                s_instance._lateStarterIterator--;
            }
            s_instance._lateStarters.RemoveAt(index);
        }
    }

    private void InstantiateCoreSystems(GameSystemOnlineFlags onlineFlags)
    {
        InstantiateSystems((sys) => sys.SystemSettings.Type == GameSystemType.Core && (sys.SystemSettings.OnlineFlags & onlineFlags) != 0);
    }

    private void InstantiateSystems(Predicate<GameSystem> predicate)
    {
        SceneManager.SetActiveScene(_gameSystemScene);
        foreach (GameSystem systemPrefab in _systemBank.Prefabs)
        {
            if (!systemPrefab)
                continue;

            if (predicate(systemPrefab))
            {
                Instantiate(systemPrefab);
            }
        }
        SceneManager.SetActiveScene(_gameContentScene);
    }

    public static void InstantiateGameplayPresentationSystems()
    {
        if (s_instance == null)
            return;

        s_instance.InstantiateSystems((sys) => sys.SystemSettings.Type == GameSystemType.GameplayPresentation);
    }
}
