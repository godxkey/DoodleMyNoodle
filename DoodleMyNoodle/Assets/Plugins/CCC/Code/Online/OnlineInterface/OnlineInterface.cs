using System;
using System.Collections.Generic;
using UnityEngine;
using UnityX;

public abstract class OnlineInterface : IDisposable
{
    const bool LOG = false;

    public event Action OnTerminate;
    public SessionInterface SessionInterface { get; protected set; }

    public abstract bool IsServerType { get; }
    public bool IsClientType => !IsServerType;

    public OnlineInterface(NetworkInterface network)
    {
        _network = network;

        _network.OnDisconnectedFromSession += OnDisconnectFromSession;

        if (LOG)
#pragma warning disable CS0162 // Unreachable code detected
            Log.Info("Online interface created");
#pragma warning restore CS0162 // Unreachable code detected
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

        if (LOG)
#pragma warning disable CS0162 // Unreachable code detected
            Log.Info("Online interface terminating");
#pragma warning restore CS0162 // Unreachable code detected

        OnTerminate?.Invoke();
    }

    protected NetworkInterface _network;
}