using System;


public class GameStateManager : MonoCoreService<GameStateManager>
{
    public GameState currentGameState { get; private set; }
    public GameState targetGameState { get; private set; } = null;
    public bool isTransitioningState => currentGameState != targetGameState;

    GameStateFactory _gameStateFactory = new GameStateFactory();

    public override void Initialize(Action<ICoreService> onComplete)
    {
        onComplete(this);
    }

    public void TransitionToState(GameStateSettings gameStateSettings)
    {
        if(gameStateSettings == null)
        {
            DebugService.LogError("[GameStateManager] Trying to transition to a null GameState");
            return;
        }

        GameState newGameState = _gameStateFactory.CreateGameState(gameStateSettings);
        newGameState.SetSettings(gameStateSettings);

        if(newGameState == null)
        {
            DebugService.LogError("[GameStateManager] Failed to create a GameState from gameStateSettings: " + gameStateSettings.GetType()
                + ". You may have forgot to register it in the factory.");
            return;
        }

        targetGameState = newGameState;

        DebugService.Log("[GameStateManager] Transitioning from "+ GetPrintGameStateName(currentGameState) + " to " + GetPrintGameStateName(targetGameState) + "...");
        currentGameState?.BeginExit();
    }

    void Update()
    {
        if (isTransitioningState)
        {
            if (currentGameState != null && currentGameState.IsExitComplete())
            {
                currentGameState = null;
            }

            if (currentGameState == null)
            {
                currentGameState = targetGameState;
                DebugService.Log("[GameStateManager] Entering " + GetPrintGameStateName(currentGameState));
                currentGameState?.Enter();
            }
        }

        currentGameState?.Update();
    }

    string GetPrintGameStateName(GameState gameState)
    {
        return gameState == null ? "null" : gameState.ToString();
    }
}
