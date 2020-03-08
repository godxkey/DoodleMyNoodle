using System;

public static class ArrayNetSerializer_Object
{
    public static int GetNetBitSize(ref object[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(UInt32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_Object.GetNetBitSize_Class(obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref object[] obj, BitStreamWriter writer)
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
            StaticNetSerializer_Object.NetSerialize_Class(obj[i], writer);
        }
    }

    public static void NetDeserialize(ref object[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new object[reader.ReadUInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            obj[i] = StaticNetSerializer_Object.NetDeserialize_Class(reader);
        }
    }
}
