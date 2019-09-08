using System;
using System.Collections.Generic;

public class PlayerRepertoireServer : PlayerRepertoireSystem
{
    public static new PlayerRepertoireServer Instance => (PlayerRepertoireServer)GameSystem<PlayerRepertoireSystem>.Instance;

    SessionServerInterface _serverSession;
    ushort _playerIdCounter = PlayerId.FirstValid.Value;

    List<INetworkInterfaceConnection> _newConnectionsNotYetPlayers = new List<INetworkInterfaceConnection>();

    // This list should match the _players list
    List<INetworkInterfaceConnection> _playerConnections = new List<INetworkInterfaceConnection>();
    public ReadOnlyList<INetworkInterfaceConnection> PlayerConnections => _playerConnections.AsReadOnlyNoAlloc();

    public override bool SystemReady => true;

    public override PlayerInfo GetPlayerInfo(INetworkInterfaceConnection connection)
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

    public void AssignSimPlayerToPlayer(in PlayerId playerId, in SimPlayerId simPlayerId)
    {
        PlayerInfo playerInfo = GetPlayerInfo(playerId);

        if (playerInfo == null)
        {
            DebugService.LogError("Trying to assign a SimPlayerId to a player that does not exist: " + playerId);
            return;
        }

        playerInfo.SimPlayerId = simPlayerId;

        NetMessageSimPlayerIdAssignement message = new NetMessageSimPlayerIdAssignement();
        message.SimPlayerId = simPlayerId;
        message.PlayerId = playerId;

        _serverSession.SendNetMessage(message, _playerConnections);
    }

    protected override void Internal_OnGameReady()
    {
        // when we're the server, we assign ourself our Id (which is 1)

        _localPlayerInfo.PlayerId = new PlayerId(_playerIdCounter++);
        _localPlayerInfo.IsServer = true;

        _players.Add(_localPlayerInfo);
        _playerConnections.Add(null);
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
        DebugService.Log("[PlayerRepertoireServer] OnClientHello");
        int index = _newConnectionsNotYetPlayers.IndexOf(clientConnection);
        if (index == -1)
        {
            DebugService.LogWarning("[PlayerRepertoireServer] We received a client hello, but the client is not in the _newConnectionsNotYetPlayers list. The hello will be ignored.");
            return;
        }

        _newConnectionsNotYetPlayers.RemoveAt(index);


        // Add new player to list
        PlayerInfo newPlayerInfo = new PlayerInfo()
        {
            PlayerId = new PlayerId(_playerIdCounter++),
            IsServer = false,
            PlayerName = message.playerName
        };

        ChatSystem.Instance.SubmitMessage(newPlayerInfo.PlayerName + " has joined the game.");

        // Notify other players
        NetMessagePlayerJoined playerJoinedMessage = new NetMessagePlayerJoined
        {
            playerInfo = newPlayerInfo
        };
        _serverSession.SendNetMessage(playerJoinedMessage, _playerConnections);
        DebugService.Log("[PlayerRepertoireServer] sent NetMessagePlayerJoined");

        // add to list
        _players.Add(newPlayerInfo);
        _playerConnections.Add(clientConnection);

        // Assign id to the new player
        NetMessagePlayerIdAssignment playerIdAssignementMessage = new NetMessagePlayerIdAssignment
        {
            playerId = newPlayerInfo.PlayerId
        };
        _serverSession.SendNetMessage(playerIdAssignementMessage, clientConnection);
        DebugService.Log("[PlayerRepertoireServer] sent NetMessagePlayerIdAssignment");


        // Send the complete player list to the new player
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
        if (playerIndex == -1)
        {
            // The connection was not yet a valid player. Nothing else to do
            return;
        }

        ChatSystem.Instance.SubmitMessage(_players[playerIndex].PlayerName + " has left the game.");

        PlayerId playerId = _players[playerIndex].PlayerId;

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