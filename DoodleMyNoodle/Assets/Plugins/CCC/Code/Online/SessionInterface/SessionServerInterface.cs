using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionServerInterface : SessionInterface
{
    public SessionServerInterface(NetworkInterface networkInterface) : base(networkInterface) { }

    public override bool isServerType => true;

    public void SendNetMessage(INetworkInterfaceConnection connection, NetMessage netMessage)
    {
        _netMessagesToSend.Add(new MessageAndConnection()
        {
            connection = connection,
            message = netMessage
        });
    }
}
