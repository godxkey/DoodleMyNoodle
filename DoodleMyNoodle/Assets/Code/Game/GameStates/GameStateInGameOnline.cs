using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityX;

public class GameStateInGameOnline : GameStateInGameBase
{
    public SessionInterface SessionInterface { get; private set; }

    GameStateDefinitionInGameOnline _specificDefinition;

    public override void SetDefinition(GameStateDefinition definition)
    {
        base.SetDefinition(definition);
        _specificDefinition = (GameStateDefinitionInGameOnline)definition;
    }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.OnlineInterface == null)
        {
            GameStateManager.TransitionToState(_specificDefinition.gameStateIfDisconnect);
            Log.Error("[GameStateInGameOnline] This game state requires an onlineInterface.");
            return;
        }

        SessionInterface = OnlineService.OnlineInterface.SessionInterface;

        if (SessionInterface == null)
        {
            GameStateManager.TransitionToState(_specificDefinition.gameStateIfDisconnect);
            Log.Error("[GameStateInGameOnline] This game state requires a session interface.");
            return;
        }

        SessionInterface = OnlineService.OnlineInterface.SessionInterface;
        SessionInterface.OnTerminate += OnSessionInterfaceTerminated;
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
        GameStateManager.TransitionToState(_specificDefinition.gameStateIfDisconnect);
    }

    void ClearSessionInterface()
    {
        if (SessionInterface != null)
        {
            SessionInterface.OnTerminate -= OnSessionInterfaceTerminated;
            SessionInterface = null;
        }
    }
}