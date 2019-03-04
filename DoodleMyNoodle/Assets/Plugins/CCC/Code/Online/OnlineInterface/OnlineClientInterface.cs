using System;
using System.Collections.Generic;

public class OnlineClientInterface : OnlineInterface
{
    public override bool IsServerType => false;

    public bool IsConnectingSession { get; private set; }

    public OnlineClientInterface(NetworkInterface network)
        : base(network)
    {
    }

    public void GetAvailableSessions(ref List<INetworkInterfaceSession> sessionList) => _network.GetSessions(ref sessionList);
    public void ConnectToSession(INetworkInterfaceSession session, Action<bool, string> onComplete = null)
    {
        IsConnectingSession = true;

        _onConnectToSessionsCallback = onComplete;
        _network.ConnectToSession(session, OnConnectToSessionComplete);
    }

    void OnConnectToSessionComplete(bool success, string message)
    {
        IsConnectingSession = false;

        if (success)
        {
            SessionInterface = new SessionClientInterface(_network);
        }

        _onConnectToSessionsCallback?.Invoke(success, message);
    }


    Action<bool, string> _onConnectToSessionsCallback;
}