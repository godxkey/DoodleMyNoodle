using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateRootMenu : GameState<GameStateSettingsRootMenu>
{
    enum Role
    {
        None,
        Client,
        Server,
        Local
    }
    Role chosenRole = Role.None;

    float _requestTimeoutRemaining;

    public override void Enter()
    {
        base.Enter();
        OnlineService.SetTargetRole(OnlineRole.None); // disconnect if we were connected
    }

    public void JoinLobbyAsClient()
    {
        chosenRole = Role.Client;

        WaitSpinnerService.Enable(this);
        OnlineService.SetTargetRole(OnlineRole.Client); // connect as client
        _requestTimeoutRemaining = specificSettings.onlineRolePickTimeout;
    }

    public void JoinLobbyAsServer()
    {
        chosenRole = Role.Server;
        WaitSpinnerService.Enable(this);
        OnlineService.SetTargetRole(OnlineRole.Server); // connect as server
        _requestTimeoutRemaining = specificSettings.onlineRolePickTimeout;
    }

    public void JoinLobbyAsSolo()
    {
        chosenRole = Role.Local;
    }

    public override void Update()
    {
        base.Update();

        _requestTimeoutRemaining -= Time.deltaTime;

        switch (chosenRole)
        {
            case Role.Client:
                if (_requestTimeoutRemaining <= 0)
                {
                    OnOnlineRoleChangeTimeout();
                }
                else
                {
                    if (OnlineService.onlineInterface != null && OnlineService.onlineInterface.isClientType)
                    {
                        GameStateManager.TransitionToState(specificSettings.gameStateIfClient);
                    }
                }
                break;
            case Role.Server:
                if (_requestTimeoutRemaining <= 0)
                {
                    OnOnlineRoleChangeTimeout();
                }
                else
                {
                    if (OnlineService.onlineInterface != null && OnlineService.onlineInterface.isServerType)
                    {
                        GameStateManager.TransitionToState(specificSettings.gameStateIfServer);
                    }
                }
                break;
            case Role.Local:
                GameStateManager.TransitionToState(specificSettings.gameStateIfLocal);
                break;
        }
    }

    public override void BeginExit()
    {
        base.BeginExit();
        WaitSpinnerService.Disable(this);
    }

    void OnOnlineRoleChangeTimeout()
    {
        chosenRole = Role.None;
        DebugService.LogError("[GameStateRootMenu] Timeout: Failed to change online role" +
            " within " + specificSettings.onlineRolePickTimeout + " seconds.");
        WaitSpinnerService.Disable(this);
    }
}
