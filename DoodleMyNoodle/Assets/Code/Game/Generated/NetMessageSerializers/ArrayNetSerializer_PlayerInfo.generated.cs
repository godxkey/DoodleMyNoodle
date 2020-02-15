// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_PlayerInfo
{
    public static int GetNetBitSize(ref PlayerInfo[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_PlayerInfo.GetNetBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref PlayerInfo[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteUInt32((UInt32)obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_PlayerInfo.NetSerialize_Class(obj[i], writer);
        }
    }

    public static void NetDeserialize(ref PlayerInfo[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new PlayerInfo[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_PlayerInfo.NetDeserialize_Class(reader);
        }
    }
}
