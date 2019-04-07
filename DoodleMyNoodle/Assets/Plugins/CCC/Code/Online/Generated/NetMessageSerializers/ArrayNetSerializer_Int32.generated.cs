// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_Int32
{
    public static int GetNetBitSize(ref Int32[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt16) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += NetSerializer_Int32.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref Int32[] obj, BitStreamWriter writer)
    {
        if (obj == null)
        {
            writer.WriteBit(false);
            return;
        }
        writer.WriteBit(true);
        writer.WriteUInt16((UInt16)obj.Length);
        for (int i = 0; i < obj.Length; i++)
        {
            NetSerializer_Int32.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref Int32[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new Int32[reader.ReadUInt16()];
        for (int i = 0; i < obj.Length; i++)
        {
            NetSerializer_Int32.NetDeserialize(ref obj[i], reader);
        }
    }
}
