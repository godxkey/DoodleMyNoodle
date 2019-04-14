using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class PlayerRepertoireSystem : GameSystem<PlayerRepertoireSystem>
{
    public ReadOnlyCollection<PlayerInfo> players { get; private set; }

    public PlayerInfo GetLocalPlayerInfo()
    {
        return _localPlayerInfo;
    }
    public abstract PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection);
    public PlayerInfo GetPlayerInfo(PlayerId playerId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].playerId == playerId)
            {
                return _players[i];
            }
        }
        return null;
    }
    public bool IsLocalPlayer(PlayerId id)
    {
        return id.isValid && _localPlayerInfo.playerId == id;
    }
    public PlayerInfo GetServerPlayerInfo()
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].isServer)
                return _players[i];
        }

        return null;
    }

    protected SessionInterface _sessionInterface { get; private set; }
    protected List<PlayerInfo> _players { get; private set; } = new List<PlayerInfo>();
    protected PlayerInfo _localPlayerInfo = new PlayerInfo();

    public override void OnGameReady()
    {
        players = _players.AsReadOnly();
        _localPlayerInfo.playerName = PlayerProfileService.Instance.playerName;

        GameStateInGameOnline gameStateOnline = GameStateManager.GetCurrentGameState<GameStateInGameOnline>();

        if (gameStateOnline != null && gameStateOnline.sessionInterface != null)
        {
            _sessionInterface = gameStateOnline.sessionInterface;
            BindToSession();
        }

        Internal_OnGameReady();
    }

    public override void OnSafeDestroy()
    {
        if(_sessionInterface != null)
        {
            UnbindFromSession();
        }
    }

    void BindToSession()
    {
        _sessionInterface.onTerminate += OnSessionInterfaceTerminating;

        OnBindedToSession();
    }

    void UnbindFromSession()
    {
        OnUnbindedFromSession();

        _sessionInterface.onTerminate -= OnSessionInterfaceTerminating;
        _sessionInterface = null;
    }

    void OnSessionInterfaceTerminating()
    {
        _sessionInterface = null;
    }

    protected virtual void OnBindedToSession() { }
    protected virtual void OnUnbindedFromSession() { }
    protected virtual void Internal_OnGameReady() { }
}