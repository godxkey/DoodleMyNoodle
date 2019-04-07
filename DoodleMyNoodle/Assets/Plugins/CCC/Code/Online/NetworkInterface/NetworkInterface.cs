using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using OperationResult = System.Action<bool, string>;

public abstract class NetworkInterface : IDisposable
{
    public abstract event Action onDisconnectedFromSession;
    public abstract event Action onShutdownBegin;
    public abstract event Action<INetworkInterfaceConnection> onDisconnect;
    public abstract event Action<INetworkInterfaceConnection> onConnect;
    public abstract event Action onSessionListUpdated;

    public NetworkState state { get; protected set; } = NetworkState.Stopped;

    public abstract bool isServer { get; }
    public bool isClient => !isServer;
    public abstract INetworkInterfaceSession connectedSessionInfo { get; }
    public abstract ReadOnlyCollection<INetworkInterfaceConnection> connections { get; }
    public abstract ReadOnlyCollection<INetworkInterfaceSession> sessions { get; }
    public abstract void GetSessions(ref List<INetworkInterfaceSession> list);

    public abstract void LaunchClient(OperationResult onComplete = null);
    public abstract void LaunchServer(OperationResult onComplete = null);
    public abstract void Shutdown(OperationResult onComplete = null);
    public abstract void CreateSession(string sessionName, OperationResult onComplete = null);
    public abstract void ConnectToSession(INetworkInterfaceSession session, OperationResult onComplete = null);
    public abstract void DisconnectFromSession(OperationResult onComplete = null);

    public abstract void Update();

    public abstract void SendMessage(INetworkInterfaceConnection connection, byte[] data, int size);
    public abstract void SetMessageReader(Action<INetworkInterfaceConnection, byte[], int> messageReader);

    public virtual void Dispose() { }
}