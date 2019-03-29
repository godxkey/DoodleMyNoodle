using System;
using System.Collections.Generic;

public class PlayerRepertoireClient : PlayerRepertoire_ToDelete
{
    List<PlayerInfo> _playerInfos;
    PlayerInfo _localPlayerInfo;
    bool _iHaveSentMyInfo = false;

    public override PlayerInfo GetLocalPlayerInfo()
    {
        return _playerInfos[0];
    }

    public override PlayerInfo GetPlayerInfo(PlayerId playerId)
    {
        for (int i = 0; i < _playerInfos.Count; i++)
        {
            if(_playerInfos[i].playerId == playerId)
            {
                return _playerInfos[i];
            }
        }
        return null;
    }

    protected override void OnBindedToSession()
    {
        base.OnBindedToSession();

        _localPlayerInfo = MakePlayerInfoFromLocalPlayerProfile();

        foreach (INetworkInterfaceConnection connection in _sessionInterface.connections)
        {
            OnConnectionAdded(connection);
        }
    }

    protected override void OnConnectionAdded(INetworkInterfaceConnection newConnection)
    {
        base.OnConnectionAdded(newConnection);

        if (_iHaveSentMyInfo)
        {
            DebugService.LogWarning("[PlayerRepertoireClient] We're sending our info twice. " +
                "It appears we have been connected to more than one entity, which should not happen when we're a client");
        }

        _sessionInterface.SendNetMessage(newConnection, _localPlayerInfo);
        _iHaveSentMyInfo = true;
    }

    protected override void OnConnectionRemoved(INetworkInterfaceConnection obj)
    {
        base.OnConnectionRemoved(obj);

        _playerInfos.Clear();
    }
}
