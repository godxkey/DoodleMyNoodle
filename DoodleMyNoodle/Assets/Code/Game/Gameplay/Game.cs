using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static bool playModeLocal { get; private set; }
    public static bool playModeClient { get; private set; }
    public static bool playModeServer { get; private set; }

    public static void AddOnReadyCallback(Action cb)
    {
        if (_instance == null)
        {
            DebugService.LogError("[AddOnReadyCallback] Game instance is null. Cannot add callback");
            return;
        }

        if (_instance._ready)
        {
            cb();
        }
        else
        {
            _instance._onReady += cb;
        }
    }
    public static void AddPreReadyCallback(Action cb)
    {
        if (_instance == null)
        {
            DebugService.LogError("[AddOnReadyCallback] Game instance is null. Cannot add callback");
            return;
        }

        if (_instance._ready)
        {
            cb();
        }
        else
        {
            _instance._preReady += cb;
        }
    }

    [SerializeField] SceneInfo _localSpecificScene;
    [SerializeField] SceneInfo _serverSpecificScene;
    [SerializeField] SceneInfo _clientSpecificScene;

    static Game _instance;

    bool _ready;
    bool _playModeSpecificSceneRequested = false;
    bool _playModeSpecificSceneLoaded = false;
    event Action _preReady;
    event Action _onReady;

    void Awake()
    {
        _instance = this;
    }

    void OnDestroy()
    {
        playModeLocal = false;
        playModeClient = false;
        playModeServer = false;
        _instance = null;
    }




    ////////////////////////////////////////////////////////////////////////////////////////
    //      THIS IS UGLY TEMPORARY CODE                                 
    ////////////////////////////////////////////////////////////////////////////////////////
    [SerializeField] SceneInfo _escapeMenuScene;

    MenuInGameEscape _menuInGameEscape;

    void Start()
    {
        SceneService.LoadAsync(_escapeMenuScene, OnEscapeMenuSceneLoaded);
    }
    void OnEscapeMenuSceneLoaded(Scene scene)
    {
        _menuInGameEscape = scene.FindRootObject<MenuInGameEscape>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _menuInGameEscape?.Open();
        }

        ////////////////////////////////////////////////////////////////////////////////////////
        //      END OF UGLY CODE                                 
        ////////////////////////////////////////////////////////////////////////////////////////

        if (!_playModeSpecificSceneRequested)
        {
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameClient clientState:
                    SceneService.LoadAsync(_clientSpecificScene.SceneName, LoadSceneMode.Additive, OnPlayModeSpecificSceneLoaded);
                    playModeClient = true;
                    break;
                case GameStateInGameServer serverState:
                    SceneService.LoadAsync(_serverSpecificScene.SceneName, LoadSceneMode.Additive, OnPlayModeSpecificSceneLoaded);
                    playModeServer = true;
                    break;
                case GameStateInGameLocal localState:
                    SceneService.LoadAsync(_localSpecificScene.SceneName, LoadSceneMode.Additive, OnPlayModeSpecificSceneLoaded);
                    _ready = true;
                    break;
            }
            _playModeSpecificSceneRequested = true;
        }


        if (_playModeSpecificSceneLoaded && !_ready)
        {
            switch (GameStateManager.currentGameState)
            {
                case GameStateInGameClient clientState:
                    _ready = clientState.sessionInterface != null;
                    break;
                case GameStateInGameServer serverState:
                    _ready = serverState.sessionInterface != null;
                    break;
                case GameStateInGameLocal localState:
                    _ready = true;
                    break;
            }

            if (_ready)
            {
                // invoke 'OnReady' callbacks

                _preReady?.Invoke(); // useful so that certain systems set themselves up

                foreach (GameMonoBehaviour b in GameMonoBehaviour.registeredBehaviours)
                {
                    b.OnGamePreReady();
                }

                _onReady?.Invoke();

                foreach (GameMonoBehaviour b in GameMonoBehaviour.registeredBehaviours)
                {
                    b.OnGameReady();
                }

                _preReady = null;
                _onReady = null;
            }
        }

    }

    void OnPlayModeSpecificSceneLoaded(Scene scene)
    {
        _playModeSpecificSceneLoaded = true;
    }

}
