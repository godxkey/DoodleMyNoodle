using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnlineInterface : IDisposable
{
    public event Action OnTerminate;
    public SessionInterface SessionInterface { get; protected set; }

    public abstract bool IsServerType { get; }
    public bool IsClientType => !IsServerType;

    public OnlineInterface(NetworkInterface network)
    {
        _network = network;

        _network.OnDisconnectedFromSession += OnDisconnectFromSession;

        Debug.Log("Online interface created");
    }

    public void Update()
    {
        SessionInterface?.Update();
    }

    protected virtual void OnDisconnectFromSession()
    {
        SessionInterface?.Dispose();
        SessionInterface = null;
    }

    public virtual void Dispose()
    {
        SessionInterface?.Dispose();
        _network.OnDisconnectedFromSession -= OnDisconnectFromSession;

        Debug.Log("Online interface terminating");

        OnTerminate?.Invoke();
    }

    protected NetworkInterface _network;
}