using CCC.Online;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionClientInterface : SessionInterface
{
    SyncedValueContainerManagerClient _syncedValueManager;

    public SessionClientInterface(NetworkInterface networkInterface) : base(networkInterface)
    {
        _syncedValueManager = new SyncedValueContainerManagerClient(this);
    }

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

    public void SendNetMessageToServer(object netMessage)
    {
        SendNetMessage(netMessage, ServerConnection);
    }

    public override void Dispose()
    {
        _syncedValueManager.Dispose();

        base.Dispose();
    }
}
