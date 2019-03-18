using System;
using System.Collections.Generic;

public class OnlineServerInterface : OnlineInterface
{
    public override bool isServerType => true;

    public bool isCreatingSession { get; private set; }

    public OnlineServerInterface(NetworkInterface network)
        : base(network)
    {
    }

    public void CreateSession(string sessionName, Action<bool, string> onComplete = null)
    {
        isCreatingSession = true;

        _onSessionCreatedCallback = onComplete;
        _network.CreateSession(sessionName, OnSessionCreationComplete);
    }

    void OnSessionCreationComplete(bool success, string message)
    {
        isCreatingSession = false;

        if (success)
        {
            sessionInterface = new SessionServerInterface(_network);
        }

        _onSessionCreatedCallback?.Invoke(success, message);
    }

    Action<bool, string> _onSessionCreatedCallback;
}