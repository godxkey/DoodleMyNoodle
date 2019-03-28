using System;


public class GameStateManager : MonoCoreService<GameStateManager>
{
    static public void TransitionToState(GameStateSettings gameStateSettings)
    {
        Instance.Internal_TransitionToState(gameStateSettings);
    }

    static public GameState currentGameState => Instance._currentGameState;
    static public GameState targetGameState => Instance._targetGameState;
    static public bool isTransitioningState => currentGameState != targetGameState;

    GameState _currentGameState;
    GameState _targetGameState;
    GameStateFactory _gameStateFactory = new GameStateFactory();

    public override void Initialize(Action<ICoreService> onComplete)
    {
        onComplete(this);
    }

    void Internal_TransitionToState(GameStateSettings gameStateSettings)
    {
        if (gameStateSettings == null)
        {
            DebugService.LogError("[GameStateManager] Trying to transition to a null GameState");
            return;
        }

        GameState newGameState = _gameStateFactory.CreateGameState(gameStateSettings);
        newGameState.SetSettings(gameStateSettings);

        if (newGameState == null)
        {
            DebugService.LogError("[GameStateManager] Failed to create a GameState from gameStateSettings: " + gameStateSettings.GetType()
                + ". You may have forgot to register it in the factory.");
            return;
        }

        _targetGameState = newGameState;

        DebugService.Log("[GameStateManager] Transitioning from " + GetPrintGameStateName(_currentGameState) + " to " + GetPrintGameStateName(_targetGameState) + "...");
        _currentGameState?.BeginExit();
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
                    DebugService.Log("[GameStateManager] Entering " + GetPrintGameStateName(_currentGameState));
                    _currentGameState?.Enter();

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
