using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameOnline<SettingsClass> : GameStateInGameBase<SettingsClass>, IGameStateInGameOnline
    where SettingsClass : GameStateDefinitionInGameOnline
{
    public SessionInterface sessionInterface { get; private set; }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.onlineInterface == null)
        {
            GameStateManager.TransitionToState(specificDefinition.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires an onlineInterface.");
            return;
        }

        sessionInterface = OnlineService.onlineInterface.sessionInterface;

        if (sessionInterface == null)
        {
            GameStateManager.TransitionToState(specificDefinition.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires a session interface.");
            return;
        }

        sessionInterface = OnlineService.onlineInterface.sessionInterface;
        sessionInterface.onTerminate += OnSessionInterfaceTerminated;
    }


    public override void BeginExit(GameStateParam[] parameters)
    {
        ClearSessionInterface();
        base.BeginExit(parameters);
    }

    void OnSessionInterfaceTerminated()
    {
        ClearSessionInterface();

        DebugScreenMessage.DisplayMessage("You were disconnected from the game.");
        GameStateManager.TransitionToState(specificDefinition.gameStateIfDisconnect);
    }

    void ClearSessionInterface()
    {
        if (sessionInterface != null)
        {
            sessionInterface.onTerminate -= OnSessionInterfaceTerminated;
            sessionInterface = null;
        }
    }
}


public interface IGameStateInGameOnline
{
    SessionInterface sessionInterface { get; }
}