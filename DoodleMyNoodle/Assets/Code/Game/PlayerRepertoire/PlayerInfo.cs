using System.Collections;
using System.Collections.Generic;

[NetMessage]
public class PlayerInfo
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
