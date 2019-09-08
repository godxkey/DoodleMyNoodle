using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using OperationResultCallback = System.Action<bool, string>;

public abstract class NetworkInterface : IDisposable
{
    public abstract event Action OnDisconnectedFromSession;
    public abstract event Action OnShutdownBegin;
    public abstract event Action<INetworkInterfaceConnection> OnDisconnect;
    public abstract event Action<INetworkInterfaceConnection> OnConnect;
    public abstract event Action OnSessionListUpdated;

    public NetworkState State { get; protected set; } = NetworkState.Stopped;

    public abstract bool IsServer { get; }
    public bool IsClient => !IsServer;
    public abstract INetworkInterfaceSession ConnectedSessionInfo { get; }
    public abstract ReadOnlyList<INetworkInterfaceConnection> Connections { get; }
    public abstract ReadOnlyList<INetworkInterfaceSession> Sessions { get; }
    public abstract void GetSessions(ref List<INetworkInterfaceSession> list);

    public abstract void LaunchClient(OperationResultCallback onComplete = null);
    public abstract void LaunchServer(OperationResultCallback onComplete = null);
    public abstract void Shutdown(OperationResultCallback onComplete = null);
    public abstract void CreateSession(string sessionName, OperationResultCallback onComplete = null);
    public abstract void ConnectToSession(INetworkInterfaceSession session, OperationResultCallback onComplete = null);
    public abstract void DisconnectFromSession(OperationResultCallback onComplete = null);

    public abstract void Update();

    public abstract void SendMessage(INetworkInterfaceConnection connection, byte[] data, int size);
    public abstract void SetMessageReader(Action<INetworkInterfaceConnection, byte[], int> messageReader);

    public virtual void Dispose() { }
}