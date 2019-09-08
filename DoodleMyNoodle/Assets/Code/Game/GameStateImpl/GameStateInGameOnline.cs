using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameOnline : GameStateInGameBase
{
    public SessionInterface sessionInterface { get; private set; }

    GameStateDefinitionInGameOnline specificDefinition;

    public override void SetDefinition(GameStateDefinition definition)
    {
        base.SetDefinition(definition);
        specificDefinition = (GameStateDefinitionInGameOnline)definition;
    }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.onlineInterface == null)
        {
            GameStateManager.TransitionToState(specificDefinition.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires an onlineInterface.");
            return;
        }

        sessionInterface = OnlineService.onlineInterface.SessionInterface;

        if (sessionInterface == null)
        {
            GameStateManager.TransitionToState(specificDefinition.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires a session interface.");
            return;
        }

        sessionInterface = OnlineService.onlineInterface.SessionInterface;
        sessionInterface.OnTerminate += OnSessionInterfaceTerminated;
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
            sessionInterface.OnTerminate -= OnSessionInterfaceTerminated;
            sessionInterface = null;
        }
    }
}