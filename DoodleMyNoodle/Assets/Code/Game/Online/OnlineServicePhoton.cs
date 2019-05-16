using Internals.PhotonNetwokInterface;
using System;

namespace Internals.OnlineServiceImpl
{
    public class OnlineServicePhoton : OnlineService
    {
        public static Func<IDynamicNetSerializerImpl> factoryCreator;

        protected override IDynamicNetSerializerImpl CreateNetMessageFactory()
        {
            return factoryCreator();
        }

        protected override NetworkInterface CreateNetworkInterface()
        {
            return new PhotonNetworkInterface();
        }
    }
}