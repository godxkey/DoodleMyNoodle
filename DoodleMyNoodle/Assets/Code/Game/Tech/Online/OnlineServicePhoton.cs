using Internals.PhotonNetworkInterface;
using System;

namespace Internals.OnlineServiceImpl
{
    public class OnlineServicePhoton : OnlineService
    {
        public static Func<INetSerializerImpl> FactoryCreator;

        protected override INetSerializerImpl CreateNetMessageFactory()
        {
            return FactoryCreator();
        }

        protected override NetworkInterface CreateNetworkInterface()
        {
            return new PhotonNetworkInterface.PhotonNetworkInterface();
        }
    }
}