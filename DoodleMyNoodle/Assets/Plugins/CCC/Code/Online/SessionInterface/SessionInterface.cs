using System;
using System.Collections.Generic;
using CCC.Online.DataTransfer;
using CCC.Operations;
using UnityEngineX;

public abstract class SessionInterface : IDisposable
{
    static LogChannel s_netMessageLogChannel = Log.CreateChannel("NetMessages", activeByDefault: false);

    [ConsoleVar("Online.TransferWithStream", 
        "Should we use a UDP Stream Channel for large data transfers? If false, we will fallback to manual paquet handling.",
        Save = ConsoleVarAttribute.SaveMode.PlayerPrefs)]
    static bool s_transferWithStream = true;

    public abstract bool IsServerType { get; }
    public bool IsClientType => !IsServerType;
    public string HostName => _networkInterface.ConnectedSessionInfo.HostName;
    public INetworkInterfaceSession SessionInfo => _networkInterface.ConnectedSessionInfo;
    public ReadOnlyList<INetworkInterfaceConnection> Connections => _networkInterface.Connections;
    public ReadOnlyList<CoroutineOperation> IncomingDataTransfers => _incomingDataTransfers.AsReadOnlyNoAlloc();
    public ReadOnlyList<CoroutineOperation> OutgoingDataTransfer => _outgoingDataTransfers.AsReadOnlyNoAlloc();
    public event Action OnTerminate;
    public event Action<INetworkInterfaceConnection> OnConnectionAdded;
    public event Action<INetworkInterfaceConnection> OnConnectionRemoved;
    public event Action<CoroutineOperation, INetworkInterfaceConnection> OnBeginReceiveLargeDataTransfer;
    internal NetworkInterface NetworkInterface => _networkInterface;


    public SessionInterface(NetworkInterface networkInterface)
    {
        _networkInterface = networkInterface;
        _networkInterface.SetMessageReader(OnReceiveMessage);

        _networkInterface.OnDisconnect += InterfaceOnDisconnect;
        _networkInterface.OnConnect += Interface_OnConnect;

        RegisterNetMessageReceiver<NetMessageViaManualPacketsHeader>(OnReceiveDataTransferHeader);
        RegisterNetMessageReceiver<NetMessageViaStreamChannelHeader>(OnReceiveDataTransferHeader);
    }

    public virtual void Dispose()
    {
        Disposed = true;

        OnTerminate?.Invoke();

        _networkInterface.OnDisconnect -= InterfaceOnDisconnect;
        _networkInterface.OnConnect -= Interface_OnConnect;

        _outgoingDataTransfers.ForEach((x) => { if (x.IsRunning) x.TerminateWithAbnormalFailure(); });
        _incomingDataTransfers.ForEach((x) => { if (x.IsRunning) x.TerminateWithAbnormalFailure(); });
        _outgoingDataTransfers.Clear();
        _incomingDataTransfers.Clear();
    }

    public virtual void Update()
    {
        _outgoingDataTransfers.RemoveAll((x) => !x.IsRunning);
        _incomingDataTransfers.RemoveAll((x) => !x.IsRunning);
    }

    public void RegisterNetMessageReceiver<NetMessageType>(Action<NetMessageType, INetworkInterfaceConnection> callback)
    {
        if (Disposed)
            return;

        Type t = typeof(NetMessageType);

        if (_netMessageReceivers.ContainsKey(t) == false)
        {
            _netMessageReceivers.Add(t, new NetMessageReceiverList<NetMessageType>());
        }

        _netMessageReceivers[t].AddListener(callback);
    }

    public void UnregisterNetMessageReceiver<NetMessageType>(Action<NetMessageType, INetworkInterfaceConnection> callback)
    {
        if (Disposed)
            return;

        Type t = typeof(NetMessageType);

        if (_netMessageReceivers.ContainsKey(t))
        {
            _netMessageReceivers[t].RemoveListener(callback);
        }
    }

    public void Disconnect()
    {
        if (Disposed)
            return;

        _networkInterface.DisconnectFromSession(null);
    }

    public void SendNetMessage<T>(in T netMessage, INetworkInterfaceConnection connection, bool reliableAndOrdered = true)
    {
        if (Disposed)
            return;

        if (NetMessageInterpreter.GetDataFromMessage(netMessage, out byte[] messageData, byteLimit: OnlineConstants.MAX_MESSAGE_SIZE))
        {
            Log.Info(s_netMessageLogChannel, $"[Session] Send message '{netMessage}' to connection {connection.Id}");

            _networkInterface.SendMessage(connection, messageData, reliableAndOrdered);
        }
    }

