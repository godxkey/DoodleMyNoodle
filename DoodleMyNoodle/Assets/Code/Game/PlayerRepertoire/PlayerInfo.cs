using System.Collections;
using System.Collections.Generic;

public partial class PlayerInfo : NetMessage
{
    public PlayerInfo() { }
    public PlayerInfo(PlayerInfo other)
    {
        playerName = other.playerName;
        playerId = other.playerId;
        isServer = other.isServer;
    }

    public string playerName;
    public PlayerId playerId;
    public bool isServer;
}
