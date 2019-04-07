using System;

/// <summary>
/// The server sends this message to all clients when a player left the game
/// </summary>
public partial class NetMessagePlayerLeft : NetMessage
{
    public PlayerId playerId;
}
