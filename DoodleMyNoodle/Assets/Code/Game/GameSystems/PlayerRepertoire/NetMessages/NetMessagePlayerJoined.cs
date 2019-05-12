using System;

/// <summary>
/// The server sends this message to all clients when a player joins the game
/// </summary>

[NetSerializable]
public struct NetMessagePlayerJoined
{
    public PlayerInfo playerInfo;
}
