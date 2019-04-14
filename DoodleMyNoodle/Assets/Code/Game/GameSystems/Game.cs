using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
    public static bool playModeLocal { get; private set; }
    public static bool playModeClient { get; private set; }
    public static bool playModeServer { get; private set; }

    [SerializeField] SceneInfo _localSpecificScene;
    [SerializeField] SceneInfo _serverSpecificScene;
    [SerializeField] SceneInfo _clientSpecificScene;

    static Game _instance;

    bool _ready;
    bool _started;
    bool _playModeSpecificSceneRequested = false;
    bool _playModeSpecificSceneLoaded = false;

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

    void Update()
    {
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

                foreach (GameMonoBehaviour b in GameMonoBehaviour.registeredBehaviours)
                {
                    b.OnGameReady();
                }
            }
        }

        if (_ready && !_started)
        {
            for (int i = 0; i < GameSystem.unreadySystems.Count; i++)
            {
                if (GameSystem.unreadySystems[i].isSystemReady)
                {
                    GameSystem.unreadySystems.RemoveAt(i);
                    i--;
                }
            }

            if (GameSystem.unreadySystems.Count == 0)
            {
                _started = true;

                foreach (GameMonoBehaviour b in GameMonoBehaviour.registeredBehaviours)
                {
                    b.OnGameStart();
                }
            }
        }


        if (_started)
        {
            foreach (GameMonoBehaviour b in GameMonoBehaviour.registeredBehaviours)
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
                    DebugService.LogError(e.Message);
                }
#endif

            }
        }
    }

    void OnPlayModeSpecificSceneLoaded(Scene scene)
    {
        _playModeSpecificSceneLoaded = true;
    }

}
