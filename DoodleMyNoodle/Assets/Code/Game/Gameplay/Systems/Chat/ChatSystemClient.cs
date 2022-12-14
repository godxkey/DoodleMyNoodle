using System;

public class ChatSystemClient : ChatSystem
{
    SessionClientInterface _session;

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _session = OnlineService.ClientInterface.SessionClientInterface;
        _session.RegisterNetMessageReceiver<NetMessageChatMessage>(OnNetMessageChatMessage);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _session?.UnregisterNetMessageReceiver<NetMessageChatMessage>(OnNetMessageChatMessage);
        _session = null;
    }



    public override void SubmitMessage(string message)
    {
        // Build the net message
        NetMessageChatMessageSubmission submission = new NetMessageChatMessageSubmission()
        {
            message = message
        };

        // send to server
        _session.SendNetMessageToServer(submission);
    }

    void OnNetMessageChatMessage(NetMessageChatMessage chatMessage, INetworkInterfaceConnection source)
    {
        // Server tells us a new message should be displayed in the chat box!
        PlayerInfo sourcePlayer = PlayerRepertoireSystem.Instance.GetPlayerInfo(chatMessage.playerId);

        string playerName = sourcePlayer == null ? "ERROR UNKNOWN PLAYER" : sourcePlayer.PlayerName;

        AddNewLine(new ChatLine()
        {
            Message = chatMessage.message,
            ChatterName = playerName
        });
    }
}
