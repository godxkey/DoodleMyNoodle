// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageCancel
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessageCancel obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TransferId);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessageCancel obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TransferId, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessageCancel obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TransferId, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacket
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessagePacket obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.PacketIndex);
        result += ArrayNetSerializer_System_Byte.GetSerializedBitSize(ref obj.Data);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessagePacket obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.PacketIndex, writer);
        ArrayNetSerializer_System_Byte.Serialize(ref obj.Data, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessagePacket obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.PacketIndex, reader);
        ArrayNetSerializer_System_Byte.Deserialize(ref obj.Data, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessagePacketACK
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessagePacketACK obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.PacketIndex);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessagePacketACK obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.PacketIndex, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessagePacketACK obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.PacketIndex, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaManualPacketsHeader
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.DataSize);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.PacketCount);
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.Description);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.DataSize, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.PacketCount, writer);
        StaticNetSerializer_System_String.Serialize(ref obj.Description, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessageViaManualPacketsHeader obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.DataSize, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.PacketCount, reader);
        StaticNetSerializer_System_String.Deserialize(ref obj.Description, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamACK
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamACK obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TransferId);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessageViaStreamACK obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TransferId, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamACK obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TransferId, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamChannelHeader
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TransferId);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.DataSize);
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.Description);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TransferId, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.DataSize, writer);
        StaticNetSerializer_System_String.Serialize(ref obj.Description, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamChannelHeader obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TransferId, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.DataSize, reader);
        StaticNetSerializer_System_String.Deserialize(ref obj.Description, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamReady
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamReady obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TransferId);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessageViaStreamReady obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TransferId, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamReady obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TransferId, reader);
    }
}
public static class StaticNetSerializer_CCC_Online_DataTransfer_NetMessageViaStreamUpdate
{
    public static int GetSerializedBitSize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Single.GetSerializedBitSize(ref obj.Progress);
        return result;
    }

    public static void Serialize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Single.Serialize(ref obj.Progress, writer);
    }

    public static void Deserialize(ref CCC.Online.DataTransfer.NetMessageViaStreamUpdate obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Single.Deserialize(ref obj.Progress, reader);
    }
}
public static class StaticNetSerializer_fix
{
    public static int GetSerializedBitSize(ref fix obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int64.GetSerializedBitSize(ref obj.RawValue);
        return result;
    }

    public static void Serialize(ref fix obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int64.Serialize(ref obj.RawValue, writer);
    }

