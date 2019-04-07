using Internals.PhotonNetwokInterface;

namespace Internals.OnlineServiceImpl
{
    public class OnlineServicePhoton : OnlineService
    {
        protected override INetMessageFactoryImpl CreateNetMessageFactory()
        {
            return new NetMessageFactoryImpl();
        }

        protected override NetworkInterface CreateNetworkInterface()
        {
            return new PhotonNetworkInterface();
        }
    }
}