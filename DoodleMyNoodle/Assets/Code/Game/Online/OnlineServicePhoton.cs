using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnlineServicePhoton : OnlineService
{
    protected override INetMessageFactory CreateNetMessageFactory()
    {
        return new NetMessageFactoryImpl();
    }

    protected override NetworkInterface CreateNetworkInterface()
    {
        return new PhotonNetworkInterface();
    }
}