    public static void Deserialize(ref fix obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int64.Deserialize(ref obj.RawValue, reader);
    }
}
public static class StaticNetSerializer_fix2
{
    public static int GetSerializedBitSize(ref fix2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.y);
        return result;
    }

    public static void Serialize(ref fix2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.x, writer);
        StaticNetSerializer_fix.Serialize(ref obj.y, writer);
    }

    public static void Deserialize(ref fix2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.x, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.y, reader);
    }
}
public static class StaticNetSerializer_fix2x2
{
    public static int GetSerializedBitSize(ref fix2x2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M22);
        return result;
    }

    public static void Serialize(ref fix2x2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.M11, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M12, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M21, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M22, writer);
    }

    public static void Deserialize(ref fix2x2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M22, reader);
    }
}
public static class StaticNetSerializer_fix2x3
{
    public static int GetSerializedBitSize(ref fix2x3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M13);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M23);
        return result;
    }

    public static void Serialize(ref fix2x3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.M11, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M12, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M13, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M21, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M22, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M23, writer);
    }

    public static void Deserialize(ref fix2x3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M13, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M23, reader);
    }
}
public static class StaticNetSerializer_fix3
{
    public static int GetSerializedBitSize(ref fix3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.y);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.z);
        return result;
    }

    public static void Serialize(ref fix3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.x, writer);
        StaticNetSerializer_fix.Serialize(ref obj.y, writer);
        StaticNetSerializer_fix.Serialize(ref obj.z, writer);
    }

    public static void Deserialize(ref fix3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.x, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.y, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.z, reader);
    }
}
public static class StaticNetSerializer_fix3x2
{
    public static int GetSerializedBitSize(ref fix3x2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M31);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M32);
        return result;
    }

    public static void Serialize(ref fix3x2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.M11, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M12, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M21, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M22, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M31, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M32, writer);
    }

    public static void Deserialize(ref fix3x2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M31, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M32, reader);
    }
}
public static class StaticNetSerializer_fix3x3
{
    public static int GetSerializedBitSize(ref fix3x3 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M13);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M23);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M31);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M32);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M33);
        return result;
    }

    public static void Serialize(ref fix3x3 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.M11, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M12, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M13, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M21, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M22, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M23, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M31, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M32, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M33, writer);
    }

    public static void Deserialize(ref fix3x3 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M13, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M23, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M31, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M32, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M33, reader);
    }
}
public static class StaticNetSerializer_fix4
{
    public static int GetSerializedBitSize(ref fix4 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.y);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.z);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.w);
        return result;
    }

    public static void Serialize(ref fix4 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.x, writer);
        StaticNetSerializer_fix.Serialize(ref obj.y, writer);
        StaticNetSerializer_fix.Serialize(ref obj.z, writer);
        StaticNetSerializer_fix.Serialize(ref obj.w, writer);
    }

    public static void Deserialize(ref fix4 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.x, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.y, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.z, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.w, reader);
    }
}
public static class StaticNetSerializer_fix4x4
{
    public static int GetSerializedBitSize(ref fix4x4 obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M11);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M21);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M31);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M41);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M12);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M22);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M32);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M42);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M13);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M23);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M33);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M43);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M14);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M24);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M34);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.M44);
        return result;
    }

    public static void Serialize(ref fix4x4 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.M11, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M21, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M31, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M41, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M12, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M22, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M32, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M42, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M13, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M23, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M33, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M43, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M14, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M24, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M34, writer);
        StaticNetSerializer_fix.Serialize(ref obj.M44, writer);
    }

    public static void Deserialize(ref fix4x4 obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.M11, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M21, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M31, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M41, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M12, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M22, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M32, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M42, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M13, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M23, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M33, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M43, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M14, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M24, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M34, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.M44, reader);
    }
}
public static class StaticNetSerializer_fixQuaternion
{
    public static int GetSerializedBitSize(ref fixQuaternion obj)
    {
        int result = 0;
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.x);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.y);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.z);
        result += StaticNetSerializer_fix.GetSerializedBitSize(ref obj.w);
        return result;
    }

    public static void Serialize(ref fixQuaternion obj, BitStreamWriter writer)
    {
        StaticNetSerializer_fix.Serialize(ref obj.x, writer);
        StaticNetSerializer_fix.Serialize(ref obj.y, writer);
        StaticNetSerializer_fix.Serialize(ref obj.z, writer);
        StaticNetSerializer_fix.Serialize(ref obj.w, writer);
    }

    public static void Deserialize(ref fixQuaternion obj, BitStreamReader reader)
    {
        StaticNetSerializer_fix.Deserialize(ref obj.x, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.y, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.z, reader);
        StaticNetSerializer_fix.Deserialize(ref obj.w, reader);
    }
}
public static class StaticNetSerializer_GameAction_ParameterData
{
    public static int GetSerializedBitSize_Class(GameAction.ParameterData obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(GameAction.ParameterData obj)
    {
        int result = 0;
        return result;
    }

    public static void Serialize_Class(GameAction.ParameterData obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(GameAction.ParameterData obj, BitStreamWriter writer)
    {
    }

    public static GameAction.ParameterData Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (GameAction.ParameterData)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(GameAction.ParameterData obj, BitStreamReader reader)
    {
    }
}
public static class StaticNetSerializer_GameAction_UseParameters
{
    public static int GetSerializedBitSize_Class(GameAction.UseParameters obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(GameAction.UseParameters obj)
    {
        int result = 0;
        result += ArrayNetSerializer_GameAction_ParameterData.GetSerializedBitSize(ref obj.ParameterDatas);
        return result;
    }

    public static void Serialize_Class(GameAction.UseParameters obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(GameAction.UseParameters obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_GameAction_ParameterData.Serialize(ref obj.ParameterDatas, writer);
    }

    public static GameAction.UseParameters Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameAction.UseParameters obj = new GameAction.UseParameters();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(GameAction.UseParameters obj, BitStreamReader reader)
    {
        ArrayNetSerializer_GameAction_ParameterData.Deserialize(ref obj.ParameterDatas, reader);
    }
}
public static class StaticNetSerializer_GameActionParameterSuccessRate_Data
{
    public static int GetSerializedBitSize_Class(GameActionParameterSuccessRate.Data obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(GameActionParameterSuccessRate.Data obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize();
        result += StaticNetSerializer_GameAction_ParameterData.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(GameActionParameterSuccessRate.Data obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(GameActionParameterSuccessRate.Data obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.Serialize((System.Int32)obj.SuccessRate, writer);
        StaticNetSerializer_GameAction_ParameterData.Serialize(obj, writer);
    }

    public static GameActionParameterSuccessRate.Data Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameActionParameterSuccessRate.Data obj = new GameActionParameterSuccessRate.Data();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(GameActionParameterSuccessRate.Data obj, BitStreamReader reader)
    {
        obj.SuccessRate = (MiniGameSuccessRate)StaticNetSerializer_System_Int32.Deserialize(reader);
        StaticNetSerializer_GameAction_ParameterData.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_GameActionParameterTile_Data
{
    public static int GetSerializedBitSize_Class(GameActionParameterTile.Data obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(GameActionParameterTile.Data obj)
    {
        int result = 0;
        result += StaticNetSerializer_Unity_Mathematics_int2.GetSerializedBitSize(ref obj.Tile);
        result += StaticNetSerializer_GameAction_ParameterData.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(GameActionParameterTile.Data obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(GameActionParameterTile.Data obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Unity_Mathematics_int2.Serialize(ref obj.Tile, writer);
        StaticNetSerializer_GameAction_ParameterData.Serialize(obj, writer);
    }

    public static GameActionParameterTile.Data Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        GameActionParameterTile.Data obj = new GameActionParameterTile.Data();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(GameActionParameterTile.Data obj, BitStreamReader reader)
    {
        StaticNetSerializer_Unity_Mathematics_int2.Deserialize(ref obj.Tile, reader);
        StaticNetSerializer_GameAction_ParameterData.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_InputSubmissionId
{
    public static int GetSerializedBitSize(ref InputSubmissionId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Byte.GetSerializedBitSize(ref obj.value);
        return result;
    }

    public static void Serialize(ref InputSubmissionId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Byte.Serialize(ref obj.value, writer);
    }

    public static void Deserialize(ref InputSubmissionId obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Byte.Deserialize(ref obj.value, reader);
    }
}
public static class StaticNetSerializer_NetMessageAcceptSimSync
{
    public static int GetSerializedBitSize(ref NetMessageAcceptSimSync obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.TransferByDisk);
        return result;
    }

    public static void Serialize(ref NetMessageAcceptSimSync obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.Serialize(ref obj.TransferByDisk, writer);
    }

    public static void Deserialize(ref NetMessageAcceptSimSync obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.TransferByDisk, reader);
    }
}
public static class StaticNetSerializer_NetMessageChatMessage
{
    public static int GetSerializedBitSize(ref NetMessageChatMessage obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetSerializedBitSize(ref obj.playerId);
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.message);
        return result;
    }

    public static void Serialize(ref NetMessageChatMessage obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.Serialize(ref obj.playerId, writer);
        StaticNetSerializer_System_String.Serialize(ref obj.message, writer);
    }

    public static void Deserialize(ref NetMessageChatMessage obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.Deserialize(ref obj.playerId, reader);
        StaticNetSerializer_System_String.Deserialize(ref obj.message, reader);
    }
}
public static class StaticNetSerializer_NetMessageChatMessageSubmission
{
    public static int GetSerializedBitSize(ref NetMessageChatMessageSubmission obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.message);
        return result;
    }

    public static void Serialize(ref NetMessageChatMessageSubmission obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.message, writer);
    }

    public static void Deserialize(ref NetMessageChatMessageSubmission obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.message, reader);
    }
}
public static class StaticNetSerializer_NetMessageClientHello
{
    public static int GetSerializedBitSize(ref NetMessageClientHello obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.playerName);
        return result;
    }

    public static void Serialize(ref NetMessageClientHello obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.playerName, writer);
    }

    public static void Deserialize(ref NetMessageClientHello obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.playerName, reader);
    }
}
public static class StaticNetSerializer_NetMessageDestroyValue
{
    public static int GetSerializedBitSize(ref NetMessageDestroyValue obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.TypeId);
        return result;
    }

    public static void Serialize(ref NetMessageDestroyValue obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.TypeId, writer);
    }

    public static void Deserialize(ref NetMessageDestroyValue obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.TypeId, reader);
    }
}
public static class StaticNetSerializer_NetMessageExample
{
    public static int GetSerializedBitSize_Class(NetMessageExample obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(NetMessageExample obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.valueString);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.valueInt);
        result += StaticNetSerializer_System_UInt32.GetSerializedBitSize(ref obj.valueUInt);
        result += StaticNetSerializer_System_Int16.GetSerializedBitSize(ref obj.valueShort);
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.valueUShort);
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.valueBool);
        result += StaticNetSerializer_System_Byte.GetSerializedBitSize(ref obj.valueByte);
        result += ArrayNetSerializer_System_Int32.GetSerializedBitSize(ref obj.listOnInts);
        return result;
    }

    public static void Serialize_Class(NetMessageExample obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(NetMessageExample obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.valueString, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.valueInt, writer);
        StaticNetSerializer_System_UInt32.Serialize(ref obj.valueUInt, writer);
        StaticNetSerializer_System_Int16.Serialize(ref obj.valueShort, writer);
        StaticNetSerializer_System_UInt16.Serialize(ref obj.valueUShort, writer);
        StaticNetSerializer_System_Boolean.Serialize(ref obj.valueBool, writer);
        StaticNetSerializer_System_Byte.Serialize(ref obj.valueByte, writer);
        ArrayNetSerializer_System_Int32.Serialize(ref obj.listOnInts, writer);
    }

    public static NetMessageExample Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        NetMessageExample obj = new NetMessageExample();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(NetMessageExample obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.valueString, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.valueInt, reader);
        StaticNetSerializer_System_UInt32.Deserialize(ref obj.valueUInt, reader);
        StaticNetSerializer_System_Int16.Deserialize(ref obj.valueShort, reader);
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.valueUShort, reader);
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.valueBool, reader);
        StaticNetSerializer_System_Byte.Deserialize(ref obj.valueByte, reader);
        ArrayNetSerializer_System_Int32.Deserialize(ref obj.listOnInts, reader);
    }
}
public static class StaticNetSerializer_NetMessageInputSubmission
{
    public static int GetSerializedBitSize(ref NetMessageInputSubmission obj)
    {
        int result = 0;
        result += StaticNetSerializer_InputSubmissionId.GetSerializedBitSize(ref obj.submissionId);
        result += StaticNetSerializer_SimInput.GetSerializedBitSize_Class(obj.input);
        return result;
    }

