using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

public abstract class SessionInterface : IDisposable
{
    public abstract bool IsServerType { get; }
    public bool IsClientType => !IsServerType;
    public INetworkInterfaceSession SessionInfo => _networkInterface.ConnectedSessionInfo;
    public ReadOnlyCollection<INetworkInterfaceConnection> Connections => _networkInterface.Connections;
    public event Action OnTerminate;
    public Action<NetMessage, INetworkInterfaceConnection> NetMessageReceiver;

    public SessionInterface(NetworkInterface networkInterface)
    {
        _networkInterface = networkInterface;
        _networkInterface.SetMessageReader(OnReceiveMessage);
        Debug.Log("Session interface created");
    }

    public void Dispose()
    {
        Debug.Log("Session interface terminating");
        OnTerminate?.Invoke();
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
            NetMessageReceiver?.Invoke(msg.message, msg.connection);
        }
        _netMessagesReceived.Clear();
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
