using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameOnline : GameStateInGameBase<GameStateSettingsInGameOnline>
{
    public override void Enter()
    {
        base.Enter();

        if(OnlineService.onlineInterface == null)
        {
            GameStateManager.TransitionToState(specificSettings.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires an onlineInterface.");
            return;
        }

        if (OnlineService.onlineInterface.sessionInterface == null)
        {
            GameStateManager.TransitionToState(specificSettings.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires a session interface.");
            return;
        }

        OnlineService.onlineInterface.sessionInterface.onTerminate += OnSessionInterfaceTerminated;
    }


    public override void BeginExit()
    {
        if (OnlineService.onlineInterface != null && OnlineService.onlineInterface.sessionInterface != null)
        {
            // remove listener
            OnlineService.onlineInterface.sessionInterface.onTerminate -= OnSessionInterfaceTerminated;
        }

        base.BeginExit();
    }

    void OnSessionInterfaceTerminated()
    {
        DebugScreenMessage.DisplayMessage("You were disconnected from the game.");
        GameStateManager.TransitionToState(specificSettings.gameStateIfDisconnect);
    }
}
