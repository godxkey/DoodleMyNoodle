using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class PlayerRepertoireServer : PlayerRepertoireSystem
{
    public static new PlayerRepertoireServer instance => (PlayerRepertoireServer)GameSystem<PlayerRepertoireSystem>.instance;

    SessionServerInterface _serverSession;
    ushort _playerIdCounter;

    List<INetworkInterfaceConnection> _newConnectionsNotYetPlayers = new List<INetworkInterfaceConnection>();

    // This list should match the _players list
    List<INetworkInterfaceConnection> _playerConnections = new List<INetworkInterfaceConnection>();
    public ReadOnlyCollection<INetworkInterfaceConnection> playerConnections;

    public override bool isSystemReady => true;

    public override PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection)
    {
        for (int i = 0; i < _playerConnections.Count; i++)
        {
            if(_playerConnections[i] != null && _playerConnections[i].Id == connection.Id)
            {
                return _players[i];
            }
        }
        return null;
    }

    protected override void Internal_OnGameReady()
    {
        playerConnections = _playerConnections.AsReadOnly();

        // when we're the server, we assign ourself our Id (which is 0)

        _localPlayerInfo.playerId = new PlayerId(_playerIdCounter++);
        _localPlayerInfo.isServer = true;

        _players.Add(_localPlayerInfo);
        _playerConnections.Add(null);
    }


    protected override void OnBindedToSession()
    {
        _serverSession = (SessionServerInterface)_sessionInterface;
        _sessionInterface.onConnectionAdded += OnConnectionAdded;
        _sessionInterface.onConnectionRemoved += OnConnectionRemoved;
        _sessionInterface.RegisterNetMessageReceiver<NetMessageClientHello>(OnClientHello);
    }
    protected override void OnUnbindedFromSession()
    {
        _sessionInterface.onConnectionAdded -= OnConnectionAdded;
        _sessionInterface.onConnectionRemoved -= OnConnectionRemoved;
        _sessionInterface.UnregisterNetMessageReceiver<NetMessageClientHello>(OnClientHello);
        _serverSession = null;
    }

    void OnConnectionAdded(INetworkInterfaceConnection newConnection)
    {
        _newConnectionsNotYetPlayers.Add(newConnection);
    }

    void OnClientHello(NetMessageClientHello message, INetworkInterfaceConnection clientConnection)
    {
        DebugService.Log("[PlayerRepertoireServer] OnClientHello");
        int index = _newConnectionsNotYetPlayers.IndexOf(clientConnection);
        if(index == -1)
        {
            DebugService.LogWarning("[PlayerRepertoireServer] We received a client hello, but the client is not in the _newConnectionsNotYetPlayers list. The hello will be ignored.");
            return;
        }

        _newConnectionsNotYetPlayers.RemoveAt(index);

        
        // Add new player to list
        PlayerInfo newPlayerInfo = new PlayerInfo()
        {
            playerId = new PlayerId(_playerIdCounter++),
            isServer = false,
            playerName = message.playerName
        };

        _players.Add(newPlayerInfo);
        _playerConnections.Add(clientConnection);

        // Assign id to the new player
        NetMessagePlayerIdAssignment playerIdAssignementMessage = new NetMessagePlayerIdAssignment
        {
            playerId = newPlayerInfo.playerId
        };
        _serverSession.SendNetMessage(playerIdAssignementMessage, clientConnection);
        DebugService.Log("[PlayerRepertoireServer] sent NetMessagePlayerIdAssignment");


        // Notify other players
        NetMessagePlayerJoined playerJoinedMessage = new NetMessagePlayerJoined
        {
            playerInfo = newPlayerInfo
        };
        _serverSession.SendNetMessage(playerJoinedMessage, _playerConnections);
        DebugService.Log("[PlayerRepertoireServer] sent NetMessagePlayerJoined");


        // Sync all the players to the new one
        NetMessagePlayerRepertoireSync syncMessage = new NetMessagePlayerRepertoireSync()
        {
            players = _players.ToArray()
        };
        _serverSession.SendNetMessage(syncMessage, clientConnection);
        DebugService.Log("[PlayerRepertoireServer] sent NetMessagePlayerRepertoireSync");
    }

    void OnConnectionRemoved(INetworkInterfaceConnection oldConnection)
    {
        DebugService.Log("[PlayerRepertoireServer] OnConnectionRemoved: " + oldConnection.Id);
        _newConnectionsNotYetPlayers.Remove(oldConnection);

        int playerIndex = _playerConnections.IndexOf(oldConnection);
        if(playerIndex == -1)
        {
            // The connection was not yet a valid player. Nothing else to do
            return;
        }

        PlayerId playerId = _players[playerIndex].playerId;

        // remove player from lists
        _playerConnections.RemoveAt(playerIndex);
        _players.RemoveAt(playerIndex);


        // Notify other players
        NetMessagePlayerLeft playerLeftMessage = new NetMessagePlayerLeft()
        {
            playerId = playerId
        };
        _serverSession.SendNetMessage(playerLeftMessage, _playerConnections);
        DebugService.Log("[PlayerRepertoireServer] sent NetMessagePlayerLeft");
    }
}