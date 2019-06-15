

/// <summary>
/// This is sent by the client to the server when he wants to submit a new message
/// </summary>
[NetSerializable]
public struct NetMessageChatMessageSubmission
{
    public string message;
}