    public bool IsConnectionValid(INetworkInterfaceConnection connection)
    {
        if (Disposed)
            return false;
        return _networkInterface.Connections.Contains(connection);
    }

    public CoroutineOperation BeginLargeDataTransfer(object netMessage, INetworkInterfaceConnection connection, string description = "")
    {
        if (Disposed)
            return null;

        if (NetMessageInterpreter.GetDataFromMessage(netMessage, out byte[] messageData, byteLimit: int.MaxValue))
        {
            Log.Info(s_netMessageLogChannel, $"[Session] BeginLargeDataTransfer '{netMessage}-{description}' to connection {connection.Id}");

            CoroutineOperation op;
            if (s_transferWithStream)
            {
                op = new SendViaStreamChannelOperation(messageData, connection, this, description);
            }
            else
            {
                op = new SendViaManualPacketsOperation(messageData, connection, this, description);
            }

            _outgoingDataTransfers.Add(op);

            op.Execute();


            return op;
        }
        else
        {
            return null;
        }
    }

    void OnReceiveDataTransferHeader(NetMessageViaManualPacketsHeader netMessage, INetworkInterfaceConnection source)
    {
        Log.Info(s_netMessageLogChannel, $"[Session] OnReceiveDataTransferHeader '{netMessage}-{netMessage.Description}' from connection {source.Id}");

        ReceiveViaManualPacketsOperation op = new ReceiveViaManualPacketsOperation(netMessage, source, this);

        _incomingDataTransfers.Add(op);

        op.OnDataReceived += (o, data) =>
        {
            OnReceiveMessage(source, data);
        };

        op.Execute();

        OnBeginReceiveLargeDataTransfer?.Invoke(op, source);
    }

    void OnReceiveDataTransferHeader(NetMessageViaStreamChannelHeader netMessage, INetworkInterfaceConnection source)
    {
        Log.Info(s_netMessageLogChannel, $"[Session] OnReceiveDataTransferHeader '{netMessage}-{netMessage.Description}' from connection {source.Id}");

        ReceiveViaStreamChannelOperation op = new ReceiveViaStreamChannelOperation(netMessage, source, this);

        _incomingDataTransfers.Add(op);

        op.OnDataReceived += (o, data) =>
        {
            OnReceiveMessage(source, data);
        };

        op.Execute();

        OnBeginReceiveLargeDataTransfer?.Invoke(op, source);
    }

    void Interface_OnConnect(INetworkInterfaceConnection connection)
    {
        OnConnectionAdded?.Invoke(connection);
    }

    void InterfaceOnDisconnect(INetworkInterfaceConnection connection)
    {
        OnConnectionRemoved?.Invoke(connection);
    }

    protected virtual void OnReceiveMessage(INetworkInterfaceConnection source, byte[] data)
    {
        if (NetMessageInterpreter.GetMessageFromData(data, out object netMessage))
        {
            Log.Info(s_netMessageLogChannel, $"[Session] Received message '{netMessage}' from connection {source.Id}");

            if (netMessage != null)
            {
                Type t = netMessage.GetType();

                if (_netMessageReceivers.ContainsKey(t))
                {
                    _netMessageReceivers[t].OnReceive(netMessage, source);
                }
            }
        }
    }

    protected NetworkInterface _networkInterface;
    protected Dictionary<Type, NetMessageReceiverList> _netMessageReceivers = new Dictionary<Type, NetMessageReceiverList>();
    protected List<CoroutineOperation> _outgoingDataTransfers = new List<CoroutineOperation>();
    protected List<CoroutineOperation> _incomingDataTransfers = new List<CoroutineOperation>();
    protected bool Disposed { get; private set; }

    protected abstract class NetMessageReceiverList
    {
        public abstract void OnReceive(object netMessage, INetworkInterfaceConnection source);
        public abstract void AddListener(object callback);
        public abstract void RemoveListener(object callback);
    }

    protected class NetMessageReceiverList<NetMessageType> : NetMessageReceiverList
    {
        public List<Action<NetMessageType, INetworkInterfaceConnection>> Listeners = new List<Action<NetMessageType, INetworkInterfaceConnection>>();

        public override void AddListener(object callback)
        {
            Listeners.Add((Action<NetMessageType, INetworkInterfaceConnection>)callback);
        }

        public override void RemoveListener(object callback)
        {
            Listeners.Remove((Action<NetMessageType, INetworkInterfaceConnection>)callback);
        }

        public override void OnReceive(object netMessage, INetworkInterfaceConnection source)
        {
            NetMessageType castedMessage = (NetMessageType)netMessage;
            for (int i = Listeners.Count - 1; i >= 0; i--)
            {
                Listeners[i].Invoke(castedMessage, source);
            }
        }
    }
}
