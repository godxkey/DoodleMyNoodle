// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.DataSize);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Description);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.ChannelName);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.DataSize, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.Description, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.ChannelName, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.DataSize, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Description, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.ChannelName, reader);
    }
}
