using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using CCC.Online.DataTransfer;
using CCC.Operations;

public abstract class SessionInterface : IDisposable
{
    const bool LOG = false;

#if DEBUG_BUILD
    [ConfigVar(name: "log.netmessage", defaultValue: "0", ConfigVarFlag.Save, "Should we log the sent/received NetMessages.")]
    static ConfigVar s_logNetMessages;
#endif

    public abstract bool IsServerType { get; }
    public bool IsClientType => !IsServerType;
    public INetworkInterfaceSession SessionInfo => _networkInterface.ConnectedSessionInfo;
    public ReadOnlyList<INetworkInterfaceConnection> Connections => _networkInterface.Connections;
    public ReadOnlyList<ReceiveViaManualPacketsOperation> IncomingDataTransfers => _incomingDataTransfers.AsReadOnlyNoAlloc();
    public ReadOnlyList<CoroutineOperation> OutgoingDataTransfer => _outgoingDataTransfers.AsReadOnlyNoAlloc();
    public event Action OnTerminate;
    public event Action<INetworkInterfaceConnection> OnConnectionAdded;
    public event Action<INetworkInterfaceConnection> OnConnectionRemoved;
    public event Action<ReceiveViaManualPacketsOperation, INetworkInterfaceConnection> OnBeginReceiveLargeDataTransfer;
    internal NetworkInterface NetworkInterface => _networkInterface;


    public SessionInterface(NetworkInterface networkInterface)
    {
        _networkInterface = networkInterface;
        _networkInterface.SetMessageReader(OnReceiveMessage);

        _networkInterface.OnDisconnect += InterfaceOnDisconnect;
        _networkInterface.OnConnect += Interface_OnConnect;

        RegisterNetMessageReceiver<NetMessageViaManualPacketsHeader>(OnReceiveDataTransferHeader);
    }

    public virtual void Dispose()
    {
        OnTerminate?.Invoke();

        _networkInterface.OnDisconnect -= InterfaceOnDisconnect;
        _networkInterface.OnConnect -= Interface_OnConnect;
    }

    public virtual void Update()
    {
        // nothing to do

        _outgoingDataTransfers.RemoveAll((x) => !x.IsRunning);
        _incomingDataTransfers.RemoveAll((x) => !x.IsRunning);
    }

    public void RegisterNetMessageReceiver<NetMessageType>(Action<NetMessageType, INetworkInterfaceConnection> callback)
    {
        Type t = typeof(NetMessageType);

        if (_netMessageReceivers.ContainsKey(t) == false)
        {
            _netMessageReceivers.Add(t, new NetMessageReceiverList<NetMessageType>());
        }

        _netMessageReceivers[t].AddListener(callback);
    }

    public void UnregisterNetMessageReceiver<NetMessageType>(Action<NetMessageType, INetworkInterfaceConnection> callback)
    {
        Type t = typeof(NetMessageType);

        if (_netMessageReceivers.ContainsKey(t))
        {
            _netMessageReceivers[t].RemoveListener(callback);
        }
    }

    public void Disconnect()
    {
        _networkInterface.DisconnectFromSession(null);
    }

    public void SendNetMessage<T>(in T netMessage, INetworkInterfaceConnection connection, bool reliableAndOrdered = true)
    {
        if (NetMessageInterpreter.GetDataFromMessage(netMessage, out byte[] messageData, byteLimit: OnlineConstants.MAX_MESSAGE_SIZE))
        {
            _networkInterface.SendMessage(connection, messageData, reliableAndOrdered);

#if DEBUG_BUILD
            if (s_logNetMessages.BoolValue)
            {
                DebugService.Log($"[Session] Send message '{netMessage}' to connection {connection.Id}");
            }
#endif
        }
    }

    public bool IsConnectionValid(INetworkInterfaceConnection connection)
    {
        return _networkInterface.Connections.Contains(connection);
    }

    public CoroutineOperation BeginLargeDataTransfer(object netMessage, INetworkInterfaceConnection connection, string description = "")
    {
        if (NetMessageInterpreter.GetDataFromMessage(netMessage, out byte[] messageData, byteLimit: int.MaxValue))
        {
            SendViaStreamChannelOperation op = new SendViaStreamChannelOperation(messageData, connection, this, description);
            //SendViaManualPacketsOperation op = new SendViaManualPacketsOperation(messageData, connection, this, description);

            _outgoingDataTransfers.Add(op);

            op.Execute();

#if DEBUG_BUILD
            if (s_logNetMessages.BoolValue)
            {
                DebugService.Log($"[Session] BeginLargeDataTransfer '{netMessage}-{description}' to connection {connection.Id}");
            }
#endif

            return op;
        }
        else
        {
            return null;
        }
    }

    void OnReceiveDataTransferHeader(NetMessageViaManualPacketsHeader netMessage, INetworkInterfaceConnection source)
    {
        ReceiveViaManualPacketsOperation op = new ReceiveViaManualPacketsOperation(netMessage, source, this);

        _incomingDataTransfers.Add(op);

        op.OnDataReceived += (o, data) =>
        {
            OnReceiveMessage(source, data);
        };

        op.Execute();

#if DEBUG_BUILD
        if (s_logNetMessages.BoolValue)
        {
            DebugService.Log($"[Session] OnReceiveDataTransferHeader '{netMessage}-{netMessage.Description}' from connection {source.Id}");
        }
#endif

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
#if DEBUG_BUILD
            if (s_logNetMessages.BoolValue)
            {
                DebugService.Log($"[Session] Received message '{netMessage}' from connection {source.Id}");
            }
#endif

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
    protected List<ReceiveViaManualPacketsOperation> _incomingDataTransfers = new List<ReceiveViaManualPacketsOperation>();

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
