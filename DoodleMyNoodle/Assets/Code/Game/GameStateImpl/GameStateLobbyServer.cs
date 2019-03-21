using System;

public class GameStateLobbyServer : GameStateLobbyOnlineBase<GameStateSettingsLobbyServer>
{
    Action<bool, string> _createSessionCallback;

    public override void Enter()
    {
        base.Enter();

        if (OnlineService.onlineInterface != null && OnlineService.onlineInterface.isServerType == false)
        {
            GameStateManager.TransitionToState(specificSettings.gameStateIfReturn);
            DebugService.LogError("[GameStateLobbyServer] The available onlineInterface is of type Client. " +
                "This game state requires a Server type.");
            return;
        }
    }

    public override void BeginExit()
    {
        base.BeginExit();

        WaitSpinnerService.Disable(this);
    }

    public void CreateSession(string sessionName, Action<bool, string> callback)
    {
        _createSessionCallback = callback;
        WaitSpinnerService.Enable(this);
        OnlineService.serverInterface.CreateSession(sessionName, OnJoinSessionResult);
    }

    void OnJoinSessionResult(bool success, string message)
    {
        _createSessionCallback?.Invoke(success, message);
        WaitSpinnerService.Disable(this);

        if (success)
        {
            GameStateManager.TransitionToState(specificSettings.gameStateIfCreateSession);
        }
        else
        {
            DebugScreenMessage.DisplayMessage("Failed to create the session: " + message);
            DebugService.Log("[GameStateLobbyServer] Failed to create session: " + message);
        }
    }
}
