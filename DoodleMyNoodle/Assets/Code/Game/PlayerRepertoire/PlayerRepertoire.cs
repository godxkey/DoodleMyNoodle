using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public abstract class PlayerRepertoire : GameMonoBehaviour
{
    public ReadOnlyCollection<PlayerInfo> players { get; private set; }

    protected SessionInterface _sessionInterface { get; private set; }
    protected List<PlayerInfo> _players { get; private set; } = new List<PlayerInfo>();
    protected PlayerInfo _localPlayerInfo { get; private set; }

    public override void OnGamePreReady()
    {
        players = _players.AsReadOnly();
        _localPlayerInfo = new PlayerInfo();
        _localPlayerInfo.playerName = PlayerProfileService.Instance.playerName;

        GameStateInGameOnline gameStateOnline = GameStateManager.GetCurrentGameState<GameStateInGameOnline>();

        if (gameStateOnline != null && gameStateOnline.sessionInterface != null)
        {
            _sessionInterface = gameStateOnline.sessionInterface;
            BindToSession();
        }

        OnPreReady();
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

    public PlayerInfo GetLocalPlayerInfo()
    {
        return _localPlayerInfo;
    }
    public PlayerInfo GetPlayerInfo(PlayerId playerId)
    {
        for (int i = 0; i < _players.Count; i++)
        {
            if (_players[i].playerId == playerId)
                return _players[i];
        }
        return null;
    }

    protected virtual void OnBindedToSession() { }
    protected virtual void OnUnbindedFromSession() { }
    protected virtual void OnPreReady() { }
}