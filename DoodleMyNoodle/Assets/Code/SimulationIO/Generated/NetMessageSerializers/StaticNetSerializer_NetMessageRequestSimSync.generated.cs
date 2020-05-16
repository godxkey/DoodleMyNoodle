// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageRequestSimSync
{
    public static int GetNetBitSize(ref NetMessageRequestSimSync obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetNetBitSize(ref obj.AttemptTransferByDisk);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.LocalMachineName);
        return result;
    }

    public static void NetSerialize(ref NetMessageRequestSimSync obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.NetSerialize(ref obj.AttemptTransferByDisk, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.LocalMachineName, writer);
    }

    public static void NetDeserialize(ref NetMessageRequestSimSync obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.NetDeserialize(ref obj.AttemptTransferByDisk, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.LocalMachineName, reader);
    }
}
