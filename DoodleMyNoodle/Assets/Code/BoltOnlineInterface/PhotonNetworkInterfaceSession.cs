using System;
using UdpKit;

namespace Internals.PhotonNetworkInterface
{
    public class PhotonNetworkInterfaceSession : INetworkInterfaceSession
    {
        public PhotonNetworkInterfaceSession(UdpSession udpSession)
        {
            UdpSession = udpSession;
        }

        public UdpSession UdpSession { get; private set; }

        public Guid Id => UdpSession.Id;

        public int ConnectionsMax => UdpSession.ConnectionsMax;

        public int ConnectionsCurrent => UdpSession.ConnectionsCurrent;

        public string HostName => UdpSession.HostName;
    }
}