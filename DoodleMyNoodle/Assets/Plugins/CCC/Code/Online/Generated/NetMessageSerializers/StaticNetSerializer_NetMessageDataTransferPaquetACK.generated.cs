// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageDataTransferPaquetACK
{
    public static int GetNetBitSize(ref NetMessageDataTransferPaquetACK obj)
    {
        int result = 0;
        result += StaticNetSerializer_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_Int32.GetNetBitSize(ref obj.PaquetIndex);
        return result;
    }

    public static void NetSerialize(ref NetMessageDataTransferPaquetACK obj, BitStreamWriter writer)
    {
        StaticNetSerializer_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_Int32.NetSerialize(ref obj.PaquetIndex, writer);
    }

    public static void NetDeserialize(ref NetMessageDataTransferPaquetACK obj, BitStreamReader reader)
    {
        StaticNetSerializer_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_Int32.NetDeserialize(ref obj.PaquetIndex, reader);
    }
}
