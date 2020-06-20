using System;
using System.Collections.Generic;
using UnityX;

public class PlayerRepertoireServer : PlayerRepertoireMaster
{
    public static new PlayerRepertoireServer Instance => (PlayerRepertoireServer)GameSystem<PlayerRepertoireSystem>.Instance;

    SessionServerInterface _serverSession;

    List<INetworkInterfaceConnection> _newConnectionsNotYetPlayers = new List<INetworkInterfaceConnection>();

    // This list should match the _players list
    List<INetworkInterfaceConnection> _playerConnections = new List<INetworkInterfaceConnection>();
    public ReadOnlyList<INetworkInterfaceConnection> PlayerConnections => _playerConnections.AsReadOnlyNoAlloc();

    public override bool SystemReady => true;

    public PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection)
    {
        for (int i = 0; i < _playerConnections.Count; i++)
        {
            if (_playerConnections[i] != null && _playerConnections[i].Id == connection.Id)
            {
                return _players[i];
            }
        }
        return null;
    }

    protected override void Internal_OnGameReady()
    {
        base.Internal_OnGameReady();

        _playerConnections.Add(null); // connection for local player
    }

    protected override void OnAssignSimPlayerToPlayer(PlayerInfo playerInfo, in PersistentId simPlayerId)
    {
        base.OnAssignSimPlayerToPlayer(playerInfo, simPlayerId);

        // notify other players
        NetMessageSimPlayerIdAssignement message = new NetMessageSimPlayerIdAssignement();
        message.SimPlayerId = simPlayerId;
        message.PlayerId = playerInfo.PlayerId;

        _serverSession.SendNetMessage(message, _playerConnections);
    }

    protected override void OnBindedToSession()
    {
        _serverSession = (SessionServerInterface)SessionInterface;
        SessionInterface.OnConnectionAdded += OnConnectionAdded;
        SessionInterface.OnConnectionRemoved += OnConnectionRemoved;
        SessionInterface.RegisterNetMessageReceiver<NetMessageClientHello>(OnClientHello);
    }
    protected override void OnUnbindedFromSession()
    {
        SessionInterface.OnConnectionAdded -= OnConnectionAdded;
        SessionInterface.OnConnectionRemoved -= OnConnectionRemoved;
        SessionInterface.UnregisterNetMessageReceiver<NetMessageClientHello>(OnClientHello);
        _serverSession = null;
    }

    void OnConnectionAdded(INetworkInterfaceConnection newConnection)
    {
        _newConnectionsNotYetPlayers.Add(newConnection);
    }

    void OnClientHello(NetMessageClientHello message, INetworkInterfaceConnection clientConnection)
    {
        Log.Info("[PlayerRepertoireServer] OnClientHello");
        int index = _newConnectionsNotYetPlayers.IndexOf(clientConnection);
        if (index == -1)
        {
            Log.Warning("[PlayerRepertoireServer] We received a client hello, but the client is not in the _newConnectionsNotYetPlayers list. The hello will be ignored.");
            return;
        }

        _newConnectionsNotYetPlayers.RemoveAt(index);


        // Add new player to list
        PlayerInfo newPlayerInfo = CreateNewPlayer(message.playerName, false);

        // Notify other players
        NetMessagePlayerJoined playerJoinedMessage = new NetMessagePlayerJoined
        {
            playerInfo = newPlayerInfo
        };
        _serverSession.SendNetMessage(playerJoinedMessage, _playerConnections);
        Log.Info("[PlayerRepertoireServer] sent NetMessagePlayerJoined");

        // add new connection
        _playerConnections.Add(clientConnection);

        // Assign id to the new player
        NetMessagePlayerIdAssignment playerIdAssignementMessage = new NetMessagePlayerIdAssignment
        {
            playerId = newPlayerInfo.PlayerId
        };
        _serverSession.SendNetMessage(playerIdAssignementMessage, clientConnection);
        Log.Info("[PlayerRepertoireServer] sent NetMessagePlayerIdAssignment");


        // Send the complete player list to the new player
        NetMessagePlayerRepertoireSync syncMessage = new NetMessagePlayerRepertoireSync()
        {
            players = _players.ToArray()
        };
        _serverSession.SendNetMessage(syncMessage, clientConnection);
        Log.Info("[PlayerRepertoireServer] sent NetMessagePlayerRepertoireSync");
    }

    void OnConnectionRemoved(INetworkInterfaceConnection oldConnection)
    {
        Log.Info("[PlayerRepertoireServer] OnConnectionRemoved: " + oldConnection.Id);
        _newConnectionsNotYetPlayers.Remove(oldConnection);

        int playerIndex = _playerConnections.IndexOf(oldConnection);
        if (playerIndex == -1)
        {
            // The connection was not yet a valid player. Nothing else to do
            return;
        }

        PlayerId playerId = _players[playerIndex].PlayerId;

        // destroy
        DestroyPlayer(playerId);

        // remove connection
        _playerConnections.RemoveAt(playerIndex);


        // Notify other players
        NetMessagePlayerLeft playerLeftMessage = new NetMessagePlayerLeft()
        {
            playerId = playerId
        };
        _serverSession.SendNetMessage(playerLeftMessage, _playerConnections);
        Log.Info("[PlayerRepertoireServer] sent NetMessagePlayerLeft");
    }
}