    public static void Serialize(ref NetMessageInputSubmission obj, BitStreamWriter writer)
    {
        StaticNetSerializer_InputSubmissionId.Serialize(ref obj.submissionId, writer);
        StaticNetSerializer_SimInput.Serialize_Class(obj.input, writer);
    }

    public static void Deserialize(ref NetMessageInputSubmission obj, BitStreamReader reader)
    {
        StaticNetSerializer_InputSubmissionId.Deserialize(ref obj.submissionId, reader);
        obj.input = StaticNetSerializer_SimInput.Deserialize_Class(reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerAssets
{
    public static int GetSerializedBitSize(ref NetMessagePlayerAssets obj)
    {
        int result = 0;
        result += ArrayNetSerializer_NetMessagePlayerAssets_Data.GetSerializedBitSize(ref obj.Assets);
        return result;
    }

    public static void Serialize(ref NetMessagePlayerAssets obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_NetMessagePlayerAssets_Data.Serialize(ref obj.Assets, writer);
    }

    public static void Deserialize(ref NetMessagePlayerAssets obj, BitStreamReader reader)
    {
        ArrayNetSerializer_NetMessagePlayerAssets_Data.Deserialize(ref obj.Assets, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerAssets_Data
{
    public static int GetSerializedBitSize(ref NetMessagePlayerAssets.Data obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Guid.GetSerializedBitSize(ref obj.Guid);
        result += StaticNetSerializer_System_Byte.GetSerializedBitSize();
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.Author);
        result += StaticNetSerializer_System_DateTime.GetSerializedBitSize(ref obj.UtcCreationTime);
        result += ArrayNetSerializer_System_Byte.GetSerializedBitSize(ref obj.AssetData);
        return result;
    }

    public static void Serialize(ref NetMessagePlayerAssets.Data obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Guid.Serialize(ref obj.Guid, writer);
        StaticNetSerializer_System_Byte.Serialize((System.Byte)obj.Type, writer);
        StaticNetSerializer_System_String.Serialize(ref obj.Author, writer);
        StaticNetSerializer_System_DateTime.Serialize(ref obj.UtcCreationTime, writer);
        ArrayNetSerializer_System_Byte.Serialize(ref obj.AssetData, writer);
    }

    public static void Deserialize(ref NetMessagePlayerAssets.Data obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Guid.Deserialize(ref obj.Guid, reader);
        obj.Type = (PlayerAssetType)StaticNetSerializer_System_Byte.Deserialize(reader);
        StaticNetSerializer_System_String.Deserialize(ref obj.Author, reader);
        StaticNetSerializer_System_DateTime.Deserialize(ref obj.UtcCreationTime, reader);
        ArrayNetSerializer_System_Byte.Deserialize(ref obj.AssetData, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerIdAssignment
{
    public static int GetSerializedBitSize(ref NetMessagePlayerIdAssignment obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetSerializedBitSize(ref obj.playerId);
        return result;
    }

    public static void Serialize(ref NetMessagePlayerIdAssignment obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.Serialize(ref obj.playerId, writer);
    }

    public static void Deserialize(ref NetMessagePlayerIdAssignment obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.Deserialize(ref obj.playerId, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerJoined
{
    public static int GetSerializedBitSize(ref NetMessagePlayerJoined obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerInfo.GetSerializedBitSize_Class(obj.playerInfo);
        return result;
    }

    public static void Serialize(ref NetMessagePlayerJoined obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerInfo.Serialize_Class(obj.playerInfo, writer);
    }

    public static void Deserialize(ref NetMessagePlayerJoined obj, BitStreamReader reader)
    {
        obj.playerInfo = StaticNetSerializer_PlayerInfo.Deserialize_Class(reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerLeft
{
    public static int GetSerializedBitSize(ref NetMessagePlayerLeft obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetSerializedBitSize(ref obj.playerId);
        return result;
    }

    public static void Serialize(ref NetMessagePlayerLeft obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.Serialize(ref obj.playerId, writer);
    }

    public static void Deserialize(ref NetMessagePlayerLeft obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.Deserialize(ref obj.playerId, reader);
    }
}
public static class StaticNetSerializer_NetMessagePlayerRepertoireSync
{
    public static int GetSerializedBitSize(ref NetMessagePlayerRepertoireSync obj)
    {
        int result = 0;
        result += ArrayNetSerializer_PlayerInfo.GetSerializedBitSize(ref obj.players);
        return result;
    }

    public static void Serialize(ref NetMessagePlayerRepertoireSync obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_PlayerInfo.Serialize(ref obj.players, writer);
    }

    public static void Deserialize(ref NetMessagePlayerRepertoireSync obj, BitStreamReader reader)
    {
        ArrayNetSerializer_PlayerInfo.Deserialize(ref obj.players, reader);
    }
}
public static class StaticNetSerializer_NetMessageRequestSimSync
{
    public static int GetSerializedBitSize(ref NetMessageRequestSimSync obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.AttemptTransferByDisk);
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.LocalMachineName);
        return result;
    }

    public static void Serialize(ref NetMessageRequestSimSync obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.Serialize(ref obj.AttemptTransferByDisk, writer);
        StaticNetSerializer_System_String.Serialize(ref obj.LocalMachineName, writer);
    }

    public static void Deserialize(ref NetMessageRequestSimSync obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.AttemptTransferByDisk, reader);
        StaticNetSerializer_System_String.Deserialize(ref obj.LocalMachineName, reader);
    }
}
public static class StaticNetSerializer_NetMessageRequestValueSync
{
    public static int GetSerializedBitSize(ref NetMessageRequestValueSync obj)
    {
        int result = 0;
        return result;
    }

    public static void Serialize(ref NetMessageRequestValueSync obj, BitStreamWriter writer)
    {
    }

    public static void Deserialize(ref NetMessageRequestValueSync obj, BitStreamReader reader)
    {
    }
}
public static class StaticNetSerializer_NetMessageSerializedSimulation
{
    public static int GetSerializedBitSize(ref NetMessageSerializedSimulation obj)
    {
        int result = 0;
        result += ArrayNetSerializer_System_Byte.GetSerializedBitSize(ref obj.SerializedSimulation);
        return result;
    }

    public static void Serialize(ref NetMessageSerializedSimulation obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_System_Byte.Serialize(ref obj.SerializedSimulation, writer);
    }

    public static void Deserialize(ref NetMessageSerializedSimulation obj, BitStreamReader reader)
    {
        ArrayNetSerializer_System_Byte.Deserialize(ref obj.SerializedSimulation, reader);
    }
}
public static class StaticNetSerializer_NetMessageSimPlayerIdAssignement
{
    public static int GetSerializedBitSize(ref NetMessageSimPlayerIdAssignement obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_PlayerId.GetSerializedBitSize(ref obj.PlayerId);
        return result;
    }

    public static void Serialize(ref NetMessageSimPlayerIdAssignement obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_PlayerId.Serialize(ref obj.PlayerId, writer);
    }

    public static void Deserialize(ref NetMessageSimPlayerIdAssignement obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_PlayerId.Deserialize(ref obj.PlayerId, reader);
    }
}
public static class StaticNetSerializer_NetMessageSimSyncFromFile
{
    public static int GetSerializedBitSize(ref NetMessageSimSyncFromFile obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.SerializedSimulationFilePath);
        return result;
    }

    public static void Serialize(ref NetMessageSimSyncFromFile obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.SerializedSimulationFilePath, writer);
    }

    public static void Deserialize(ref NetMessageSimSyncFromFile obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.SerializedSimulationFilePath, reader);
    }
}
public static class StaticNetSerializer_NetMessageSyncValue
{
    public static int GetSerializedBitSize(ref NetMessageSyncValue obj)
    {
        int result = 0;
        result += ArrayNetSerializer_System_Byte.GetSerializedBitSize(ref obj.ValueData);
        return result;
    }

    public static void Serialize(ref NetMessageSyncValue obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_System_Byte.Serialize(ref obj.ValueData, writer);
    }

    public static void Deserialize(ref NetMessageSyncValue obj, BitStreamReader reader)
    {
        ArrayNetSerializer_System_Byte.Deserialize(ref obj.ValueData, reader);
    }
}
public static class StaticNetSerializer_NetMessageValueSyncComplete
{
    public static int GetSerializedBitSize(ref NetMessageValueSyncComplete obj)
    {
        int result = 0;
        return result;
    }

    public static void Serialize(ref NetMessageValueSyncComplete obj, BitStreamWriter writer)
    {
    }

    public static void Deserialize(ref NetMessageValueSyncComplete obj, BitStreamReader reader)
    {
    }
}
public static class StaticNetSerializer_PersistentId
{
    public static int GetSerializedBitSize(ref PersistentId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetSerializedBitSize(ref obj.Value);
        return result;
    }

    public static void Serialize(ref PersistentId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.Serialize(ref obj.Value, writer);
    }

    public static void Deserialize(ref PersistentId obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.Deserialize(ref obj.Value, reader);
    }
}
public static class StaticNetSerializer_PlayerId
{
    public static int GetSerializedBitSize(ref PlayerId obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.Value);
        return result;
    }

    public static void Serialize(ref PlayerId obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt16.Serialize(ref obj.Value, writer);
    }

    public static void Deserialize(ref PlayerId obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.Value, reader);
    }
}
public static class StaticNetSerializer_PlayerInfo
{
    public static int GetSerializedBitSize_Class(PlayerInfo obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(PlayerInfo obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.PlayerName);
        result += StaticNetSerializer_PlayerId.GetSerializedBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.IsMaster);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        return result;
    }

    public static void Serialize_Class(PlayerInfo obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(PlayerInfo obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.PlayerName, writer);
        StaticNetSerializer_PlayerId.Serialize(ref obj.PlayerId, writer);
        StaticNetSerializer_System_Boolean.Serialize(ref obj.IsMaster, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
    }

    public static PlayerInfo Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        PlayerInfo obj = new PlayerInfo();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(PlayerInfo obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.PlayerName, reader);
        StaticNetSerializer_PlayerId.Deserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.IsMaster, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
    }
}
public static class StaticNetSerializer_SimCheatInput
{
    public static int GetSerializedBitSize_Class(SimCheatInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimCheatInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimCheatInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(SimCheatInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.Serialize(obj, writer);
    }

    public static SimCheatInput Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimCheatInput)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(SimCheatInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimCommand
{
    public static int GetSerializedBitSize_Class(SimCommand obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimCommand obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimCommand obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(SimCommand obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.Serialize(obj, writer);
    }

    public static SimCommand Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimCommand)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(SimCommand obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimCommandLoadScene
{
    public static int GetSerializedBitSize_Class(SimCommandLoadScene obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimCommandLoadScene obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.SceneName);
        result += StaticNetSerializer_SimMasterInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimCommandLoadScene obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimCommandLoadScene obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.SceneName, writer);
        StaticNetSerializer_SimMasterInput.Serialize(obj, writer);
    }

    public static SimCommandLoadScene Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimCommandLoadScene obj = new SimCommandLoadScene();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimCommandLoadScene obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.SceneName, reader);
        StaticNetSerializer_SimMasterInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInput
{
    public static int GetSerializedBitSize_Class(SimInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInput obj)
    {
        int result = 0;
        return result;
    }

    public static void Serialize_Class(SimInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(SimInput obj, BitStreamWriter writer)
    {
    }

    public static SimInput Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimInput)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(SimInput obj, BitStreamReader reader)
    {
    }
}
public static class StaticNetSerializer_SimInputCheatAddAllItems
{
    public static int GetSerializedBitSize_Class(SimInputCheatAddAllItems obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputCheatAddAllItems obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_SimCheatInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputCheatAddAllItems obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputCheatAddAllItems obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.PlayerId, writer);
        StaticNetSerializer_SimCheatInput.Serialize(obj, writer);
    }

    public static SimInputCheatAddAllItems Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputCheatAddAllItems obj = new SimInputCheatAddAllItems();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputCheatAddAllItems obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_SimCheatInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputCheatDamagePlayer
{
    public static int GetSerializedBitSize_Class(SimInputCheatDamagePlayer obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputCheatDamagePlayer obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.Damage);
        result += StaticNetSerializer_SimCheatInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputCheatDamagePlayer obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputCheatDamagePlayer obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.PlayerId, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.Damage, writer);
        StaticNetSerializer_SimCheatInput.Serialize(obj, writer);
    }

    public static SimInputCheatDamagePlayer Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputCheatDamagePlayer obj = new SimInputCheatDamagePlayer();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputCheatDamagePlayer obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.Damage, reader);
        StaticNetSerializer_SimCheatInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputCheatInfiniteAP
{
    public static int GetSerializedBitSize_Class(SimInputCheatInfiniteAP obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputCheatInfiniteAP obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_SimCheatInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputCheatInfiniteAP obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputCheatInfiniteAP obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.PlayerId, writer);
        StaticNetSerializer_SimCheatInput.Serialize(obj, writer);
    }

    public static SimInputCheatInfiniteAP Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputCheatInfiniteAP obj = new SimInputCheatInfiniteAP();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputCheatInfiniteAP obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_SimCheatInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputCheatKillPlayerPawn
{
    public static int GetSerializedBitSize_Class(SimInputCheatKillPlayerPawn obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputCheatKillPlayerPawn obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_SimCheatInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputCheatKillPlayerPawn obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputCheatKillPlayerPawn obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.PlayerId, writer);
        StaticNetSerializer_SimCheatInput.Serialize(obj, writer);
    }

    public static SimInputCheatKillPlayerPawn Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputCheatKillPlayerPawn obj = new SimInputCheatKillPlayerPawn();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputCheatKillPlayerPawn obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_SimCheatInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputCheatNextTurn
{
    public static int GetSerializedBitSize_Class(SimInputCheatNextTurn obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputCheatNextTurn obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimCheatInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputCheatNextTurn obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputCheatNextTurn obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimCheatInput.Serialize(obj, writer);
    }

    public static SimInputCheatNextTurn Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputCheatNextTurn obj = new SimInputCheatNextTurn();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputCheatNextTurn obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimCheatInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputCheatToggleInvincible
{
    public static int GetSerializedBitSize_Class(SimInputCheatToggleInvincible obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputCheatToggleInvincible obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.PlayerId);
        result += StaticNetSerializer_SimCheatInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputCheatToggleInvincible obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputCheatToggleInvincible obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.PlayerId, writer);
        StaticNetSerializer_SimCheatInput.Serialize(obj, writer);
    }

    public static SimInputCheatToggleInvincible Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputCheatToggleInvincible obj = new SimInputCheatToggleInvincible();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputCheatToggleInvincible obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.PlayerId, reader);
        StaticNetSerializer_SimCheatInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputPlayerCreate
{
    public static int GetSerializedBitSize_Class(SimInputPlayerCreate obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputPlayerCreate obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.PlayerName);
        result += StaticNetSerializer_SimMasterInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputPlayerCreate obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputPlayerCreate obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.PlayerName, writer);
        StaticNetSerializer_SimMasterInput.Serialize(obj, writer);
    }

    public static SimInputPlayerCreate Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputPlayerCreate obj = new SimInputPlayerCreate();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputPlayerCreate obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.PlayerName, reader);
        StaticNetSerializer_SimMasterInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputSetPlayerActive
{
    public static int GetSerializedBitSize_Class(SimInputSetPlayerActive obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimInputSetPlayerActive obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.PlayerID);
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.IsActive);
        result += StaticNetSerializer_SimMasterInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimInputSetPlayerActive obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimInputSetPlayerActive obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.PlayerID, writer);
        StaticNetSerializer_System_Boolean.Serialize(ref obj.IsActive, writer);
        StaticNetSerializer_SimMasterInput.Serialize(obj, writer);
    }

    public static SimInputSetPlayerActive Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimInputSetPlayerActive obj = new SimInputSetPlayerActive();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimInputSetPlayerActive obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.PlayerID, reader);
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.IsActive, reader);
        StaticNetSerializer_SimMasterInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimInputSubmission
{
    public static int GetSerializedBitSize(ref SimInputSubmission obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetSerializedBitSize(ref obj.InstigatorConnectionId);
        result += StaticNetSerializer_InputSubmissionId.GetSerializedBitSize(ref obj.ClientSubmissionId);
        result += StaticNetSerializer_SimInput.GetSerializedBitSize_Class(obj.Input);
        return result;
    }

    public static void Serialize(ref SimInputSubmission obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.Serialize(ref obj.InstigatorConnectionId, writer);
        StaticNetSerializer_InputSubmissionId.Serialize(ref obj.ClientSubmissionId, writer);
        StaticNetSerializer_SimInput.Serialize_Class(obj.Input, writer);
    }

    public static void Deserialize(ref SimInputSubmission obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.Deserialize(ref obj.InstigatorConnectionId, reader);
        StaticNetSerializer_InputSubmissionId.Deserialize(ref obj.ClientSubmissionId, reader);
        obj.Input = StaticNetSerializer_SimInput.Deserialize_Class(reader);
    }
}
public static class StaticNetSerializer_SimMasterInput
{
    public static int GetSerializedBitSize_Class(SimMasterInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimMasterInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimMasterInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(SimMasterInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimInput.Serialize(obj, writer);
    }

    public static SimMasterInput Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimMasterInput)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(SimMasterInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInput
{
    public static int GetSerializedBitSize_Class(SimPlayerInput obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInput obj)
    {
        int result = 0;
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInput obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInput obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimInput.Serialize(obj, writer);
    }

    public static SimPlayerInput Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimPlayerInput)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(SimPlayerInput obj, BitStreamReader reader)
    {
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputDropItem
{
    public static int GetSerializedBitSize_Class(SimPlayerInputDropItem obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputDropItem obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.ItemIndex);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputDropItem obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputDropItem obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.Serialize(ref obj.ItemIndex, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputDropItem Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimPlayerInputDropItem)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(SimPlayerInputDropItem obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.Deserialize(ref obj.ItemIndex, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputEquipItem
{
    public static int GetSerializedBitSize_Class(SimPlayerInputEquipItem obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputEquipItem obj)
    {
        int result = 0;
        result += StaticNetSerializer_Unity_Mathematics_int2.GetSerializedBitSize(ref obj.ItemEntityPosition);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.ItemIndex);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputEquipItem obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputEquipItem obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Unity_Mathematics_int2.Serialize(ref obj.ItemEntityPosition, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.ItemIndex, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputEquipItem Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (SimPlayerInputEquipItem)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(SimPlayerInputEquipItem obj, BitStreamReader reader)
    {
        StaticNetSerializer_Unity_Mathematics_int2.Deserialize(ref obj.ItemEntityPosition, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.ItemIndex, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputNextTurn
{
    public static int GetSerializedBitSize_Class(SimPlayerInputNextTurn obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputNextTurn obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.ReadyForNextTurn);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputNextTurn obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputNextTurn obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.Serialize(ref obj.ReadyForNextTurn, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputNextTurn Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputNextTurn obj = new SimPlayerInputNextTurn();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimPlayerInputNextTurn obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.ReadyForNextTurn, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputSelectStartingInventory
{
    public static int GetSerializedBitSize_Class(SimPlayerInputSelectStartingInventory obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputSelectStartingInventory obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.KitNumber);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputSelectStartingInventory obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputSelectStartingInventory obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.Serialize(ref obj.KitNumber, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputSelectStartingInventory Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputSelectStartingInventory obj = new SimPlayerInputSelectStartingInventory();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimPlayerInputSelectStartingInventory obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.Deserialize(ref obj.KitNumber, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputSetPawnDoodle
{
    public static int GetSerializedBitSize_Class(SimPlayerInputSetPawnDoodle obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputSetPawnDoodle obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Guid.GetSerializedBitSize(ref obj.DoodleId);
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.DoodleDirectionIsLookingRight);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputSetPawnDoodle obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputSetPawnDoodle obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Guid.Serialize(ref obj.DoodleId, writer);
        StaticNetSerializer_System_Boolean.Serialize(ref obj.DoodleDirectionIsLookingRight, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputSetPawnDoodle Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputSetPawnDoodle obj = new SimPlayerInputSetPawnDoodle();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimPlayerInputSetPawnDoodle obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Guid.Deserialize(ref obj.DoodleId, reader);
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.DoodleDirectionIsLookingRight, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputSetPawnName
{
    public static int GetSerializedBitSize_Class(SimPlayerInputSetPawnName obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputSetPawnName obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.Name);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputSetPawnName obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputSetPawnName obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.Name, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputSetPawnName Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputSetPawnName obj = new SimPlayerInputSetPawnName();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimPlayerInputSetPawnName obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.Name, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputUseInteractable
{
    public static int GetSerializedBitSize_Class(SimPlayerInputUseInteractable obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputUseInteractable obj)
    {
        int result = 0;
        result += StaticNetSerializer_Unity_Mathematics_int2.GetSerializedBitSize(ref obj.InteractablePosition);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputUseInteractable obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputUseInteractable obj, BitStreamWriter writer)
    {
        StaticNetSerializer_Unity_Mathematics_int2.Serialize(ref obj.InteractablePosition, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputUseInteractable Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputUseInteractable obj = new SimPlayerInputUseInteractable();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimPlayerInputUseInteractable obj, BitStreamReader reader)
    {
        StaticNetSerializer_Unity_Mathematics_int2.Deserialize(ref obj.InteractablePosition, reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimPlayerInputUseItem
{
    public static int GetSerializedBitSize_Class(SimPlayerInputUseItem obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(SimPlayerInputUseItem obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.ItemIndex);
        result += StaticNetSerializer_GameAction_UseParameters.GetSerializedBitSize_Class(obj.UseData);
        result += StaticNetSerializer_PersistentId.GetSerializedBitSize(ref obj.SimPlayerId);
        result += StaticNetSerializer_SimPlayerInput.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(SimPlayerInputUseItem obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(SimPlayerInputUseItem obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.Serialize(ref obj.ItemIndex, writer);
        StaticNetSerializer_GameAction_UseParameters.Serialize_Class(obj.UseData, writer);
        StaticNetSerializer_PersistentId.Serialize(ref obj.SimPlayerId, writer);
        StaticNetSerializer_SimPlayerInput.Serialize(obj, writer);
    }

    public static SimPlayerInputUseItem Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        SimPlayerInputUseItem obj = new SimPlayerInputUseItem();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(SimPlayerInputUseItem obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.Deserialize(ref obj.ItemIndex, reader);
        obj.UseData = StaticNetSerializer_GameAction_UseParameters.Deserialize_Class(reader);
        StaticNetSerializer_PersistentId.Deserialize(ref obj.SimPlayerId, reader);
        StaticNetSerializer_SimPlayerInput.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_SimulationControl_NetMessageSimTick
{
    public static int GetSerializedBitSize(ref SimulationControl.NetMessageSimTick obj)
    {
        int result = 0;
        result += StaticNetSerializer_SimulationControl_SimTickData.GetSerializedBitSize(ref obj.TickData);
        return result;
    }

    public static void Serialize(ref SimulationControl.NetMessageSimTick obj, BitStreamWriter writer)
    {
        StaticNetSerializer_SimulationControl_SimTickData.Serialize(ref obj.TickData, writer);
    }

    public static void Deserialize(ref SimulationControl.NetMessageSimTick obj, BitStreamReader reader)
    {
        StaticNetSerializer_SimulationControl_SimTickData.Deserialize(ref obj.TickData, reader);
    }
}
public static class StaticNetSerializer_SimulationControl_SimTickData
{
    public static int GetSerializedBitSize(ref SimulationControl.SimTickData obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_UInt32.GetSerializedBitSize(ref obj.ExpectedNewTickId);
        result += ListNetSerializer_SimInputSubmission.GetSerializedBitSize_Class(obj.InputSubmissions);
        return result;
    }

    public static void Serialize(ref SimulationControl.SimTickData obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_UInt32.Serialize(ref obj.ExpectedNewTickId, writer);
        ListNetSerializer_SimInputSubmission.Serialize_Class(obj.InputSubmissions, writer);
    }

    public static void Deserialize(ref SimulationControl.SimTickData obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_UInt32.Deserialize(ref obj.ExpectedNewTickId, reader);
        obj.InputSubmissions = ListNetSerializer_SimInputSubmission.Deserialize_Class(reader);
    }
}
public static class StaticNetSerializer_SyncedValueCurrentLevel
{
    public static int GetSerializedBitSize(ref SyncedValueCurrentLevel obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.Name);
        return result;
    }

    public static void Serialize(ref SyncedValueCurrentLevel obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.Name, writer);
    }

    public static void Deserialize(ref SyncedValueCurrentLevel obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.Name, reader);
    }
}
public static class StaticNetSerializer_TestMessage
{
    public static int GetSerializedBitSize_Class(TestMessage obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(TestMessage obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.valueString);
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.valueInt);
        result += StaticNetSerializer_System_UInt32.GetSerializedBitSize(ref obj.valueUInt);
        result += StaticNetSerializer_System_Int16.GetSerializedBitSize(ref obj.valueShort);
        result += StaticNetSerializer_System_UInt16.GetSerializedBitSize(ref obj.valueUShort);
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.valueBool);
        result += StaticNetSerializer_System_Byte.GetSerializedBitSize(ref obj.valueByte);
        result += ArrayNetSerializer_System_Int32.GetSerializedBitSize(ref obj.arrayOfInts);
        result += ListNetSerializer_System_Int32.GetSerializedBitSize_Class(obj.listOfInts);
        result += StaticNetSerializer_TestMessageCat.GetSerializedBitSize_Class(obj.cat);
        result += StaticNetSerializer_TestMessageDog.GetSerializedBitSize_Class(obj.dog);
        result += StaticNetSerializer_TestMessageAnimal.GetSerializedBitSize_Class(obj.animal);
        result += ArrayNetSerializer_TestMessageAnimal.GetSerializedBitSize(ref obj.animals);
        return result;
    }

    public static void Serialize_Class(TestMessage obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(TestMessage obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.valueString, writer);
        StaticNetSerializer_System_Int32.Serialize(ref obj.valueInt, writer);
        StaticNetSerializer_System_UInt32.Serialize(ref obj.valueUInt, writer);
        StaticNetSerializer_System_Int16.Serialize(ref obj.valueShort, writer);
        StaticNetSerializer_System_UInt16.Serialize(ref obj.valueUShort, writer);
        StaticNetSerializer_System_Boolean.Serialize(ref obj.valueBool, writer);
        StaticNetSerializer_System_Byte.Serialize(ref obj.valueByte, writer);
        ArrayNetSerializer_System_Int32.Serialize(ref obj.arrayOfInts, writer);
        ListNetSerializer_System_Int32.Serialize_Class(obj.listOfInts, writer);
        StaticNetSerializer_TestMessageCat.Serialize_Class(obj.cat, writer);
        StaticNetSerializer_TestMessageDog.Serialize_Class(obj.dog, writer);
        StaticNetSerializer_TestMessageAnimal.Serialize_Class(obj.animal, writer);
        ArrayNetSerializer_TestMessageAnimal.Serialize(ref obj.animals, writer);
    }

    public static TestMessage Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessage obj = new TestMessage();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(TestMessage obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.valueString, reader);
        StaticNetSerializer_System_Int32.Deserialize(ref obj.valueInt, reader);
        StaticNetSerializer_System_UInt32.Deserialize(ref obj.valueUInt, reader);
        StaticNetSerializer_System_Int16.Deserialize(ref obj.valueShort, reader);
        StaticNetSerializer_System_UInt16.Deserialize(ref obj.valueUShort, reader);
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.valueBool, reader);
        StaticNetSerializer_System_Byte.Deserialize(ref obj.valueByte, reader);
        ArrayNetSerializer_System_Int32.Deserialize(ref obj.arrayOfInts, reader);
        obj.listOfInts = ListNetSerializer_System_Int32.Deserialize_Class(reader);
        obj.cat = StaticNetSerializer_TestMessageCat.Deserialize_Class(reader);
        obj.dog = StaticNetSerializer_TestMessageDog.Deserialize_Class(reader);
        obj.animal = StaticNetSerializer_TestMessageAnimal.Deserialize_Class(reader);
        ArrayNetSerializer_TestMessageAnimal.Deserialize(ref obj.animals, reader);
    }
}
public static class StaticNetSerializer_TestMessageAnimal
{
    public static int GetSerializedBitSize_Class(TestMessageAnimal obj)
    {
        if (obj == null)
            return 1;
        return 1 + NetSerializer.GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(TestMessageAnimal obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.name);
        return result;
    }

    public static void Serialize_Class(TestMessageAnimal obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        NetSerializer.Serialize(obj, writer);
    }
    public static void Serialize(TestMessageAnimal obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_String.Serialize(ref obj.name, writer);
    }

    public static TestMessageAnimal Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        return (TestMessageAnimal)NetSerializer.Deserialize(reader);
    }
    public static void Deserialize(TestMessageAnimal obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_String.Deserialize(ref obj.name, reader);
    }
}
public static class StaticNetSerializer_TestMessageCat
{
    public static int GetSerializedBitSize_Class(TestMessageCat obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(TestMessageCat obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj.numberOfLivesLeft);
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.name);
        result += StaticNetSerializer_TestMessageAnimal.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(TestMessageCat obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(TestMessageCat obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Int32.Serialize(ref obj.numberOfLivesLeft, writer);
        StaticNetSerializer_System_String.Serialize(ref obj.name, writer);
        StaticNetSerializer_TestMessageAnimal.Serialize(obj, writer);
    }

    public static TestMessageCat Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessageCat obj = new TestMessageCat();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(TestMessageCat obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Int32.Deserialize(ref obj.numberOfLivesLeft, reader);
        StaticNetSerializer_System_String.Deserialize(ref obj.name, reader);
        StaticNetSerializer_TestMessageAnimal.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_TestMessageDog
{
    public static int GetSerializedBitSize_Class(TestMessageDog obj)
    {
        if (obj == null)
            return 1;
        return 1 + GetSerializedBitSize(obj);
    }

    public static int GetSerializedBitSize(TestMessageDog obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Boolean.GetSerializedBitSize(ref obj.isAGoodBoy);
        result += StaticNetSerializer_System_String.GetSerializedBitSize(ref obj.name);
        result += StaticNetSerializer_TestMessageAnimal.GetSerializedBitSize(obj);
        return result;
    }

    public static void Serialize_Class(TestMessageDog obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        Serialize(obj, writer);
    }
    public static void Serialize(TestMessageDog obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Boolean.Serialize(ref obj.isAGoodBoy, writer);
        StaticNetSerializer_System_String.Serialize(ref obj.name, writer);
        StaticNetSerializer_TestMessageAnimal.Serialize(obj, writer);
    }

    public static TestMessageDog Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        TestMessageDog obj = new TestMessageDog();
        Deserialize(obj, reader);
        return obj;
    }
    public static void Deserialize(TestMessageDog obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Boolean.Deserialize(ref obj.isAGoodBoy, reader);
        StaticNetSerializer_System_String.Deserialize(ref obj.name, reader);
        StaticNetSerializer_TestMessageAnimal.Deserialize(obj, reader);
    }
}
public static class StaticNetSerializer_UnityEngine_Vector2
{
    public static int GetSerializedBitSize(ref UnityEngine.Vector2 obj)
    {
        int result = 0;
        result += StaticNetSerializer_System_Single.GetSerializedBitSize(ref obj.x);
        result += StaticNetSerializer_System_Single.GetSerializedBitSize(ref obj.y);
        return result;
    }

    public static void Serialize(ref UnityEngine.Vector2 obj, BitStreamWriter writer)
    {
        StaticNetSerializer_System_Single.Serialize(ref obj.x, writer);
        StaticNetSerializer_System_Single.Serialize(ref obj.y, writer);
    }

    public static void Deserialize(ref UnityEngine.Vector2 obj, BitStreamReader reader)
    {
        StaticNetSerializer_System_Single.Deserialize(ref obj.x, reader);
        StaticNetSerializer_System_Single.Deserialize(ref obj.y, reader);
    }
}

public static class ArrayNetSerializer_System_Byte
{
    public static int GetSerializedBitSize(ref System.Byte[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_System_Byte.GetSerializedBitSize(ref obj[i]);
        }
        return result;
    }

    public static void Serialize(ref System.Byte[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Byte.Serialize(ref obj[i], writer);
        }
    }

    public static void Deserialize(ref System.Byte[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new System.Byte[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Byte.Deserialize(ref obj[i], reader);
        }
    }
}

public static class ArrayNetSerializer_GameAction_ParameterData
{
    public static int GetSerializedBitSize(ref GameAction.ParameterData[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_GameAction_ParameterData.GetSerializedBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void Serialize(ref GameAction.ParameterData[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_GameAction_ParameterData.Serialize_Class(obj[i], writer);
        }
    }

    public static void Deserialize(ref GameAction.ParameterData[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new GameAction.ParameterData[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_GameAction_ParameterData.Deserialize_Class(reader);
        }
    }
}

public static class ArrayNetSerializer_System_Int32
{
    public static int GetSerializedBitSize(ref System.Int32[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref obj[i]);
        }
        return result;
    }

    public static void Serialize(ref System.Int32[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Int32.Serialize(ref obj[i], writer);
        }
    }

    public static void Deserialize(ref System.Int32[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new System.Int32[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Int32.Deserialize(ref obj[i], reader);
        }
    }
}

public static class ArrayNetSerializer_NetMessagePlayerAssets_Data
{
    public static int GetSerializedBitSize(ref NetMessagePlayerAssets.Data[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_NetMessagePlayerAssets_Data.GetSerializedBitSize(ref obj[i]);
        }
        return result;
    }

    public static void Serialize(ref NetMessagePlayerAssets.Data[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_NetMessagePlayerAssets_Data.Serialize(ref obj[i], writer);
        }
    }

    public static void Deserialize(ref NetMessagePlayerAssets.Data[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new NetMessagePlayerAssets.Data[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_NetMessagePlayerAssets_Data.Deserialize(ref obj[i], reader);
        }
    }
}

public static class ArrayNetSerializer_PlayerInfo
{
    public static int GetSerializedBitSize(ref PlayerInfo[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_PlayerInfo.GetSerializedBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void Serialize(ref PlayerInfo[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_PlayerInfo.Serialize_Class(obj[i], writer);
        }
    }

    public static void Deserialize(ref PlayerInfo[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new PlayerInfo[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_PlayerInfo.Deserialize_Class(reader);
        }
    }
}

public static class ArrayNetSerializer_TestMessageAnimal
{
    public static int GetSerializedBitSize(ref TestMessageAnimal[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_TestMessageAnimal.GetSerializedBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void Serialize(ref TestMessageAnimal[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_TestMessageAnimal.Serialize_Class(obj[i], writer);
        }
    }

    public static void Deserialize(ref TestMessageAnimal[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new TestMessageAnimal[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_TestMessageAnimal.Deserialize_Class(reader);
        }
    }
}

public static class ListNetSerializer_SimInputSubmission
{
    public static int GetSerializedBitSize_Class(List<SimInputSubmission> obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Count; i++)
        {
            var x = obj[i];
            result += StaticNetSerializer_SimInputSubmission.GetSerializedBitSize(ref x);
        }
        return result;
    }

    public static void Serialize_Class(List<SimInputSubmission> obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Count);
        for (int i = 0; i < obj.Count; i++)
        {
            var x = obj[i];
            StaticNetSerializer_SimInputSubmission.Serialize(ref x, writer);
        }
    }

    public static List<SimInputSubmission> Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        int size = reader.ReadInt32();
        List<SimInputSubmission> obj = new List<SimInputSubmission>(size);
        for (int i = 0; i < size; i++)
        {
            SimInputSubmission x = default;
            StaticNetSerializer_SimInputSubmission.Deserialize(ref x, reader);
            obj.Add(x);
        }
        return obj;
    }
}

public static class ListNetSerializer_System_Int32
{
    public static int GetSerializedBitSize_Class(List<System.Int32> obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Count; i++)
        {
            var x = obj[i];
            result += StaticNetSerializer_System_Int32.GetSerializedBitSize(ref x);
        }
        return result;
    }

    public static void Serialize_Class(List<System.Int32> obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteInt32(obj.Count);
        for (int i = 0; i < obj.Count; i++)
        {
            var x = obj[i];
            StaticNetSerializer_System_Int32.Serialize(ref x, writer);
        }
    }

    public static List<System.Int32> Deserialize_Class(BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            return null;
        }
        int size = reader.ReadInt32();
        List<System.Int32> obj = new List<System.Int32>(size);
        for (int i = 0; i < size; i++)
        {
            System.Int32 x = default;
            StaticNetSerializer_System_Int32.Deserialize(ref x, reader);
            obj.Add(x);
        }
        return obj;
    }
}
