using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class OnlineClientInterface : OnlineInterface
{
    public override bool isServerType => false;

    public event Action onSessionListUpdated;
    public bool isConnectingSession { get; private set; }

    public OnlineClientInterface(NetworkInterface network)
        : base(network)
    {
        network.onSessionListUpdated += OnSessionListUpdated;
    }

    public ReadOnlyCollection<INetworkInterfaceSession> availableSessions => _network.sessions;
    public void GetAvailableSessions(ref List<INetworkInterfaceSession> sessionList) => _network.GetSessions(ref sessionList);
    public void ConnectToSession(INetworkInterfaceSession session, Action<bool, string> onComplete = null)
    {
        isConnectingSession = true;

        _onConnectToSessionsCallback = onComplete;
        _network.ConnectToSession(session, OnConnectToSessionComplete);
    }

    void OnConnectToSessionComplete(bool success, string message)
    {
        isConnectingSession = false;

        if (success)
        {
            sessionInterface = new SessionClientInterface(_network);
        }

        _onConnectToSessionsCallback?.Invoke(success, message);
    }

    void OnSessionListUpdated()
    {
        onSessionListUpdated?.Invoke();
    }

    public override void Dispose()
    {
        if (_network != null)
            _network.onSessionListUpdated -= OnSessionListUpdated;

        base.Dispose();
    }


    Action<bool, string> _onConnectToSessionsCallback;
}