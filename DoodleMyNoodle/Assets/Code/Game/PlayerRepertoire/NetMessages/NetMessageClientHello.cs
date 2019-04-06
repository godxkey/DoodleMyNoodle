using System;

/// <summary>
/// The client sends this message to the server upon joining the game
/// </summary>
public partial class NetMessageClientHello : NetMessage
{
    public string playerName;
}
