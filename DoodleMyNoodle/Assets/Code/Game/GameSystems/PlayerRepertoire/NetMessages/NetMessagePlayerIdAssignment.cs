using System;

/// <summary>
/// The server sends this message back to a client that just joined a game to assign it an Id
/// </summary>
[NetSerializable]
public struct NetMessagePlayerIdAssignment
{
    public PlayerId playerId;
}
