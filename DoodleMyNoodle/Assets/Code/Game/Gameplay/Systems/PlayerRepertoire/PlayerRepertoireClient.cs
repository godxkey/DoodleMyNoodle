using System;
using System.Collections.Generic;
using UnityEngineX;

public class PlayerRepertoireClient : PlayerRepertoireSystem
{
    public static new PlayerRepertoireClient Instance => (PlayerRepertoireClient)GameSystem<PlayerRepertoireSystem>.Instance;

    SessionClientInterface _clientSession;

    bool _localPlayerIdAssigned = false;
    bool _playerListSyncReceived = false;

    // we defer any received change to the player repertoire until the system is ready
    Queue<object> _deferredNetMessages = new Queue<object>();

    public override bool SystemReady => _localPlayerIdAssigned && _playerListSyncReceived;

    public PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection)
    {
        if (_clientSession != null && _clientSession.ServerConnection != null && _clientSession.ServerConnection.Id == connection.Id)
        {
            return GetServerPlayerInfo();
        }
        else
        {
            return null;
        }
    }

    protected override void OnBindedToSession()
    {
        _clientSession = (SessionClientInterface)SessionInterface;
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerIdAssignment>(OnMsg_PlayerIdAssignement);
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerRepertoireSync>(OnMsg_NetMessagePlayerRepertoireSync);
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerJoined>(OnMsg_NetMessagePlayerJoined);
        _clientSession.RegisterNetMessageReceiver<NetMessagePlayerLeft>(OnMsg_NetMessagePlayerLeft);
        _clientSession.RegisterNetMessageReceiver<NetMessageSimPlayerIdAssignement>(OnMsg_NetMessageSimPlayerIdAssignement);
    }

    protected override void OnUnbindedFromSession()
    {
        _clientSession.UnregisterNetMessageReceiver<NetMessageSimPlayerIdAssignement>(OnMsg_NetMessageSimPlayerIdAssignement);
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerIdAssignment>(OnMsg_PlayerIdAssignement);
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerRepertoireSync>(OnMsg_NetMessagePlayerRepertoireSync);
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerJoined>(OnMsg_NetMessagePlayerJoined);
        _clientSession.UnregisterNetMessageReceiver<NetMessagePlayerLeft>(OnMsg_NetMessagePlayerLeft);
        _clientSession = null;
    }

    protected override void Internal_OnGameReady()
    {
        _localPlayerInfo.IsMaster = false;
        _localPlayerInfo.PlayerId = PlayerId.Invalid;

        // Say hello to server ! (this should initiate the process of being added to valid players)
        NetMessageClientHello helloMessage = new NetMessageClientHello()
        {
            playerName = _localPlayerInfo.PlayerName
        };
        _clientSession.SendNetMessageToServer(helloMessage);
        Log.Info("[PlayerRepertoireClient] Hello sent");
    }

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (SystemReady)
        {
            while (_deferredNetMessages.Count > 0)
            {
                object netMessage = _deferredNetMessages.Dequeue();
                switch (netMessage)
                {
                    case NetMessagePlayerJoined playerJoined:
                        OnMsg_NetMessagePlayerJoined(playerJoined, null);
                        break;
                    case NetMessagePlayerLeft playerLeft:
                        OnMsg_NetMessagePlayerLeft(playerLeft, null);
                        break;
                    case NetMessageSimPlayerIdAssignement simPlayerIdAssignement:
                        OnMsg_NetMessageSimPlayerIdAssignement(simPlayerIdAssignement, null);
                        break;
                }
            }
        }
    }

    void OnMsg_PlayerIdAssignement(NetMessagePlayerIdAssignment message, INetworkInterfaceConnection source)
    {
        Log.Info("[PlayerRepertoireClient] OnMsg_PlayerIdAssignement");
        _localPlayerInfo.PlayerId = message.playerId;

        _localPlayerIdAssigned = true;
    }

    void OnMsg_NetMessagePlayerRepertoireSync(NetMessagePlayerRepertoireSync message, INetworkInterfaceConnection source)
    {
        Log.Info("[PlayerRepertoireClient] OnMsg_NetMessagePlayerRepertoireSync");
        _players.Clear();
        foreach (var playerInfo in message.players)
        {
            _players.Add(new PlayerInfo(playerInfo));
        }

        _localPlayerInfo = _players.Find((x) => x.PlayerId == _localPlayerInfo.PlayerId); // a bit hackish

        _playerListSyncReceived = true;
    }

    void OnMsg_NetMessagePlayerJoined(NetMessagePlayerJoined message, INetworkInterfaceConnection source)
    {
        if (SystemReady)
        {
            Log.Info("[PlayerRepertoireClient] OnMsg_NetMessagePlayerJoined");
            _players.Add(new PlayerInfo(message.playerInfo));
        }
        else
        {
            Log.Info("[PlayerRepertoireClient] *Deferring* OnMsg_NetMessagePlayerJoined");
            _deferredNetMessages.Enqueue(message);
        }
    }

    void OnMsg_NetMessagePlayerLeft(NetMessagePlayerLeft message, INetworkInterfaceConnection source)
    {
        if (SystemReady)
        {
            Log.Info("[PlayerRepertoireClient] OnMsg_NetMessagePlayerLeft");
            _players.RemoveFirst((p) => p.PlayerId == message.playerId);
        }
        else
        {
            Log.Info("[PlayerRepertoireClient] *Deferring* OnMsg_NetMessagePlayerLeft");
            _deferredNetMessages.Enqueue(message);
        }
    }

    void OnMsg_NetMessageSimPlayerIdAssignement(NetMessageSimPlayerIdAssignement message, INetworkInterfaceConnection source)
    {
        if (SystemReady)
        {
            Log.Info("[PlayerRepertoireClient] OnMsg_NetMessageSimPlayerIdAssignement");
            PlayerInfo playerInfo = GetPlayerInfo(message.PlayerId);
            if (playerInfo == null)
            {
                Log.Error("The server told us to assign a simPlayerId to a player but that player could not be found.");
                return;
            }

            playerInfo.SimPlayerId = message.SimPlayerId;
        }
        else
        {
            Log.Info("[PlayerRepertoireClient] *Deferring* OnMsg_NetMessageSimPlayerIdAssignement");
            _deferredNetMessages.Enqueue(message);
        }
    }
}