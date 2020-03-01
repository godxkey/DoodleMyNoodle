using CCC.Online;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionServerInterface : SessionInterface
{
    private SyncedValueContainerManagerMaster _syncedValueManager;

    public SessionServerInterface(NetworkInterface networkInterface) : base(networkInterface)
    {
        _syncedValueManager = new SyncedValueContainerManagerMaster(this);
    }

    public override bool IsServerType => true;

    public override void Dispose()
    {
        _syncedValueManager.Dispose();

        base.Dispose();
    }
}
