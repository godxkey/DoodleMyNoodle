// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageAcceptSimSync
{
    public static int GetNetBitSize(ref NetMessageAcceptSimSync obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.TransferByDisk);
        return result;
    }

    public static void NetSerialize(ref NetMessageAcceptSimSync obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.TransferByDisk, writer);
    }

    public static void NetDeserialize(ref NetMessageAcceptSimSync obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.TransferByDisk, reader);
    }
}
