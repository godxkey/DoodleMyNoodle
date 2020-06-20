using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityX;

public abstract class GameStateLobbyOnlineBase : GameState
{
    GameStateDefinitionLobbyBase _specificDefinition;

    public override void SetDefinition(GameStateDefinition definition)
    {
        base.SetDefinition(definition);

        _specificDefinition = (GameStateDefinitionLobbyBase)definition;
    }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.OnlineInterface == null)
        {
            GameStateManager.TransitionToState(_specificDefinition.gameStateIfReturn);
            Log.Error("[" + ToString() + "] Failed to get onlineInterface." +
                "This game state should be reachable if a valid onlineInterface is available.");
            return;
        }

        OnlineService.OnlineInterface.OnTerminate += OnOnlineInterfaceTerminate;
    }

    public override void BeginExit(GameStateParam[] parameters)
    {
        base.BeginExit(parameters);

        if (OnlineService.OnlineInterface != null)
        {
            OnlineService.OnlineInterface.OnTerminate -= OnOnlineInterfaceTerminate;
        }
    }

    public virtual void Return()
    {
        GameStateManager.TransitionToState(_specificDefinition.gameStateIfReturn);
    }

    protected virtual void OnOnlineInterfaceTerminate()
    {
        GameStateManager.TransitionToState(_specificDefinition.gameStateIfReturn);

        DebugScreenMessage.DisplayMessage("Disconnected from online service. Check your connection.");
        Log.Warning("[" + ToString() + "] OnlineInterface has terminated itself without user intent.");
    }
}
