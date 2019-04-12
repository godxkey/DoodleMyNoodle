using System;

/// <summary>
/// The server sends this message to all clients when a player left the game
/// </summary>
[NetMessage]
public struct NetMessagePlayerLeft
{
    public PlayerId playerId;
}
