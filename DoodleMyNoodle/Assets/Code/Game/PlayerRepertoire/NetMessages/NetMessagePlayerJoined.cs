using System;

/// <summary>
/// The server sends this message to all clients when a player joins the game
/// </summary>
public partial class NetMessagePlayerJoined : NetMessage
{
    public PlayerInfo playerInfo;
}
