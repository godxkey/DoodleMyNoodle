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
    public Action<NetMessage, INetworkInterfaceConnection> netMessageReceiver;

    public SessionInterface(NetworkInterface networkInterface)
    {
        _networkInterface = networkInterface;
        _networkInterface.SetMessageReader(OnReceiveMessage);
        Debug.Log("Session interface created");
    }

    public void Dispose()
    {
        Debug.Log("Session interface terminating");
        onTerminate?.Invoke();
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
            netMessageReceiver?.Invoke(msg.message, msg.connection);
        }
        _netMessagesReceived.Clear();
    }

    public void Disconnect()
    {
        _networkInterface.DisconnectFromSession(null);
    }

    protected virtual void OnReceiveMessage(INetworkInterfaceConnection source, byte[] data, int messageSize)
    {
        // FOR NOW, we don't care about the messageSize
        _netMessagesReceived.Add(new MessageAndConnection()
        {
            connection = source,
            message = NetMessageInterpreter.GetMessageFromData(data)
        });
    }

    protected NetworkInterface _networkInterface;
    protected List<MessageAndConnection> _netMessagesToSend = new List<MessageAndConnection>();
    protected List<MessageAndConnection> _netMessagesReceived = new List<MessageAndConnection>();

    protected struct MessageAndConnection
    {
        public INetworkInterfaceConnection connection;
        public NetMessage message;
    }
}
