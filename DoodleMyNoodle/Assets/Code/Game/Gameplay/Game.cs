
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineX;

public class Game : MonoBehaviour
{
    public static bool PlayingAsLocal { get; private set; }
    public static bool PlayingAsClient { get; private set; }
    public static bool PlayingAsServer { get; private set; }
    public static bool PlayingAsMaster => PlayingAsServer || PlayingAsLocal;

    public static bool Ready => s_instance && s_instance._ready;
    public static bool Started => s_instance && s_instance._started;

    [SerializeField] GameSystemBank _systemBank;
    [SerializeField] RectTransform _sharedCanvas;

    static Game s_instance;

    bool _ready;
    bool _started;
    bool _systemsCreated = false;

    List<GameMonoBehaviour> _lateStarters = new List<GameMonoBehaviour>();


    void Awake()
    {
        s_instance = this;
    }

    void OnDestroy()
    {
        PlayingAsLocal = false;
        PlayingAsClient = false;
        PlayingAsServer = false;
        s_instance = null;
    }

    void Update()
    {
        if (!_systemsCreated)
        {
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameClient clientState:
                    PlayingAsClient = true;
                    break;
                case GameStateInGameServer serverState:
                    PlayingAsServer = true;
                    break;
                case GameStateInGameLocal localState:
                    PlayingAsLocal = true;
                    break;
            }

            if (PlayingAsServer || PlayingAsLocal || PlayingAsClient)
            {
                _systemsCreated = true;
            
                _gameContentScene = SceneManager.CreateScene("Game Content (dynamic)");
                _gameSystemScene = SceneManager.CreateScene("Game Systems (dynamic)");

                if (PlayingAsLocal)
                    InstantiateCoreSystems(GameSystemOnlineFlags.Local);
            
                if (PlayingAsServer)
                    InstantiateCoreSystems(GameSystemOnlineFlags.Server);
            
                if (PlayingAsClient)
                    InstantiateCoreSystems(GameSystemOnlineFlags.Client);
            }
        }


        if (_systemsCreated && !_ready)
        {
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameClient clientState:
                    _ready = clientState.SessionInterface != null;
                    break;
                case GameStateInGameServer serverState:
                    _ready = serverState.SessionInterface != null;
                    break;
                case GameStateInGameLocal localState:
                    _ready = true;
                    break;
            }

            if (_ready)
            {
                // invoke 'OnReady' callbacks

                foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
                {
                    b.OnGameAwake();
                }
            }
        }

        if (_ready && !_started)
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
                _started = true;

                foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
                {
                    b.OnGameStart();
                }
            }
        }


        if (_started)
        {
            ExecuteLateStarter();

            foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
            {
#if DEBUG
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameUpdate();
#if DEBUG
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
#if DEBUG
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameFixedUpdate();
#if DEBUG
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
#if DEBUG
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameLateUpdate();
#if DEBUG
                }
                catch (Exception e)
                {
                    Log.Error(e.Message + " - stack:\n " + e.StackTrace);
                }
#endif

            }
        }
    }

    int _lateStarterIterator;
    private List<GameSystem> _systems;
    private Scene _gameContentScene;
    private Scene _gameSystemScene;

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
        foreach (var item in _systemBank.Prefabs)
        {
            if (!item)
                continue;

            if (predicate(item))
            {
                // Instantiate UI system
                if (item.HasComponent<RectTransform>() && !item.HasComponent<Canvas>())
                {
                    Instantiate(item, _sharedCanvas);
                }
                // Instantiate normal system
                else
                {
                    Instantiate(item);
                }
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
