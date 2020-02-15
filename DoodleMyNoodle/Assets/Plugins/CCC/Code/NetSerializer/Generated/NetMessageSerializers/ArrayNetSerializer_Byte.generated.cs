// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_Byte
{
    public static int GetNetBitSize(ref Byte[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_Byte.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref Byte[] obj, BitStreamWriter writer)
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
            StaticNetSerializer_Byte.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref Byte[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new Byte[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_Byte.NetDeserialize(ref obj[i], reader);
        }
    }
}
