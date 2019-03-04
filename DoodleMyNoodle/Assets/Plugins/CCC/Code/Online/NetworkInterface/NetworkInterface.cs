using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using OperationResult = System.Action<bool, string>;

public abstract class NetworkInterface : IDisposable
{
    public abstract event Action OnDisconnectedFromSession;
    public abstract event Action OnShutdownBegin;
    public abstract event Action<INetworkInterfaceConnection> OnDisconnect;

    public NetworkState State { get; protected set; } = NetworkState.Stopped;

    public abstract bool IsServer { get; }
    public bool IsClient => !IsServer;
    public abstract INetworkInterfaceSession ConnectedSessionInfo { get; }
    public abstract ReadOnlyCollection<INetworkInterfaceConnection> Connections { get; }
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