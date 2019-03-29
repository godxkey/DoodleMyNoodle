using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class SessionInterface : IDisposable
{
    public abstract bool isServerType { get; }
    public bool isClientType => !isServerType;
    public INetworkInterfaceSession sessionInfo => _networkInterface.connectedSessionInfo;
    public ReadOnlyCollection<INetworkInterfaceConnection> connections => _networkInterface.connections;
    public event Action onTerminate;
    public event Action<INetworkInterfaceConnection> onConnectionAdded;
    public event Action<INetworkInterfaceConnection> onConnectionRemoved;

    public SessionInterface(NetworkInterface networkInterface)
    {
        _networkInterface = networkInterface;
        _networkInterface.SetMessageReader(OnReceiveMessage);

        _networkInterface.onDisconnect += InterfaceOnDisconnect;
        _networkInterface.onConnect += Interface_OnConnect;
        DebugService.Log("Session interface created");
    }

    public void Dispose()
    {
        DebugService.Log("Session interface terminating");
        onTerminate?.Invoke();

        _networkInterface.onDisconnect -= InterfaceOnDisconnect;
        _networkInterface.onConnect -= Interface_OnConnect;
    }

    public virtual void Update()
    {
        // Send net messages
        foreach (MessageAndConnection msg in _netMessagesToSend)
        {
            byte[] messageData;
            NetMessageInterpreter.GetDataFromMessage(msg.message, out messageData);
            _networkInterface.SendMessage(msg.connection, messageData, messageData.Length);
        }
        _netMessagesToSend.Clear();

        // Act on received net messages
        foreach (MessageAndConnection msg in _netMessagesReceived)
        {
            Type t = msg.message.GetType();

            if (_netMessageReceivers.ContainsKey(t))
            {
                _netMessageReceivers[t].OnReceive(msg.message, msg.connection);
            }
        }
        _netMessagesReceived.Clear();
    }

    public void RegisterNetMessageReceiver<NetMessageType>(Action<NetMessageType, INetworkInterfaceConnection> callback)
        where NetMessageType : INetSerializable
    {
        Type t = typeof(NetMessageType);

        if (_netMessageReceivers.ContainsKey(t) == false)
        {
            _netMessageReceivers.Add(t, new NetMessageReceiverList<NetMessageType>());
        }

        _netMessageReceivers[t].AddListener(callback);
    }

    public void UnregisterNetMessageReceiver<NetMessageType>(Action<NetMessageType, INetworkInterfaceConnection> callback)
        where NetMessageType : INetSerializable
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

    public void SendNetMessage(INetworkInterfaceConnection connection, INetSerializable netMessage)
    {
        _netMessagesToSend.Add(new MessageAndConnection()
        {
            connection = connection,
            message = netMessage
        });
    }

    void Interface_OnConnect(INetworkInterfaceConnection connection)
    {
        onConnectionAdded?.Invoke(connection);
    }

    void InterfaceOnDisconnect(INetworkInterfaceConnection connection)
    {
        onConnectionRemoved?.Invoke(connection);
    }

    protected virtual void OnReceiveMessage(INetworkInterfaceConnection source, byte[] data, int messageSize)
    {
        INetSerializable netMessage = NetMessageInterpreter.GetMessageFromData(data);

        if(netMessage != null)
        {
            // FOR NOW, we don't care about the messageSize
            _netMessagesReceived.Add(new MessageAndConnection()
            {
                connection = source,
                message = NetMessageInterpreter.GetMessageFromData(data)
            });
        }
    }

    protected NetworkInterface _networkInterface;
    protected List<MessageAndConnection> _netMessagesToSend = new List<MessageAndConnection>();
    protected List<MessageAndConnection> _netMessagesReceived = new List<MessageAndConnection>();
    protected Dictionary<Type, NetMessageReceiverList> _netMessageReceivers = new Dictionary<Type, NetMessageReceiverList>();

    protected abstract class NetMessageReceiverList
    {
        public abstract void OnReceive(INetSerializable netMessage, INetworkInterfaceConnection source);
        public abstract void AddListener(object callback);
        public abstract void RemoveListener(object callback);
    }

    protected class NetMessageReceiverList<NetMessageType> : NetMessageReceiverList
        where NetMessageType : INetSerializable
    {
        public List<Action<NetMessageType, INetworkInterfaceConnection>> _listeners = new List<Action<NetMessageType, INetworkInterfaceConnection>>();

        public override void AddListener(object callback)
        {
            _listeners.Add((Action<NetMessageType, INetworkInterfaceConnection>)callback);
        }

        public override void RemoveListener(object callback)
        {
            _listeners.Remove((Action<NetMessageType, INetworkInterfaceConnection>)callback);
        }

        public override void OnReceive(INetSerializable netMessage, INetworkInterfaceConnection source)
        {
            NetMessageType castedMessage = (NetMessageType)netMessage;
            for (int i = _listeners.Count - 1; i >= 0; i--)
            {
                _listeners[i].Invoke(castedMessage, source);
            }
        }
    }

    protected struct MessageAndConnection
    {
        public INetworkInterfaceConnection connection;
        public INetSerializable message;
    }
}
