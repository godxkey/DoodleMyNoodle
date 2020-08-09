using CCC.Online;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionClientInterface : SessionInterface
{
    public SessionClientInterface(NetworkInterface networkInterface) : base(networkInterface) { }

    public override bool IsServerType => false;

    public INetworkInterfaceConnection ServerConnection
    {
        get
        {
            if (_networkInterface.Connections.Count > 0)
            {
                return _networkInterface.Connections[0];
            }
            else
            {
                return null;
            }
        }
    }

    public void SendNetMessageToServer<T>(T netMessage)
    {
        SendNetMessage(netMessage, ServerConnection);
    }
}
