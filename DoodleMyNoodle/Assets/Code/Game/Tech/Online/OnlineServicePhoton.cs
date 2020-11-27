using Internals.PhotonNetworkInterface;
using System;

namespace Internals.OnlineServiceImpl
{
    public class OnlineServicePhoton : OnlineService
    {
        public static Func<INetSerializerImpl> factoryCreator;

        protected override INetSerializerImpl CreateNetMessageFactory()
        {
            return factoryCreator();
        }

        protected override NetworkInterface CreateNetworkInterface()
        {
            return new PhotonNetworkInterface.PhotonNetworkInterface();
        }
    }
}