using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GameStateLobbyOnlineBase<SettingsClass> : GameState<SettingsClass>
    where SettingsClass : GameStateDefinitionLobbyBase
{
    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.onlineInterface == null)
        {
            GameStateManager.TransitionToState(specificDefinition.gameStateIfReturn);
            DebugService.LogError("[" + ToString() + "] Failed to get onlineInterface." +
                "This game state should be reachable if a valid onlineInterface is available.");
            return;
        }

        OnlineService.onlineInterface.onTerminate += OnOnlineInterfaceTerminate;
    }

    public override void BeginExit(GameStateParam[] parameters)
    {
        base.BeginExit(parameters);

        if (OnlineService.onlineInterface != null)
        {
            OnlineService.onlineInterface.onTerminate -= OnOnlineInterfaceTerminate;
        }
    }

    public virtual void Return()
    {
        GameStateManager.TransitionToState(specificDefinition.gameStateIfReturn);
    }

    protected virtual void OnOnlineInterfaceTerminate()
    {
        GameStateManager.TransitionToState(specificDefinition.gameStateIfReturn);

        DebugScreenMessage.DisplayMessage("Disconnected from online service. Check your connection.");
        DebugService.LogWarning("[" + ToString() + "] OnlineInterface has terminated itself without user intent.");
    }
}
