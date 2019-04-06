using System;
using System.Collections.Generic;

public class PlayerRepertoireServer : PlayerRepertoire
{
    SessionServerInterface _serverSession;
    ushort _playerIdCounter;

    List<INetworkInterfaceConnection> _newConnectionsNotYetPlayers = new List<INetworkInterfaceConnection>();

    // This list should match the _players list
    List<INetworkInterfaceConnection> _playerConnections = new List<INetworkInterfaceConnection>();

    protected override void OnPreReady()
    {
        // when we're the server, we assign ourself our Id (which is 0)

        _localPlayerInfo.playerId = new PlayerId(_playerIdCounter++);
        _localPlayerInfo.isServer = true;
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
        int index = _newConnectionsNotYetPlayers.IndexOf(clientConnection);
        if(index == -1)
        {
            DebugService.LogWarning("[PlayerRepertoireServer] We received a client hello, but the client is not in the _newConnectionsNotYetPlayers list. The hello will be ignored.");
            return;
        }

        _newConnectionsNotYetPlayers.RemoveAt(index);

        
        // Add new player to list
        PlayerInfo newPlayerInfo = new PlayerInfo
        {
            playerId = new PlayerId(_playerIdCounter++),
            isServer = false,
            playerName = message.playerName
        };

        _players.Add(newPlayerInfo);
        _playerConnections.Add(clientConnection);


        // Notify other players
        NetMessagePlayerJoined playerJoinedMessage = new NetMessagePlayerJoined
        {
            playerInfo = newPlayerInfo
        };
        _serverSession.SendNetMessage(playerJoinedMessage, _playerConnections);


        // Sync all the players to the new one
        NetMessagePlayerRepertoireSync syncMessage = new NetMessagePlayerRepertoireSync()
        {
            players = _players
        };
        _serverSession.SendNetMessage(syncMessage, clientConnection);
    }

    void OnConnectionRemoved(INetworkInterfaceConnection oldConnection)
    {
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
    }
}