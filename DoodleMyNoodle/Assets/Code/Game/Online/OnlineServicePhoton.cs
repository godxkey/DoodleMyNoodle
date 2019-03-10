using Internals.PhotonNetwokInterface;

namespace Internals.OnlineServiceImpl
{
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
}