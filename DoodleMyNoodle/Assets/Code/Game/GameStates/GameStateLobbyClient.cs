using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class GameStateLobbyClient : GameStateLobbyOnlineBase
{
    Action<bool, string> _joinSessionCallback;
    GameStateDefinitionLobbyClient _specificDefinition;

    public override void SetDefinition(GameStateDefinition definition)
    {
        base.SetDefinition(definition);

        _specificDefinition = (GameStateDefinitionLobbyClient)definition;
    }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.OnlineInterface != null && OnlineService.OnlineInterface.IsClientType == false)
        {
            GameStateManager.TransitionToState(_specificDefinition.gameStateIfReturn);
            Log.Error("[GameStateLobbyClient] The available onlineInterface is of type Server. " +
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
        OnlineService.ClientInterface.ConnectToSession(session, OnJoinSessionResult);
    }

    void OnJoinSessionResult(bool success, string message)
    {
        _joinSessionCallback?.Invoke(success, message);
        WaitSpinnerService.Disable(this);

        if (success)
        {
            GameStateManager.TransitionToState(_specificDefinition.gameStateIfJoinSession);
        }
        else
        {
            DebugScreenMessage.DisplayMessage("Failed to join the session: " + message);
            Log.Info("[GameStateLobbyClient] Failed to join session: " + message);
        }
    }
}
