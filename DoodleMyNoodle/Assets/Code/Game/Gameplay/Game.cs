using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static bool PlayModeLocal { get; private set; }
    public static bool PlayModeClient { get; private set; }
    public static bool PlayModeServer { get; private set; }

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

    void Awake()
    {
        s_instance = this;
    }

    void OnDestroy()
    {
        PlayModeLocal = false;
        PlayModeClient = false;
        PlayModeServer = false;
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
                    PlayModeClient = true;
                    break;
                case GameStateInGameServer serverState:
                    sceneToLoad = _serverSpecificScene;
                    PlayModeServer = true;
                    break;
                case GameStateInGameLocal localState:
                    sceneToLoad = _localSpecificScene;
                    PlayModeLocal = true;
                    break;
            }

            if(sceneToLoad != null)
            {
                _sceneLoadPromise = SceneService.LoadAsync(sceneToLoad.SceneName, LoadSceneMode.Additive, LocalPhysicsMode.Physics3D);
                _sceneLoadPromise.OnComplete += OnPlayModeSpecificSceneLoaded;
            }


            _playModeSpecificSceneRequested = true;
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
                    b.OnGameReady();
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
            foreach (GameMonoBehaviour b in GameMonoBehaviour.RegisteredBehaviours)
            {
#if DEBUG_BUILD
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameUpdate();
#if DEBUG_BUILD
                }
                catch (Exception e)
                {
                    DebugService.LogError(e.Message + " - stack:\n " + e.StackTrace);
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
#if DEBUG_BUILD
                try
                {
#endif
                    if (b.isActiveAndEnabled)
                        b.OnGameFixedUpdate();
#if DEBUG_BUILD
                }
                catch (Exception e)
                {
                    DebugService.LogError(e.Message + " - stack:\n " + e.StackTrace);
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

    

}
