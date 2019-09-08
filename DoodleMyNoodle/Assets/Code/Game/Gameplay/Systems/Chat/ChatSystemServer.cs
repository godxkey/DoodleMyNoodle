using System;

public class ChatSystemServer : ChatSystem
{
    SessionServerInterface _session;

    public override void OnGameReady()
    {
        base.OnGameReady();

        _session = OnlineService.serverInterface.sessionServerInterface;
        _session.RegisterNetMessageReceiver<NetMessageChatMessageSubmission>(OnNetMessageChatMessageSubmission);
    }

    public override void OnSafeDestroy()
    {
        base.OnSafeDestroy();

        _session?.UnregisterNetMessageReceiver<NetMessageChatMessageSubmission>(OnNetMessageChatMessageSubmission);
        _session = null;
    }



    public override void SubmitMessage(string message)
    {
        PlayerInfo localPlayer = PlayerRepertoireSystem.Instance.GetLocalPlayerInfo();
        AddNewMessage(message, localPlayer);
    }

    void OnNetMessageChatMessageSubmission(NetMessageChatMessageSubmission submission, INetworkInterfaceConnection source)
    {
        // A client wants to submit a new message
        PlayerInfo sourcePlayer = PlayerRepertoireServer.Instance.GetPlayerInfo(source);

        if (sourcePlayer != null)
            AddNewMessage(submission.message, sourcePlayer);
    }

    void AddNewMessage(string message, PlayerInfo playerInfo)
    {
        AddNewLine(new ChatLine()
        {
            ChatterName = playerInfo.PlayerName,
            Message = message
        });


        // Notify clients!
        NetMessageChatMessage netMessage = new NetMessageChatMessage()
        {
            message = message,
            playerId = playerInfo.PlayerId
        };

        _session.SendNetMessage(netMessage, PlayerRepertoireServer.Instance.PlayerConnections);
    }
}
