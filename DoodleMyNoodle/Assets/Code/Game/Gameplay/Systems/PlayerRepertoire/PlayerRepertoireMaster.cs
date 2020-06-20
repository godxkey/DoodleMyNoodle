using System;
using System.Collections.Generic;
using UnityX;

public abstract class PlayerRepertoireMaster : PlayerRepertoireSystem
{
    public static new PlayerRepertoireMaster Instance => (PlayerRepertoireMaster)GameSystem<PlayerRepertoireSystem>.Instance;

    ushort _playerIdCounter = PlayerId.FirstValid.Value;

    public override bool SystemReady => true;

    public void AssignSimPlayerToPlayer(in PlayerId playerId, in PersistentId simPlayerId)
    {
        PlayerInfo playerInfo = GetPlayerInfo(playerId);

        if (playerInfo == null)
        {
            Log.Error("Trying to assign a SimPlayerId to a player that does not exist: " + playerId);
            return;
        }

        playerInfo.SimPlayerId = simPlayerId;

        OnAssignSimPlayerToPlayer(playerInfo, simPlayerId);
    }

    protected virtual void OnAssignSimPlayerToPlayer(PlayerInfo playerInfo, in PersistentId simPlayerId) { }

    protected override void Internal_OnGameReady()
    {
        // when we're the master, we assign ourself our Id (which is 1)

        _localPlayerInfo.PlayerId = new PlayerId(_playerIdCounter++);
        _localPlayerInfo.IsMaster = true;

        _players.Add(_localPlayerInfo);
    }

    protected PlayerInfo CreateNewPlayer(string playerName, bool isMaster)
    {
        // Add new player to list
        PlayerInfo newPlayerInfo = new PlayerInfo()
        {
            PlayerId = new PlayerId(_playerIdCounter++),
            IsMaster = isMaster,
            PlayerName = playerName
        };

        ChatSystem.Instance.SubmitMessage(newPlayerInfo.PlayerName + " has joined the game.");

        _players.Add(newPlayerInfo);

        OnCreatedNewPlayer(newPlayerInfo);

        return newPlayerInfo;
    }

    protected virtual void OnCreatedNewPlayer(PlayerInfo playerInfo) { }

    protected void DestroyPlayer(PlayerId playerId)
    {
        int index = GetIndexOfPlayer(playerId);

        if (index == -1)
            return;

        ChatSystem.Instance.SubmitMessage(_players[index].PlayerName + " has left the game.");

        OnDestroyingNewPlayer(_players[index]);

        // remove player from lists
        _players.RemoveAt(index);
    }

    protected virtual void OnDestroyingNewPlayer(PlayerInfo playerInfo) { }
}