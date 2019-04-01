﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateRootMenu : GameState<GameStateDefinitionRootMenu>
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

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);
        OnlineService.SetTargetRole(OnlineRole.None); // disconnect if we were connected
    }

    public void JoinLobbyAsClient()
    {
        chosenRole = Role.Client;

        WaitSpinnerService.Enable(this);
        OnlineService.SetTargetRole(OnlineRole.Client); // connect as client
        _requestTimeoutRemaining = specificDefinition.onlineRolePickTimeout;
    }

    public void JoinLobbyAsServer()
    {
        chosenRole = Role.Server;
        WaitSpinnerService.Enable(this);
        OnlineService.SetTargetRole(OnlineRole.Server); // connect as server
        _requestTimeoutRemaining = specificDefinition.onlineRolePickTimeout;
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
                        GameStateManager.TransitionToState(specificDefinition.gameStateIfClient);
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
                        GameStateManager.TransitionToState(specificDefinition.gameStateIfServer);
                    }
                }
                break;
            case Role.Local:
                GameStateManager.TransitionToState(specificDefinition.gameStateIfLocal);
                break;
        }
    }

    public override void BeginExit(GameStateParam[] parameters)
    {
        base.BeginExit(parameters);
        WaitSpinnerService.Disable(this);
    }

    void OnOnlineRoleChangeTimeout()
    {
        chosenRole = Role.None;
        DebugService.LogError("[GameStateRootMenu] Timeout: Failed to change online role" +
            " within " + specificDefinition.onlineRolePickTimeout + " seconds.");
        WaitSpinnerService.Disable(this);
    }
}
