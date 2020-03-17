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
        IsMaster = other.IsMaster;
        SimPlayerId = other.SimPlayerId;
    }

    public string       PlayerName;
    public PlayerId     PlayerId;
    public bool         IsMaster;
    public PersistentId SimPlayerId;
}
