using Internals.PhotonNetwokInterface;

namespace Internals.OnlineServiceImpl
{
    public class OnlineServicePhoton : OnlineService
    {
        protected override IDynamicNetSerializerImpl CreateNetMessageFactory()
        {
            return new DynamicNetSerializerImpl();
        }

        protected override NetworkInterface CreateNetworkInterface()
        {
            return new PhotonNetworkInterface();
        }
    }
}