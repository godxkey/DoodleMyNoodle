using System.Collections;
using System.Collections.Generic;

[NetSerializable]
[System.Serializable]
public class PlayerInfo
{
    public PlayerInfo() { }
    public PlayerInfo(PlayerInfo other)
    {
        PlayerName = other.PlayerName;
        PlayerId = other.PlayerId;
        IsServer = other.IsServer;
        SimPlayerId = other.SimPlayerId;
    }

    public string       PlayerName;
    public PlayerId     PlayerId;
    public bool         IsServer;
    public SimPlayerId  SimPlayerId;
}
