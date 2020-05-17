using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INetworkInterfaceConnection
{
    uint Id { get; }
    void SetStreamBandwidth(int bytesPerSecond);
    void StreamBytes(IStreamChannel channel, byte[] data);
}
