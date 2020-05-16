// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.DataSize);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.PacketCount);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Description);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.DataSize, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.PacketCount, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.Description, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.DataSize, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.PacketCount, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Description, reader);
    }
}
