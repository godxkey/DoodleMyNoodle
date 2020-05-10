// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessagePacket obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.PacketIndex);
        result += ArrayNetSerializer_System_Byte.GetNetBitSize(ref obj.Data);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessagePacket obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.PacketIndex, writer);
        ArrayNetSerializer_System_Byte.NetSerialize(ref obj.Data, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessagePacket obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.PacketIndex, reader);
        ArrayNetSerializer_System_Byte.NetDeserialize(ref obj.Data, reader);
    }
}
