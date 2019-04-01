using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateLobbyClient : GameStateLobbyOnlineBase<GameStateDefinitionLobbyClient>
{
    Action<bool, string> _joinSessionCallback;

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.onlineInterface != null && OnlineService.onlineInterface.isClientType == false)
        {
            GameStateManager.TransitionToState(specificDefinition.gameStateIfReturn);
            DebugService.LogError("[GameStateLobbyClient] The available onlineInterface is of type Server. " +
                "This game state requires a Client type.");
            return;
        }
    }

    public override void BeginExit(GameStateParam[] parameters)
    {
        base.BeginExit(parameters);

        WaitSpinnerService.Disable(this);
    }

    public void JoinSession(INetworkInterfaceSession session, Action<bool, string> callback)
    {
        _joinSessionCallback = callback;
        WaitSpinnerService.Enable(this);
        OnlineService.clientInterface.ConnectToSession(session, OnJoinSessionResult);
    }

    void OnJoinSessionResult(bool success, string message)
    {
        _joinSessionCallback?.Invoke(success, message);
        WaitSpinnerService.Disable(this);

        if (success)
        {
            GameStateManager.TransitionToState(specificDefinition.gameStateIfJoinSession);
        }
        else
        {
            DebugScreenMessage.DisplayMessage("Failed to join the session: " + message);
            DebugService.Log("[GameStateLobbyClient] Failed to join session: " + message);
        }
    }
}
