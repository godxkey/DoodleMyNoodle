using System;
using System.Collections.Generic;

/// <summary>
/// The server sends this message back to a client that just joined a game to sync him/her up to date
/// </summary>
[NetSerializable]
public struct NetMessagePlayerRepertoireSync
{
    public PlayerInfo[] players;
}
