// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageDataTransferCancel
{
    public static int GetNetBitSize(ref NetMessageDataTransferCancel obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        return result;
    }

    public static void NetSerialize(ref NetMessageDataTransferCancel obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
    }

    public static void NetDeserialize(ref NetMessageDataTransferCancel obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
    }
}
