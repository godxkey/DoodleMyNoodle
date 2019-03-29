using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerRepertoire_ToDelete : MonoBehaviour
{
    public abstract PlayerInfo GetLocalPlayerInfo();
    public abstract PlayerInfo GetPlayerInfo(PlayerId playerId);



    protected SessionInterface _sessionInterface;

    void Update()
    {
        if (_sessionInterface == null)
        {
            FetchSessionInterface();
        }
    }

    void OnDisable()
    {
        if (!ApplicationUtilityService.ApplicationIsQuitting && _sessionInterface != null)
        {
            UnbindFromSession();
        }
    }

    void FetchSessionInterface()
    {
        GameStateInGameOnline gameStateOnline = GameStateManager.GetCurrentGameState<GameStateInGameOnline>();

        if (gameStateOnline != null && gameStateOnline.sessionInterface != null)
        {
            _sessionInterface = gameStateOnline.sessionInterface;
            BindToSession();
        }
    }

    void BindToSession()
    {
        _sessionInterface.onConnectionAdded += OnConnectionAdded;
        _sessionInterface.onConnectionRemoved += OnConnectionRemoved;
        _sessionInterface.onTerminate += OnSessionInterfaceTerminating;
        _sessionInterface.RegisterNetMessageReceiver<PlayerInfo>(OnReceivePlayerInfo);

        OnBindedToSession();
    }

    void UnbindFromSession()
    {
        OnUnbindedFromSession();

        _sessionInterface.onConnectionAdded -= OnConnectionAdded;
        _sessionInterface.onConnectionRemoved -= OnConnectionRemoved;
        _sessionInterface.onTerminate -= OnSessionInterfaceTerminating;
        _sessionInterface.UnregisterNetMessageReceiver<PlayerInfo>(OnReceivePlayerInfo);
        _sessionInterface = null;
    }


    void OnSessionInterfaceTerminating()
    {
        _sessionInterface = null;
    }

    protected PlayerInfo MakePlayerInfoFromLocalPlayerProfile()
    {
        PlayerInfo playerInfo = new PlayerInfo();
        playerInfo.playerId = PlayerId.invalid;
        playerInfo.playerName = PlayerProfileService.Instance.playerName;

        return playerInfo;
    }


    protected virtual void OnBindedToSession() { }
    protected virtual void OnUnbindedFromSession() { }
    protected virtual void OnReceivePlayerInfo(PlayerInfo playerInfo, INetworkInterfaceConnection source) { }
    protected virtual void OnConnectionAdded(INetworkInterfaceConnection newConnection) { }
    protected virtual void OnConnectionRemoved(INetworkInterfaceConnection obj) { }
}
