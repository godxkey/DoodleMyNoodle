using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerRepertoireSystem : GameSystem<PlayerRepertoireSystem>
{
    public ReadOnlyList<PlayerInfo> Players => _players.AsReadOnlyNoAlloc();

    public PlayerInfo GetLocalPlayerInfo()
    {
        return _localPlayerInfo;
    }

    public PlayerInfo GetPlayerInfo(PlayerId playerId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].PlayerId == playerId)
            {
                return _players[i];
            }
        }
        return null;
    }

    public bool IsLocalPlayer(PlayerId id)
    {
        return id.IsValid && _localPlayerInfo.PlayerId == id;
    }

    public PlayerInfo GetServerPlayerInfo()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].IsServer)
                return _players[i];
        }

        return null;
    }

    protected int GetIndexOfPlayer(in PlayerId playerId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].PlayerId == playerId)
                return i;
        }

        return -1;
    }

    protected SessionInterface SessionInterface { get; private set; }
    [SerializeField] // for display purposes
    protected List<PlayerInfo> _players;
    protected PlayerInfo _localPlayerInfo = new PlayerInfo();

    public override void OnGameReady()
    {
        _localPlayerInfo.PlayerName = PlayerProfileService.Instance.playerName;

        GameStateInGameOnline gameStateOnline = GameStateManager.GetCurrentGameState<GameStateInGameOnline>();

        if (gameStateOnline != null && gameStateOnline.SessionInterface != null)
        {
            SessionInterface = gameStateOnline.SessionInterface;
            BindToSession();
        }

        Internal_OnGameReady();
    }

    public override void OnSafeDestroy()
    {
        if(SessionInterface != null)
        {
            UnbindFromSession();
        }
    }

    void BindToSession()
    {
        SessionInterface.OnTerminate += OnSessionInterfaceTerminating;

        OnBindedToSession();
    }

    void UnbindFromSession()
    {
        OnUnbindedFromSession();

        SessionInterface.OnTerminate -= OnSessionInterfaceTerminating;
        SessionInterface = null;
    }

    void OnSessionInterfaceTerminating()
    {
        SessionInterface = null;
    }

    protected virtual void OnBindedToSession() { }
    protected virtual void OnUnbindedFromSession() { }
    protected virtual void Internal_OnGameReady() { }
}