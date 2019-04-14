

/// <summary>
/// This is sent by the client to the server when he wants to submit a new message
/// </summary>
[NetMessage]
public struct NetMessageChatMessageSubmission
{
    public string message;
}