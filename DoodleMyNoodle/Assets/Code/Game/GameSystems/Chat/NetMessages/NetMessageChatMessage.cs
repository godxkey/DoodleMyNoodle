

/// <summary>
/// This is sent from the server to all players when a new message arrived in the chat
/// </summary>
[NetSerializable]
public struct NetMessageChatMessage
{
    public PlayerId playerId;
    public string message;
}