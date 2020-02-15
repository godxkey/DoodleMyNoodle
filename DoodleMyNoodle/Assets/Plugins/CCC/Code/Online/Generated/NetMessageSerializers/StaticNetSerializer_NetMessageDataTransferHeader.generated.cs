// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageDataTransferHeader
{
    public static int GetNetBitSize(ref NetMessageDataTransferHeader obj)
    {
        int result = 0;
        result += StaticNetSerializer_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_Int32.GetNetBitSize(ref obj.DataSize);
        result += StaticNetSerializer_Int32.GetNetBitSize(ref obj.PaquetCount);
        result += StaticNetSerializer_String.GetNetBitSize(ref obj.Description);
        return result;
    }

    public static void NetSerialize(ref NetMessageDataTransferHeader obj, BitStreamWriter writer)
    {
        StaticNetSerializer_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_Int32.NetSerialize(ref obj.DataSize, writer);
        StaticNetSerializer_Int32.NetSerialize(ref obj.PaquetCount, writer);
        StaticNetSerializer_String.NetSerialize(ref obj.Description, writer);
    }

    public static void NetDeserialize(ref NetMessageDataTransferHeader obj, BitStreamReader reader)
    {
        StaticNetSerializer_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_Int32.NetDeserialize(ref obj.DataSize, reader);
        StaticNetSerializer_Int32.NetDeserialize(ref obj.PaquetCount, reader);
        StaticNetSerializer_String.NetDeserialize(ref obj.Description, reader);
    }
}
