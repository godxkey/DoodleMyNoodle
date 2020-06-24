// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageCancel obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageCancel obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageCancel obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
    }
}
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
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessagePacketACK obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.PacketIndex);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessagePacketACK obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.PacketIndex, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessagePacketACK obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.PacketIndex, reader);
    }
}
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
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamACK obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageViaStreamACK obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamACK obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj.DataSize);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.Description);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.NetSerialize(ref obj.DataSize, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.Description, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.NetDeserialize(ref obj.DataSize, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.Description, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamReady obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TransferId);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageViaStreamReady obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TransferId, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamReady obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TransferId, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamUpdate
{
    public static int GetNetBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Single.GetNetBitSize(ref obj.Progress);
        return result;
    }

    public static void NetSerialize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Single.NetSerialize(ref obj.Progress, writer);
    }

    public static void NetDeserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Single.NetDeserialize(ref obj.Progress, reader);
    }
}
public static class StaticNetSerializer_NetMessageDestroyValue
{
    public static int GetNetBitSize(ref NetMessageDestroyValue obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetNetBitSize(ref obj.TypeId);
        return result;
    }

    public static void NetSerialize(ref NetMessageDestroyValue obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.NetSerialize(ref obj.TypeId, writer);
    }

    public static void NetDeserialize(ref NetMessageDestroyValue obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.NetDeserialize(ref obj.TypeId, reader);
    }
}
public static class StaticNetSerializer_NetMessageRequestValueSync
{
    public static int GetNetBitSize(ref NetMessageRequestValueSync obj)
    {
        int result = 0;
        return result;
    }

    public static void NetSerialize(ref NetMessageRequestValueSync obj, BitStreamWriter writer)
    {
    }

    public static void NetDeserialize(ref NetMessageRequestValueSync obj, BitStreamReader reader)
    {
    }
}
public static class StaticNetSerializer_NetMessageSyncValue
{
    public static int GetNetBitSize(ref NetMessageSyncValue obj)
    {
        int result = 0;
        result += ArrayNetSerializer_System_Byte.GetNetBitSize(ref obj.ValueData);
        return result;
    }

    public static void NetSerialize(ref NetMessageSyncValue obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_System_Byte.NetSerialize(ref obj.ValueData, writer);
    }

    public static void NetDeserialize(ref NetMessageSyncValue obj, BitStreamReader reader)
    {
        ArrayNetSerializer_System_Byte.NetDeserialize(ref obj.ValueData, reader);
    }
}
public static class StaticNetSerializer_NetMessageValueSyncComplete
{
    public static int GetNetBitSize(ref NetMessageValueSyncComplete obj)
    {
        int result = 0;
        return result;
    }

    public static void NetSerialize(ref NetMessageValueSyncComplete obj, BitStreamWriter writer)
    {
    }

    public static void NetDeserialize(ref NetMessageValueSyncComplete obj, BitStreamReader reader)
    {
    }
}
