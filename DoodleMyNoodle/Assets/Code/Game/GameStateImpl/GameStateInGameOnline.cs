using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameOnline : GameStateInGameBase<GameStateSettingsInGameOnline>
{
    public SessionInterface sessionInterface { get; private set; }

    public override void Enter()
    {
        base.Enter();

        if (OnlineService.onlineInterface == null)
        {
            GameStateManager.TransitionToState(specificSettings.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires an onlineInterface.");
            return;
        }

        sessionInterface = OnlineService.onlineInterface.sessionInterface;

        if (sessionInterface == null)
        {
            GameStateManager.TransitionToState(specificSettings.gameStateIfDisconnect);
            DebugService.LogError("[GameStateInGameOnline] This game state requires a session interface.");
            return;
        }

        sessionInterface = OnlineService.onlineInterface.sessionInterface;
        sessionInterface.onTerminate += OnSessionInterfaceTerminated;
    }


    public override void BeginExit()
    {
        ClearSessionInterface();
        base.BeginExit();
    }

    void OnSessionInterfaceTerminated()
    {
        ClearSessionInterface();

        DebugScreenMessage.DisplayMessage("You were disconnected from the game.");
        GameStateManager.TransitionToState(specificSettings.gameStateIfDisconnect);
    }

    void ClearSessionInterface()
    {
        if(sessionInterface != null)
        {
            sessionInterface.onTerminate -= OnSessionInterfaceTerminated;
            sessionInterface = null;
        }
    }
}
