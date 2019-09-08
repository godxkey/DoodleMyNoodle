using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class OnlineClientInterface : OnlineInterface
{
    public override bool IsServerType => false;
    public SessionClientInterface SessionClientInterface => SessionInterface == null ? null : (SessionClientInterface)SessionInterface;

    public event Action OnSessionListUpdated;
    public bool IsConnectingSession { get; private set; }

    public OnlineClientInterface(NetworkInterface network)
        : base(network)
    {
        network.OnSessionListUpdated += OnSessionListUpdatedCallback;
    }

    public ReadOnlyList<INetworkInterfaceSession> AvailableSessions => _network.Sessions;
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

    void OnSessionListUpdatedCallback()
    {
        OnSessionListUpdated?.Invoke();
    }

    public override void Dispose()
    {
        if (_network != null)
            _network.OnSessionListUpdated -= OnSessionListUpdated;

        base.Dispose();
    }


    Action<bool, string> _onConnectToSessionsCallback;
}