
namespace Internals.PhotonNetwokInterface
{
    class PhotonNetworkInterfaceConnection : INetworkInterfaceConnection
    {
        public BoltConnection BoltConnection { get; private set; }

        public PhotonNetworkInterfaceConnection(BoltConnection boltConnection)
        {
            BoltConnection = boltConnection;
        }

        public uint Id => BoltConnection.ConnectionId;

        public void SetStreamBandwidth(int bytesPerSecond)
        {
            BoltConnection.SetStreamBandwidth(bytesPerSecond);
        }

        public void StreamBytes(IStreamChannel channel, byte[] data)
        {
            BoltConnection.StreamBytes(((StreamChannel)channel).UdpChannelName, data);
        }
    }
}