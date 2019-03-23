using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnlineInterface : IDisposable
{
    public event Action onTerminate;
    public SessionInterface sessionInterface { get; protected set; }

    public abstract bool isServerType { get; }
    public bool isClientType => !isServerType;

    public OnlineInterface(NetworkInterface network)
    {
        _network = network;

        _network.onDisconnectedFromSession += OnDisconnectFromSession;

        DebugService.Log("Online interface created");
    }

    public void Update()
    {
        sessionInterface?.Update();
    }

    protected virtual void OnDisconnectFromSession()
    {
        sessionInterface?.Dispose();
        sessionInterface = null;
    }

    public virtual void Dispose()
    {
        sessionInterface?.Dispose();
        _network.onDisconnectedFromSession -= OnDisconnectFromSession;

        DebugService.Log("Online interface terminating");

        onTerminate?.Invoke();
    }

    protected NetworkInterface _network;
}