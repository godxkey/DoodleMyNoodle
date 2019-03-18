using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionClientInterface : SessionInterface
{
    public SessionClientInterface(NetworkInterface networkInterface) : base(networkInterface) { }

    public override bool isServerType => false;

    public INetworkInterfaceConnection ServerConnection
    {
        get
        {
            if (_networkInterface.connections.Count > 0)
            {
                return _networkInterface.connections[0];
            }
            else
            {
                return null;
            }
        }
    }

    public void SendNetMessage(NetMessage netMessage)
    {
        _netMessagesToSend.Add(new MessageAndConnection()
        {
            connection = ServerConnection,
            message = netMessage
        });
    }
}
