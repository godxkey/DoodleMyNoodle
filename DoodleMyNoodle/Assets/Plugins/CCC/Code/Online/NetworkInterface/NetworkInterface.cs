using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngineX;

public delegate void OperationResultDelegate(bool success, string reason);

public interface IStreamChannel
{
    string Name { get; }
    bool IsReliable { get; }
    int Priority { get; }
    StreamChannelType Type { get; }
}

public enum StreamChannelType
{
    LargeDataTransfer
}

public abstract class NetworkInterface : IDisposable
{
    protected void GetStreamChannelSettings(StreamChannelType streamChannelType, out bool reliable, out int priority)
    {
        switch (streamChannelType)
        {
            case StreamChannelType.LargeDataTransfer:
                reliable = true;
                priority = 1;
                break;

            default:
                throw new Exception($"Missing entry for stream channel {streamChannelType}");
        }
    }

    public abstract event Action OnDisconnectedFromSession;
    public abstract event Action OnShutdownBegin;
    public abstract event Action<INetworkInterfaceConnection> OnDisconnect;
    public abstract event Action<INetworkInterfaceConnection> OnConnect;
    public abstract event Action OnSessionListUpdated;
    public abstract event Action<byte[], IStreamChannel, INetworkInterfaceConnection> StreamDataReceived;

    public delegate void StreamDataStartedDelegate(INetworkInterfaceConnection connection, IStreamChannel channel, ulong streamID);
    public abstract event StreamDataStartedDelegate StreamDataStarted;
    
    public delegate void StreamDataProgressDelegate(INetworkInterfaceConnection connection, IStreamChannel channel, ulong streamID, float progress);
    public abstract event StreamDataProgressDelegate StreamDataProgress;
    
    public delegate void StreamDataAbortedDelegate(INetworkInterfaceConnection connection, IStreamChannel channel, ulong streamID);
    public abstract event StreamDataAbortedDelegate StreamDataAborted;

    public NetworkState State { get; protected set; } = NetworkState.Stopped;

    public abstract bool IsServer { get; }
    public bool IsClient => !IsServer;
    public abstract INetworkInterfaceSession ConnectedSessionInfo { get; }
    public abstract ReadOnlyList<INetworkInterfaceConnection> Connections { get; }
    public abstract ReadOnlyList<INetworkInterfaceSession> Sessions { get; }
    public abstract void GetSessions(ref List<INetworkInterfaceSession> list);

    public abstract void LaunchClient(OperationResultDelegate onComplete = null);
    public abstract void LaunchServer(OperationResultDelegate onComplete = null);
    public abstract void Shutdown(OperationResultDelegate onComplete = null);
    public abstract void CreateSession(string sessionName, OperationResultDelegate onComplete = null);
    public abstract void ConnectToSession(INetworkInterfaceSession session, OperationResultDelegate onComplete = null);
    public abstract void DisconnectFromSession(OperationResultDelegate onComplete = null);

    public abstract void Update();

    public abstract void SendMessage(INetworkInterfaceConnection connection, byte[] data, bool reliableAndOrdered);
    public abstract void SetMessageReader(Action<INetworkInterfaceConnection, byte[]> messageReader);

    public abstract IStreamChannel GetStreamChannel(StreamChannelType channel);

    public virtual void Dispose() { }
}

public class NetworkInterfaceNone : NetworkInterface
{
    public override bool IsServer => false;
    public override INetworkInterfaceSession ConnectedSessionInfo => throw new NotImplementedException();
    public override ReadOnlyList<INetworkInterfaceConnection> Connections => throw new NotImplementedException();
    public override ReadOnlyList<INetworkInterfaceSession> Sessions => throw new NotImplementedException();

#pragma warning disable CS0067 // Naming Styles
    public override event Action OnDisconnectedFromSession;
    public override event Action OnShutdownBegin;
    public override event Action<INetworkInterfaceConnection> OnDisconnect;
    public override event Action<INetworkInterfaceConnection> OnConnect;
    public override event Action OnSessionListUpdated;
    public override event Action<byte[], IStreamChannel, INetworkInterfaceConnection> StreamDataReceived;
    public override event StreamDataStartedDelegate StreamDataStarted;
    public override event StreamDataProgressDelegate StreamDataProgress;
    public override event StreamDataAbortedDelegate StreamDataAborted;
#pragma warning restore CS0067 // Naming Styles

    public override void ConnectToSession(INetworkInterfaceSession session, OperationResultDelegate onComplete = null)
    {
        throw new NotImplementedException();
    }

    public override void CreateSession(string sessionName, OperationResultDelegate onComplete = null)
    {
        throw new NotImplementedException();
    }

    public override void DisconnectFromSession(OperationResultDelegate onComplete = null)
    {
        throw new NotImplementedException();
    }

    public override void GetSessions(ref List<INetworkInterfaceSession> list)
    {
    }

    public override IStreamChannel GetStreamChannel(StreamChannelType channel)
    {
        return null;
    }

    public override void LaunchClient(OperationResultDelegate onComplete = null)
    {
        onComplete(false, "Not implemented");
    }

    public override void LaunchServer(OperationResultDelegate onComplete = null)
    {
        onComplete(false, "Not implemented");
    }

    public override void SendMessage(INetworkInterfaceConnection connection, byte[] data, bool reliableAndOrdered)
    {
        throw new NotImplementedException();
    }

    public override void SetMessageReader(Action<INetworkInterfaceConnection, byte[]> messageReader)
    {
        throw new NotImplementedException();
    }

    public override void Shutdown(OperationResultDelegate onComplete = null)
    {
        onComplete(true, "");
    }

    public override void Update()
    {
    }
}
