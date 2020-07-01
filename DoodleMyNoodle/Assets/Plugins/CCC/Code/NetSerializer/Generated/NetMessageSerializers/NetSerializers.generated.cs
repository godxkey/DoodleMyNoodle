// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class ArrayNetSerializer_System_Byte
{
    public static int GetNetBitSize(ref System.Byte[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_System_Byte.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref System.Byte[] obj, BitStreamWriter writer)
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
            StaticNetSerializer_System_Byte.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref System.Byte[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new System.Byte[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Byte.NetDeserialize(ref obj[i], reader);
        }
    }
}

public static class ArrayNetSerializer_System_Int32
{
    public static int GetNetBitSize(ref System.Int32[] obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Length; i++)
        {
            result += StaticNetSerializer_System_Int32.GetNetBitSize(ref obj[i]);
        }
        return result;
    }

    public static void NetSerialize(ref System.Int32[] obj, BitStreamWriter writer)
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
            StaticNetSerializer_System_Int32.NetSerialize(ref obj[i], writer);
        }
    }

    public static void NetDeserialize(ref System.Int32[] obj, BitStreamReader reader)
    {
        if (reader.ReadBit() == false)
        {
            obj = null;
            return;
        }
        obj = new System.Int32[reader.ReadInt32()];
        for (int i = 0; i < obj.Length; i++)
        {
            StaticNetSerializer_System_Int32.NetDeserialize(ref obj[i], reader);
        }
    }
}

public static class ListNetSerializer_System_Int32
{
    public static int GetNetBitSize_Class(List<System.Int32> obj)
    {
        if (obj == null)
            return 1;
        int result = 1 + sizeof(Int32) * 8;
        for (int i = 0; i < obj.Count; i++)
        {
            var x = obj[i];
            result += StaticNetSerializer_System_Int32.GetNetBitSize(ref x);
        }
        return result;
    }

    public static void NetSerialize_Class(List<System.Int32> obj, BitStreamWriter writer)
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
            StaticNetSerializer_System_Int32.NetSerialize(ref x, writer);
        }
    }

    public static List<System.Int32> NetDeserialize_Class(BitStreamReader reader)
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
            StaticNetSerializer_System_Int32.NetDeserialize(ref x, reader);
            obj.Add(x);
        }
        return obj;
    }
}
