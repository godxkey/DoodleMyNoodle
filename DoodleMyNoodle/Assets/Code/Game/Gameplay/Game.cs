
using System;
using System.Collections.Generic;
using Unity.Entities;
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

    [SerializeField] SceneInfo _localSpecificScene;
    [SerializeField] SceneInfo _serverSpecificScene;
    [SerializeField] SceneInfo _clientSpecificScene;

    static Game s_instance;

    bool _ready;
    bool _started;
    bool _playModeSpecificSceneRequested = false;
    bool _playModeSpecificSceneLoaded = false;

    ISceneLoadPromise _sceneLoadPromise;
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

        if (_sceneLoadPromise != null)
            _sceneLoadPromise.OnComplete -= OnPlayModeSpecificSceneLoaded;
    }

    void Update()
    {
        if (!_playModeSpecificSceneRequested)
        {
            SceneInfo sceneToLoad = null;
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameClient clientState:
                    sceneToLoad = _clientSpecificScene;
                    PlayingAsClient = true;
                    break;
                case GameStateInGameServer serverState:
                    sceneToLoad = _serverSpecificScene;
                    PlayingAsServer = true;
                    break;
                case GameStateInGameLocal localState:
                    sceneToLoad = _localSpecificScene;
                    PlayingAsLocal = true;
                    break;
            }

            if(sceneToLoad != null)
            {
                _sceneLoadPromise = SceneService.LoadAsync(sceneToLoad.SceneName, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
                _sceneLoadPromise.OnComplete += OnPlayModeSpecificSceneLoaded;
                _playModeSpecificSceneRequested = true;
            }
        }


        if (_playModeSpecificSceneLoaded && !_ready)
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
            for (int i = 0; i < GameSystem.unreadySystems.Count; i++)
            {
                if (GameSystem.unreadySystems[i].SystemReady)
                {
                    GameSystem.unreadySystems.RemoveAt(i);
                    i--;
                }
            }

            if (GameSystem.unreadySystems.Count == 0)
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

    void OnPlayModeSpecificSceneLoaded(ISceneLoadPromise sceneLoadPromise)
    {
        _playModeSpecificSceneLoaded = true;
        _sceneLoadPromise = null;
    }

    int _lateStarterIterator;

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
        if(index != -1)
        {
            if(s_instance._lateStarterIterator >= index)
            {
                s_instance._lateStarterIterator--;
            }
            s_instance._lateStarters.RemoveAt(index);
        }
    }
}
