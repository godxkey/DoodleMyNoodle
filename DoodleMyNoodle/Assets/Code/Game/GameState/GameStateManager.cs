using System;
using UnityEngine;

public class GameStateManager : MonoCoreService<GameStateManager>
{
    const bool log = false;

    static public void TransitionToState(GameStateDefinition gameStateSettings, params GameStateParam[] parameters)
    {
        Instance.Internal_TransitionToState(gameStateSettings, parameters);
    }

    static public GameState currentGameState => Instance._currentGameState;
    static public GameState targetGameState => Instance._targetGameState;
    static public bool isTransitioningState => currentGameState != targetGameState;
    static public T GetCurrentGameState<T>() where T : GameState => currentGameState == null ? null : currentGameState as T;

    GameState _currentGameState;
    GameState _targetGameState;
    GameStateFactory _gameStateFactory = new GameStateFactory();
    GameStateDefinitionGraph _graph;

    [SerializeField]
    [Reorderable]
    GameStateDefinition[] _gameStateDefinitions = new GameStateDefinition[0];
    GameStateParam[] _parameters;

    public override void Initialize(Action<ICoreService> onComplete)
    {
        onComplete(this);

        _graph = new GameStateDefinitionGraph(_gameStateDefinitions);

        //var path = _graph.FindPathToGameState(_graph.root.gameStateDefinition, _gameStateDefinitions.Last());

        //foreach (var item in path)
        //{
        //    Debug.Log(item);
        //}
    }

    void Internal_TransitionToState(GameStateDefinition gameStateSettings, GameStateParam[] parameters)
    {
        if (gameStateSettings == null)
        {
            DebugService.LogError("[GameStateManager] Trying to transition to a null GameState");
            return;
        }

        _parameters = parameters;

        GameState newGameState = _gameStateFactory.CreateGameState(gameStateSettings);
        newGameState.SetDefinition(gameStateSettings);

        if (newGameState == null)
        {
            DebugService.LogError("[GameStateManager] Failed to create a GameState from gameStateSettings: " + gameStateSettings.GetType()
                + ". You may have forgot to register it in the factory.");
            return;
        }

        _targetGameState = newGameState;

        if (log)
#pragma warning disable CS0162 // Unreachable code detected
            DebugService.Log("[GameStateManager] Transitioning from " + GetPrintGameStateName(_currentGameState) + " to " + GetPrintGameStateName(_targetGameState) + "...");
#pragma warning restore CS0162 // Unreachable code detected
        _currentGameState?.BeginExit(_parameters);
    }

    void Update()
    {
        bool repeat = true;
        while (repeat)
        {
            repeat = false;

            if (isTransitioningState)
            {
                if (_currentGameState != null && _currentGameState.IsExitComplete())
                {
                    _currentGameState = null;
                }

                if (_currentGameState == null)
                {
                    _currentGameState = _targetGameState;
                    if (log)
#pragma warning disable CS0162 // Unreachable code detected
                        DebugService.Log("[GameStateManager] Entering " + GetPrintGameStateName(_currentGameState));
#pragma warning restore CS0162 // Unreachable code detected
                    _currentGameState?.Enter(_parameters);

                    if (isTransitioningState)
                    {
                        repeat = true;
                    }
                }
            }
        }

        _currentGameState?.Update();
    }

    string GetPrintGameStateName(GameState gameState)
    {
        return gameState == null ? "null" : gameState.ToString();
    }
}
