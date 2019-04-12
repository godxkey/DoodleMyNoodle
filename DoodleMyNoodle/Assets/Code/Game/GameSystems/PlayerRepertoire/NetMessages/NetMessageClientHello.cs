using System;

/// <summary>
/// The client sends this message to the server upon joining the game
/// </summary>
[NetMessage]
public struct NetMessageClientHello
{
    public string playerName;
}
