using System;
using UnityEngineX;

public class GameStateLobbyServer : GameStateLobbyOnlineBase
{
    Action<bool, string> _createSessionCallback;
    GameStateDefinitionLobbyServer _specificDefinition;

    public override void SetDefinition(GameStateDefinition definition)
    {
        base.SetDefinition(definition);

        _specificDefinition = (GameStateDefinitionLobbyServer)definition;
    }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        if (OnlineService.OnlineInterface != null && OnlineService.OnlineInterface.IsServerType == false)
        {
            GameStateManager.TransitionToState(_specificDefinition.gameStateIfReturn);
            Log.Error("[GameStateLobbyServer] The available onlineInterface is of type Client. " +
                "This game state requires a Server type.");
            return;
        }
    }

    public override void BeginExit(GameStateParam[] parameters)
    {
        base.BeginExit(parameters);

        WaitSpinnerService.Disable(this);
    }

    public void CreateSession(string sessionName, Action<bool, string> callback)
    {
        _createSessionCallback = callback;
        WaitSpinnerService.Enable(this);
        OnlineService.ServerInterface.CreateSession(sessionName, OnJoinSessionResult);
    }

    void OnJoinSessionResult(bool success, string message)
    {
        _createSessionCallback?.Invoke(success, message);
        WaitSpinnerService.Disable(this);

        if (success)
        {
            GameStateManager.TransitionToState(_specificDefinition.gameStateIfCreateSession);
        }
        else
        {
            DebugScreenMessage.DisplayMessage("Failed to create the session: " + message);
            Log.Info("[GameStateLobbyServer] Failed to create session: " + message);
        }
    }
}
