// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageDataTransferPaquet
{
    public static int GetNetBitSize(ref NetMessageDataTransferPaquet obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.PaquetIndex);
        result += ArrayNetSerializer_System_Byte.GetNetBitSize(ref obj.Data);
        return result;
    }

    public static void NetSerialize(ref NetMessageDataTransferPaquet obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.PaquetIndex, writer);
        ArrayNetSerializer_System_Byte.NetSerialize(ref obj.Data, writer);
    }

    public static void NetDeserialize(ref NetMessageDataTransferPaquet obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.PaquetIndex, reader);
        ArrayNetSerializer_System_Byte.NetDeserialize(ref obj.Data, reader);
    }
}
