
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
    }
}